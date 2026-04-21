using System.Windows;
using PomodoroTimer.Services;

namespace PomodoroTimer;

public partial class App : Application
{
    public static MellowtelService Mellowtel { get; private set; } = null!;

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        var config = AppConfig.Load();
        if (!config.HasValidKey)
        {
            MessageBox.Show(
                "appsettings.json is missing or does not contain a valid Mellowtel publishable key.\n\n" +
                "Copy appsettings.example.json to appsettings.json and fill in your key from mellowtel.com. " +
                "See README.md for details.",
                "Pomodoro Timer — configuration required",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            Shutdown();
            return;
        }

        Mellowtel = new MellowtelService(config.Mellowtel.PublishableKey, config.Mellowtel.PluginId);

        // Resumes background work silently if the user previously opted in.
        await Mellowtel.InitializeAsync();

        var main = new MainWindow();
        MainWindow = main;
        main.Show();

        // First-launch consent: we only prompt once. On subsequent launches
        // users change their mind through Settings.
        if (!Mellowtel.HasCompletedFirstRun)
        {
            await Mellowtel.ShowConsentAndOptInAsync(main);
        }
    }

    private async void Application_Exit(object sender, ExitEventArgs e)
    {
        if (Mellowtel is not null)
        {
            await Mellowtel.ShutdownAsync();
        }
    }
}
