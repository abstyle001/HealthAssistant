using Health_Assistant.Models;

namespace Health_Assistant.ViewModels;

public class ReminderEditViewModel : ViewModelBase
{
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private ReminderType _selectedType = ReminderType.RepeatInterval;
    public ReminderType SelectedType
    {
        get => _selectedType;
        set
        {
            if (SetProperty(ref _selectedType, value))
            {
                OnPropertyChanged(nameof(IsRepeatMode));
                OnPropertyChanged(nameof(IsDelayMode));
                OnPropertyChanged(nameof(IsSpecificMode));
                OnPropertyChanged(nameof(IsDailyMode));
            }
        }
    }

    private int _intervalValue = 30;
    public int IntervalValue
    {
        get => _intervalValue;
        set => SetProperty(ref _intervalValue, value);
    }

    private int _popupDuration = 5;
    public int PopupDuration
    {
        get => _popupDuration;
        set => SetProperty(ref _popupDuration, value);
    }

    private IntervalUnit _intervalUnit = IntervalUnit.Minutes;
    public IntervalUnit IntervalUnit
    {
        get => _intervalUnit;
        set => SetProperty(ref _intervalUnit, value);
    }

    public int IntervalUnitIndex
    {
        get => (int)_intervalUnit;
        set => IntervalUnit = (IntervalUnit)value;
    }

    private string _repeatStartHour = "09";
    public string RepeatStartHour
    {
        get => _repeatStartHour;
        set
        {
            if (TrySetTimePart(ref _repeatStartHour, value, 23))
                OnPropertyChanged(nameof(RepeatStartTime));
        }
    }

    private string _repeatStartMinute = "00";
    public string RepeatStartMinute
    {
        get => _repeatStartMinute;
        set
        {
            if (TrySetTimePart(ref _repeatStartMinute, value, 59))
                OnPropertyChanged(nameof(RepeatStartTime));
        }
    }

    public TimeOnly RepeatStartTime =>
        new(int.TryParse(_repeatStartHour, out var h) ? Math.Clamp(h, 0, 23) : 9,
            int.TryParse(_repeatStartMinute, out var m) ? Math.Clamp(m, 0, 59) : 0);

    private string _repeatEndHour = "18";
    public string RepeatEndHour
    {
        get => _repeatEndHour;
        set
        {
            if (TrySetTimePart(ref _repeatEndHour, value, 23))
                OnPropertyChanged(nameof(RepeatEndTime));
        }
    }

    private string _repeatEndMinute = "00";
    public string RepeatEndMinute
    {
        get => _repeatEndMinute;
        set
        {
            if (TrySetTimePart(ref _repeatEndMinute, value, 59))
                OnPropertyChanged(nameof(RepeatEndTime));
        }
    }

    public TimeOnly RepeatEndTime =>
        new(int.TryParse(_repeatEndHour, out var h) ? Math.Clamp(h, 0, 23) : 18,
            int.TryParse(_repeatEndMinute, out var m) ? Math.Clamp(m, 0, 59) : 0);

    private int _delayValue = 1;
    public int DelayValue
    {
        get => _delayValue;
        set => SetProperty(ref _delayValue, value);
    }

    private string _dailyHour = "09";
    public string DailyHour
    {
        get => _dailyHour;
        set
        {
            if (TrySetTimePart(ref _dailyHour, value, 23))
                OnPropertyChanged(nameof(DailyTimeOfTask));
        }
    }

    private string _dailyMinute = "00";
    public string DailyMinute
    {
        get => _dailyMinute;
        set
        {
            if (TrySetTimePart(ref _dailyMinute, value, 59))
                OnPropertyChanged(nameof(DailyTimeOfTask));
        }
    }

    public TimeOnly DailyTimeOfTask =>
        new(int.TryParse(_dailyHour, out var h) ? Math.Clamp(h, 0, 23) : 9,
            int.TryParse(_dailyMinute, out var m) ? Math.Clamp(m, 0, 59) : 0);

    private IntervalUnit _delayUnit = IntervalUnit.Minutes;
    public IntervalUnit DelayUnit
    {
        get => _delayUnit;
        set => SetProperty(ref _delayUnit, value);
    }

