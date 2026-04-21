using System.IO;
using System.Text.Json;

namespace PomodoroTimer.Services;

public sealed class AppConfig
{
    public MellowtelConfig Mellowtel { get; init; } = new();

    public sealed class MellowtelConfig
    {
        public string PublishableKey { get; init; } = "";
        public string PluginId { get; init; } = "pomodoro-demo";
    }

    private const string PlaceholderKey = "YOUR_PUBLISHABLE_KEY_HERE";

    public bool HasValidKey =>
        !string.IsNullOrWhiteSpace(Mellowtel.PublishableKey) &&
        Mellowtel.PublishableKey != PlaceholderKey;

    public static AppConfig Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(path))
        {
            return new AppConfig();
        }

        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<AppConfig>(json, options) ?? new AppConfig();
    }
}
