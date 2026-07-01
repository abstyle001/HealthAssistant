# Health Assistant - Project Context

## Overview
A Windows desktop health reminder app built with .NET 10 WPF and WPF-UI 4.3.0 (Dark theme).
Helps sedentary workers set timed reminders (drink water, stretch, etc.) with system popup notifications.

## Tech Stack
- **Framework**: .NET 10 (net10.0-windows)
- **UI**: WPF with WPF-UI 4.3.0 (Fluent/WinUI style)
- **Theme**: Dark (configured in App.xaml)
- **Architecture**: Simplified MVVM (no DI framework, hand-rolled ViewModelBase)
- **Persistence**: JSON file at `%AppData%/HealthAssistant/reminders.json`
- **Scheduling**: `System.Windows.Threading.DispatcherTimer` (1-second tick)

## Project Structure
```
Models/
  ReminderTask.cs              # Data model (Id, Name, Type, Interval, TimeOnly, etc.)

Services/
  ReminderService.cs           # Timer-based scheduler, fires ReminderTriggered event
  ReminderStorage.cs           # JSON load/save via System.Text.Json

ViewModels/
  ViewModelBase.cs             # INotifyPropertyChanged base
  RelayCommand.cs              # ICommand implementation
  MainViewModel.cs             # Main window logic (CRUD, notification handler)
  ReminderEditViewModel.cs     # Edit dialog logic (3 modes, TimeSpan? for TimePicker)

Views/
  MainWindow.xaml/.cs          # Main UI - FluentWindow, task card list
  ReminderEditDialog.xaml/.cs  # Add/edit dialog - 3 reminder modes
  ReminderNotification.xaml/.cs # Pop-up notification - 5s auto-close, bottom-right

Converters/
  ReminderTypeConverter.cs     # Type -> icon symbol name
  ReminderInfoConverter.cs     # Multi-binding -> info text string
  InverseCountToVisibilityConverter.cs  # Count 0 -> Visible
```

## Key Enums
```csharp
enum ReminderType { RepeatInterval, OneTimeDelay, SpecificDateTime }
enum IntervalUnit { Minutes, Hours }
```

## Reminder Modes
1. **RepeatInterval**: Every X minutes/hours, from StartTime to StartTime each day
2. **OneTimeDelay**: After X minutes/hours, fire once then deactivate
3. **SpecificDateTime**: Fire at exact date+time, then deactivate

## WPF-UI Controls Used
- `ui:FluentWindow` - Window base class (code-behind must inherit `Wpf.Ui.Controls.FluentWindow`)
- `ui:TitleBar` - Title bar with `TrailingContent` (NOT `HeaderRight`)
- `ui:Button` - Buttons with `Appearance` property (Primary, Secondary, Transparent)
- `ui:TextBox` - Text input with `PlaceholderText`
- `ui:NumberBox` - Numeric input with `Value`, `Minimum`, `Maximum`
- `ui:TimePicker` - Time selection with `SelectedTime` (type `TimeSpan?`), `ClockIdentifier`
- `ui:ToggleSwitch` - On/Off toggle
- `ui:CardExpander` - Expandable card for task list items
- `ui:SymbolIcon` - Icon with `Symbol` property (string name like "AddCircle24")

## Known Gotchas
- `ui:TimePicker.SelectedTime` is `TimeSpan?`, NOT `TimeOnly`
- `ui:TitleBar` uses `TrailingContent` for right-side content, NOT `HeaderRight`
- `MessageBox` is ambiguous between `System.Windows.MessageBox` and `Wpf.Ui.Controls.MessageBox` - use alias
- `TextBlockStyles.Header` / `TextBlockStyles.Subtitle` may not exist - use inline FontSize/FontWeight
- All XAML code-behind files must inherit from `Wpf.Ui.Controls.FluentWindow` when XAML root is `ui:FluentWindow`

## Build & Run
```bash
dotnet build
dotnet run
```

## NuGet Packages
- `WPF-UI` Version 4.3.0
