# Health Assistant - 健康助手

一款 Windows 桌面健康提醒应用，帮助久坐办公的你定时提醒喝水、站起来活动等。

## 功能特性

- **三种提醒模式**
  - 周期重复：每天从指定时间开始，每隔 X 分钟/小时提醒一次
  - 延迟一次：X 分钟/小时后提醒一次，然后自动关闭
  - 指定时间：在指定的日期和时间弹窗提醒

- **任务管理**
  - 新增、编辑、删除提醒任务
  - 启用/禁用开关，随时暂停或恢复提醒
  - 任务列表实时显示下次提醒时间

- **弹窗通知**
  - 屏幕右下角弹窗通知，支持多个弹窗堆叠显示
  - 可自定义弹窗持续时间（1-60 秒）
  - 5 秒后自动关闭，也可手动关闭

- **数据持久化**
  - 所有任务保存到 `%AppData%/HealthAssistant/reminders.json`
  - 关闭应用后重新打开，数据不丢失

## 技术栈

| 项目 | 技术 |
|------|------|
| 框架 | .NET 10 (WPF) |
| UI 库 | WPF-UI 4.3.0 (Fluent/WinUI 风格) |
| 主题 | Dark (深色) |
| 架构 | 简化 MVVM（手写 ViewModelBase） |
| 存储 | System.Text.Json |
| 调度 | DispatcherTimer（1 秒轮询） |

## 项目结构

```
Health-Assistant/
├── Models/
│   └── ReminderTask.cs              # 数据模型
├── Services/
│   ├── ReminderService.cs           # 提醒调度核心
│   └── ReminderStorage.cs           # JSON 持久化
├── ViewModels/
│   ├── ViewModelBase.cs             # INotifyPropertyChanged 基类
│   ├── RelayCommand.cs              # ICommand 实现
│   ├── MainViewModel.cs             # 主窗口逻辑
│   └── ReminderEditViewModel.cs     # 编辑对话框逻辑
├── Views/
│   ├── MainWindow.xaml/.cs          # 主界面
│   ├── ReminderEditDialog.xaml/.cs  # 新增/编辑对话框
│   └── ReminderNotification.xaml/.cs # 提醒弹窗
├── Converters/
│   ├── ReminderTypeConverter.cs     # 类型→图标
│   ├── ReminderInfoConverter.cs     # 多值绑定→信息文本
│   └── InverseCountToVisibilityConverter.cs
└── App.xaml                         # 应用入口，配置深色主题
```

## 快速开始

### 环境要求
- .NET 10 SDK
- Windows 10/11

### 构建运行

```bash
# 克隆项目后进入目录
cd Health-Assistant

# 构建
dotnet build

# 运行
dotnet run
```

或在 Visual Studio 中按 F5 直接运行。

## 使用说明

1. 点击右上角 **+** 按钮添加新提醒
2. 输入任务名称（如"喝水"）和描述
3. 选择提醒类型：
   - **周期重复**：设置间隔时间和每天的起止时间
   - **延迟一次**：设置延迟时长
   - **指定时间**：选择具体日期和时间
4. 设置弹窗持续时间（默认 5 秒）
5. 点击保存，任务开始生效
6. 使用开关随时启用/禁用任务

## 文件说明

| 文件 | 说明 |
|------|------|
| `ReminderTask.cs` | 定义提醒任务的数据结构，包含名称、类型、间隔、触发时间等 |
| `ReminderService.cs` | 核心调度服务，每秒检查是否需要触发提醒，管理任务的增删改查 |
| `ReminderStorage.cs` | 将任务列表序列化为 JSON 文件，实现数据持久化 |
| `MainViewModel.cs` | 主窗口的业务逻辑，处理任务列表的展示和交互 |
| `ReminderEditViewModel.cs` | 编辑对话框的逻辑，支持三种提醒模式的切换和表单验证 |
| `ReminderNotification.xaml` | 提醒弹窗的 UI，支持多个弹窗堆叠显示 |
