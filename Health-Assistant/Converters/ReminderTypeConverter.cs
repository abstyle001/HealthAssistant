using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Health_Assistant.Models;

namespace Health_Assistant.Converters;

public class ReminderTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ReminderType type)
        {
            return type switch
            {
                ReminderType.RepeatInterval => "ArrowClockwise16",
                ReminderType.OneTimeDelay => "Timer16",
                ReminderType.SpecificDateTime => "Calendar16",
                ReminderType.DailyTime => "ClockAlarm16",
                _ => "Question16"
            };
        }
        return "Question16";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
