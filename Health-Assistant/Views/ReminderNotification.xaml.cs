using System.Windows;
using System.Windows.Threading;
using Health_Assistant.Models;

namespace Health_Assistant.Views;

public partial class ReminderNotification : Window
{
    private readonly DispatcherTimer _autoCloseTimer;
    private static readonly List<Window> _openNotifications = new();
    private static readonly object _listLock = new();

    public ReminderNotification(ReminderTask task)
    {
        InitializeComponent();
        DataContext = task;

        _autoCloseTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(task.PopupDuration > 0 ? task.PopupDuration : 5)
        };
        _autoCloseTimer.Tick += (_, _) =>
        {
            _autoCloseTimer.Stop();
            Close();
        };

        Loaded += OnLoaded;
        Closed += OnClosed;

        _autoCloseTimer.Start();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        PositionWindow();
        lock (_listLock)
        {
            _openNotifications.Add(this);
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        lock (_listLock)
        {
            _openNotifications.Remove(this);
        }
    }

    private void PositionWindow()
    {
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        var screenHeight = SystemParameters.PrimaryScreenHeight;
        const double margin = 20;
        const double gap = 8;

        Left = screenWidth - Width - margin;

        lock (_listLock)
        {
            double top = screenHeight - Height - 60;
            foreach (var win in _openNotifications)
            {
                if (win != this && win.IsLoaded && win.WindowState != WindowState.Minimized)
                {
                    var winTop = win.Top;
                    var winHeight = win.Height;
                    if (top >= winTop && top < winTop + winHeight + gap)
                    {
                        top = winTop - Height - gap;
                    }
                }
            }
            Top = Math.Max(top, 0);
        }
    }

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        _autoCloseTimer.Stop();
        Close();
    }
}
