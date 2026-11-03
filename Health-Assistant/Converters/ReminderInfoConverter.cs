using System.Globalization;
using System.Windows.Data;
using Health_Assistant.Models;

namespace Health_Assistant.Converters;

public class ReminderInfoConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 9 || values[0] is not ReminderType type)
            return string.Empty;

        return type switch
        {
            ReminderType.RepeatInterval => BuildRepeatInfo(values),
            ReminderType.OneTimeDelay => BuildDelayInfo(values),
            ReminderType.SpecificDateTime => BuildDateTimeInfo(values),
            ReminderType.DailyTime => BuildDailyInfo(values),
            _ => string.Empty
        };
    }

    private static string BuildDailyInfo(object[] values)
    {
        if (values[8] is TimeOnly t)
            return $"每天 {t:HH:mm} 定时提醒";
        return "每天定时提醒";
    }

    private static string BuildRepeatInfo(object[] values)
    {
        var intervalValue = SafeInt(values[1]);
        var intervalUnit = SafeEnum<IntervalUnit>(values[2]);
        var startTime = values[3] as TimeOnly?;
        var endTime = values[4] as TimeOnly?;

        var unitStr = intervalUnit == IntervalUnit.Minutes ? "分钟" : "小时";
        var result = $"每隔 {intervalValue} {unitStr}";

        if (startTime.HasValue && endTime.HasValue)
            result += $"（{startTime.Value:HH:mm} - {endTime.Value:HH:mm}）";
        else if (startTime.HasValue)
            result += $"（从 {startTime.Value:HH:mm} 开始）";

        return result;
    }

    private static string BuildDelayInfo(object[] values)
    {
        var delayValue = SafeInt(values[5]);
        var delayUnit = SafeEnum<IntervalUnit>(values[6]);
        var unitStr = delayUnit == IntervalUnit.Minutes ? "分钟" : "小时";
        return $"{delayValue} {unitStr}后提醒一次";
    }

    private static string BuildDateTimeInfo(object[] values)
    {
        if (values[7] is DateTime dt)
            return dt.ToString("yyyy年MM月dd日 HH:mm");
        return "未设置时间";
    }

    private static int SafeInt(object value) =>
        value is int i ? i : 0;

    private static T SafeEnum<T>(object value) where T : struct =>
        value is T e ? e : default;

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
