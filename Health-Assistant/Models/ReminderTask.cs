namespace Health_Assistant.Models;

public enum ReminderType
{
    RepeatInterval,
    OneTimeDelay,
    SpecificDateTime,
    DailyTime
}

public enum IntervalUnit
{
    Minutes,
    Hours
}

public class ReminderTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReminderType Type { get; set; }

    public int IntervalValue { get; set; }
    public IntervalUnit IntervalUnit { get; set; }
    public TimeOnly? RepeatStartTime { get; set; }
    public TimeOnly? RepeatEndTime { get; set; }

    public int DelayValue { get; set; }
    public IntervalUnit DelayUnit { get; set; }

    public DateTime? TargetDateTime { get; set; }

    public TimeOnly? DailyTime { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? NextTriggerTime { get; set; }
    public int PopupDuration { get; set; } = 5;
}
