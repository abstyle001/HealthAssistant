using System.Windows.Threading;
using Health_Assistant.Models;

namespace Health_Assistant.Services;

public class ReminderService
{
    private readonly DispatcherTimer _timer;
    private List<ReminderTask> _tasks = new();
    private readonly object _lock = new();

    public event Action<ReminderTask>? ReminderTriggered;
    public event Action? TasksChanged;

    public ReminderService()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += Timer_Tick;
    }

    public void Start()
    {
        _tasks = ReminderStorage.Load();
        foreach (var task in _tasks)
        {
            if (task.IsActive)
                task.NextTriggerTime = CalculateNextTrigger(task);
        }
        ReminderStorage.Save(_tasks);
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    public List<ReminderTask> GetAll()
    {
        lock (_lock)
        {
            return _tasks.Select(t => new ReminderTask
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Type = t.Type,
                IsActive = t.IsActive,
                NextTriggerTime = t.NextTriggerTime,
                IntervalValue = t.IntervalValue,
                IntervalUnit = t.IntervalUnit,
                RepeatStartTime = t.RepeatStartTime,
                RepeatEndTime = t.RepeatEndTime,
                DelayValue = t.DelayValue,
                DelayUnit = t.DelayUnit,
                TargetDateTime = t.TargetDateTime,
                CreatedAt = t.CreatedAt,
                PopupDuration = t.PopupDuration
            }).ToList();
        }
    }

    public void Add(ReminderTask task)
    {
        lock (_lock)
        {
            task.CreatedAt = DateTime.Now;
            task.NextTriggerTime = CalculateNextTrigger(task);
            _tasks.Add(task);
            ReminderStorage.Save(_tasks);
        }
        TasksChanged?.Invoke();
    }

    public void Update(ReminderTask task)
    {
        lock (_lock)
        {
            var index = _tasks.FindIndex(t => t.Id == task.Id);
            if (index >= 0)
            {
                task.NextTriggerTime = CalculateNextTrigger(task);
                _tasks[index] = task;
                ReminderStorage.Save(_tasks);
            }
        }
        TasksChanged?.Invoke();
    }

    public void Delete(Guid id)
    {
        lock (_lock)
        {
            _tasks.RemoveAll(t => t.Id == id);
            ReminderStorage.Save(_tasks);
        }
        TasksChanged?.Invoke();
    }

    public void ToggleActive(Guid id)
    {
        lock (_lock)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.IsActive = !task.IsActive;
                task.NextTriggerTime = task.IsActive
                    ? CalculateNextTrigger(task)
                    : null;
                ReminderStorage.Save(_tasks);
            }
        }
        TasksChanged?.Invoke();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        var now = DateTime.Now;
        List<Guid> triggeredIds = new();

        lock (_lock)
        {
            foreach (var task in _tasks)
            {
                if (!task.IsActive || task.NextTriggerTime == null)
                    continue;

                if (now >= task.NextTriggerTime.Value)
                {
                    triggeredIds.Add(task.Id);

                    if (task.Type == ReminderType.RepeatInterval)
                    {
                        task.NextTriggerTime = CalculateNextTrigger(task);
                    }
                    else
                    {
                        task.IsActive = false;
                        task.NextTriggerTime = null;
                    }
                }
            }

            if (triggeredIds.Count > 0)
                ReminderStorage.Save(_tasks);
        }

        if (triggeredIds.Count == 0)
            return;

        foreach (var id in triggeredIds)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
                ReminderTriggered?.Invoke(task);
        }

        TasksChanged?.Invoke();
    }

    private static DateTime? CalculateNextTrigger(ReminderTask task)
    {
        if (!task.IsActive) return null;

        var now = DateTime.Now;

        return task.Type switch
        {
            ReminderType.RepeatInterval => CalculateRepeatNext(task, now),
            ReminderType.OneTimeDelay => CalculateDelayNext(task, now),
            ReminderType.SpecificDateTime => task.TargetDateTime,
            _ => null
        };
    }

    private static DateTime CalculateRepeatNext(ReminderTask task, DateTime now)
    {
        var interval = task.IntervalUnit == IntervalUnit.Minutes
            ? TimeSpan.FromMinutes(task.IntervalValue)
            : TimeSpan.FromHours(task.IntervalValue);

        if (task.RepeatStartTime.HasValue && task.RepeatEndTime.HasValue)
        {
            var todayStart = now.Date.Add(task.RepeatStartTime.Value.ToTimeSpan());
            var todayEnd = now.Date.Add(task.RepeatEndTime.Value.ToTimeSpan());

            if (now < todayStart)
                return todayStart;

            if (now >= todayEnd)
                return now.Date.AddDays(1).Add(task.RepeatStartTime.Value.ToTimeSpan());

            var next = todayStart;
            while (next <= now)
                next = next.Add(interval);

            if (next > todayEnd)
                return now.Date.AddDays(1).Add(task.RepeatStartTime.Value.ToTimeSpan());

            return next;
        }

        if (task.RepeatStartTime.HasValue)
        {
            var todayStart = now.Date.Add(task.RepeatStartTime.Value.ToTimeSpan());
            if (now < todayStart)
                return todayStart;

            var next = todayStart;
            while (next <= now)
                next = next.Add(interval);

            return next;
        }

        return now.Add(interval);
    }

    private static DateTime CalculateDelayNext(ReminderTask task, DateTime now)
    {
        var delay = task.DelayUnit == IntervalUnit.Minutes
            ? TimeSpan.FromMinutes(task.DelayValue)
            : TimeSpan.FromHours(task.DelayValue);

        return now.Add(delay);
    }
}
