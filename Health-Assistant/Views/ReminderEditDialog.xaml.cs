using System.Windows;
using System.Windows.Controls;
using Health_Assistant.Models;
using Health_Assistant.ViewModels;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;

namespace Health_Assistant.Views;

public partial class ReminderEditDialog : FluentWindow
{
    public ReminderTask? ResultTask { get; private set; }
    private readonly ReminderEditViewModel _vm;
    private readonly Guid? _existingId;

    public ReminderEditDialog(ReminderTask? existingTask)
    {
        InitializeComponent();
        _existingId = existingTask?.Id;
        _vm = new ReminderEditViewModel();
        DataContext = _vm;

        if (existingTask != null)
        {
            _vm.LoadFromTask(existingTask);
            Title = $"编辑提醒 - {existingTask.Name}";
        }
        else
        {
            Title = "新增提醒";
        }

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        switch (_vm.SelectedType)
        {
            case ReminderType.RepeatInterval:
                RadioRepeat.IsChecked = true;
                break;
            case ReminderType.DailyTime:
                RadioDaily.IsChecked = true;
                break;
            case ReminderType.OneTimeDelay:
                RadioDelay.IsChecked = true;
                break;
            case ReminderType.SpecificDateTime:
                RadioSpecific.IsChecked = true;
                break;
        }
    }

    private void OnRadioTypeChecked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton rb && rb.IsChecked == true)
        {
            _vm.SelectedType = rb.Name switch
            {
                "RadioRepeat" => ReminderType.RepeatInterval,
                "RadioDaily" => ReminderType.DailyTime,
                "RadioDelay" => ReminderType.OneTimeDelay,
                "RadioSpecific" => ReminderType.SpecificDateTime,
                _ => ReminderType.RepeatInterval
            };
        }
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_vm.Name))
        {
            MessageBox.Show("请输入任务名称", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_vm.IsRepeatMode && _vm.IntervalValue <= 0)
        {
            MessageBox.Show("请输入有效的间隔时间", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_vm.IsDelayMode && _vm.DelayValue <= 0)
        {
            MessageBox.Show("请输入有效的延迟时间", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ResultTask = _vm.BuildTask(_existingId);
        DialogResult = true;
        Close();
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