    public int DelayUnitIndex
    {
        get => (int)_delayUnit;
        set => DelayUnit = (IntervalUnit)value;
    }

    private DateTime _targetDateTime = DateTime.Now.AddHours(1);
    public DateTime TargetDateTime
    {
        get => _targetDateTime;
        set => SetProperty(ref _targetDateTime, value);
    }

    public DateTime TargetDate
    {
        get => TargetDateTime.Date;
        set => TargetDateTime = value.Date
            .AddHours(int.TryParse(_targetHour, out var h) ? h : 0)
            .AddMinutes(int.TryParse(_targetMinute, out var m) ? m : 0);
    }

    private string _targetHour = DateTime.Now.AddHours(1).Hour.ToString("D2");
    public string TargetHour
    {
        get => _targetHour;
        set
        {
            if (TrySetTimePart(ref _targetHour, value, 23))
                UpdateTargetDateTimeFromParts();
        }
    }

    private string _targetMinute = "00";
    public string TargetMinute
    {
        get => _targetMinute;
        set
        {
            if (TrySetTimePart(ref _targetMinute, value, 59))
                UpdateTargetDateTimeFromParts();
        }
    }

    private void UpdateTargetDateTimeFromParts()
    {
        var date = TargetDateTime.Date;
        var h = int.TryParse(TargetHour, out var hh) ? Math.Clamp(hh, 0, 23) : 0;
        var m = int.TryParse(TargetMinute, out var mm) ? Math.Clamp(mm, 0, 59) : 0;
        TargetDateTime = date.AddHours(h).AddMinutes(m);
    }

    private static bool TrySetTimePart(ref string field, string value, int max)
    {
        if (string.IsNullOrEmpty(value))
        {
            field = value;
            return true;
        }

        if (!int.TryParse(value, out var num) || num < 0 || num > max)
            return false;

        field = value;
        return true;
    }

    public bool IsRepeatMode => SelectedType == ReminderType.RepeatInterval;
    public bool IsDelayMode => SelectedType == ReminderType.OneTimeDelay;
    public bool IsSpecificMode => SelectedType == ReminderType.SpecificDateTime;
    public bool IsDailyMode => SelectedType == ReminderType.DailyTime;

    public void LoadFromTask(ReminderTask task)
    {
        Name = task.Name;
        Description = task.Description;
        SelectedType = task.Type;

        IntervalValue = task.IntervalValue == 0 ? 30 : task.IntervalValue;
        IntervalUnit = task.IntervalUnit;

        var start = task.RepeatStartTime ?? new TimeOnly(9, 0);
        RepeatStartHour = start.Hour.ToString("D2");
        RepeatStartMinute = start.Minute.ToString("D2");

        var end = task.RepeatEndTime ?? new TimeOnly(18, 0);
        RepeatEndHour = end.Hour.ToString("D2");
        RepeatEndMinute = end.Minute.ToString("D2");

        DelayValue = task.DelayValue == 0 ? 1 : task.DelayValue;
        DelayUnit = task.DelayUnit;

        TargetDateTime = task.TargetDateTime ?? DateTime.Now.AddHours(1);
        TargetHour = TargetDateTime.Hour.ToString("D2");
        TargetMinute = TargetDateTime.Minute.ToString("D2");

        var daily = task.DailyTime ?? new TimeOnly(9, 0);
        DailyHour = daily.Hour.ToString("D2");
        DailyMinute = daily.Minute.ToString("D2");

        PopupDuration = task.PopupDuration;
    }

    public ReminderTask BuildTask(Guid? existingId = null)
    {
        return new ReminderTask
        {
            Id = existingId ?? Guid.NewGuid(),
            Name = Name,
            Description = Description,
            Type = SelectedType,
            IntervalValue = IntervalValue,
            IntervalUnit = IntervalUnit,
            RepeatStartTime = RepeatStartTime,
            RepeatEndTime = RepeatEndTime,
            DelayValue = DelayValue,
            DelayUnit = DelayUnit,
            TargetDateTime = TargetDateTime,
            DailyTime = DailyTimeOfTask,
            IsActive = true,
            PopupDuration = PopupDuration
        };
    }
}
