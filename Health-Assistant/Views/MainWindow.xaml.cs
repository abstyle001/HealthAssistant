using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Health_Assistant.Models;
using Health_Assistant.ViewModels;
using Wpf.Ui.Controls;

namespace Health_Assistant.Views;

public partial class MainWindow : FluentWindow
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel.StartService();
    }

    private void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        _viewModel.StopService();
    }

    private void OnToggleChecked(object sender, RoutedEventArgs e)
    {
        if (sender is Wpf.Ui.Controls.ToggleSwitch toggle && toggle.Tag is ReminderTask task)
        {
            _viewModel.ToggleTask(task.Id);
        }
    }

    private void OnToggleUnchecked(object sender, RoutedEventArgs e)
    {
        if (sender is Wpf.Ui.Controls.ToggleSwitch toggle && toggle.Tag is ReminderTask task)
        {
            _viewModel.ToggleTask(task.Id);
        }
    }

    private async void OnDeleteClick(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Button btn && btn.CommandParameter is ReminderTask task)
        {
            var dialog = new ContentDialog(DialogHost);
            dialog.SetCurrentValue(ContentDialog.TitleProperty, "确认删除");
            dialog.SetCurrentValue(System.Windows.Controls.ContentControl.ContentProperty, $"确定要删除提醒 \"{task.Name}\" 吗？");
            dialog.SetCurrentValue(ContentDialog.PrimaryButtonTextProperty, "删除");
            dialog.SetCurrentValue(ContentDialog.CloseButtonTextProperty, "取消");
            dialog.SetCurrentValue(ContentDialog.DefaultButtonProperty, ContentDialogButton.Close);

            var result = await dialog.ShowAsync(CancellationToken.None);
            if (result == ContentDialogResult.Primary)
            {
                _viewModel.ExecuteDelete(task.Id);
            }
        }
    }
}
