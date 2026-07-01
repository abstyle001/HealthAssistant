using System.IO;
using System.Text.Json;
using Health_Assistant.Models;

namespace Health_Assistant.Services;

public static class ReminderStorage
{
    private static readonly string StorageDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "HealthAssistant");

    private static readonly string StorageFile = Path.Combine(StorageDir, "reminders.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static List<ReminderTask> Load()
    {
        try
        {
            if (!File.Exists(StorageFile))
                return new List<ReminderTask>();

            var json = File.ReadAllText(StorageFile);
            return JsonSerializer.Deserialize<List<ReminderTask>>(json, JsonOptions) ?? new List<ReminderTask>();
        }
        catch
        {
            return new List<ReminderTask>();
        }
    }

    public static void Save(List<ReminderTask> tasks)
    {
        Directory.CreateDirectory(StorageDir);
        var json = JsonSerializer.Serialize(tasks, JsonOptions);
        File.WriteAllText(StorageFile, json);
    }
}
