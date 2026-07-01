using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Health_Assistant.Models;
using Health_Assistant.Services;

namespace Health_Assistant.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ReminderService _service;

    public ObservableCollection<ReminderTask> Tasks { get; } = new();

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    private ReminderTask? _selectedTask;
    public ReminderTask? SelectedTask
    {
        get => _selectedTask;
        set => SetProperty(ref _selectedTask, value);
    }

    public MainViewModel()
    {
        _service = new ReminderService();
        _service.ReminderTriggered += OnReminderTriggered;
        _service.TasksChanged += RefreshTasks;

        AddCommand = new RelayCommand(_ => OpenEditDialog(null));
        EditCommand = new RelayCommand(task => OpenEditDialog(task as ReminderTask));
        DeleteCommand = new RelayCommand(task => { });
    }

    public void StartService()
    {
        _service.Start();
        RefreshTasks();
    }

    public void StopService()
    {
        _service.Stop();
    }

    private void RefreshTasks()
    {
        var latest = _service.GetAll().OrderBy(t => t.NextTriggerTime).ToList();
        Tasks.Clear();
        foreach (var task in latest)
            Tasks.Add(task);
    }

    private void OpenEditDialog(ReminderTask? existingTask)
    {
        var dialog = new Views.ReminderEditDialog(existingTask);
        dialog.Owner = App.Current.MainWindow;
        if (dialog.ShowDialog() == true)
        {
            if (existingTask == null)
            {
                _service.Add(dialog.ResultTask!);
            }
            else
            {
                _service.Update(dialog.ResultTask!);
            }
        }
    }

    public void ToggleTask(Guid taskId)
    {
        _service.ToggleActive(taskId);
    }

    public void ExecuteDelete(Guid taskId)
    {
        _service.Delete(taskId);
    }

    private void OnReminderTriggered(ReminderTask task)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var notification = new Views.ReminderNotification(task);
            notification.Show();
        });
    }
}
