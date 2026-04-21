# Pomodoro Timer

A clean, minimal Pomodoro timer for Windows. Focus, short break, repeat — and that's it.

Pomodoro Timer is funded by [Mellowtel](https://www.mellowtel.com): if you opt in, the app shares a tiny fraction of your unused bandwidth in the background so the developer can keep the app free. You can decline at install time or turn it off any time in Settings. No personal data is ever collected.

## Features

- 25 / 5 minute focus and break cycles (configurable)
- Start, pause, reset
- Session counter
- Settings for timer durations and privacy
- Runs on Windows 10 / 11, WPF / .NET 10

## Running from source

### Prerequisites

- Windows 10 / 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022+, Rider, or the `dotnet` CLI
- A Mellowtel publishable key from [mellowtel.com](https://www.mellowtel.com)

### Repo layout

This project references the Mellowtel SDK by **project reference**, so it expects `mellowtel-windows` to sit next to this folder:

```
some-parent/
├── mellowtel-windows/
└── windows-demo-project/    ← this repo
```

### Build and run

```bash
# 1. Copy the config template and fill in your publishable key
cp PomodoroTimer/appsettings.example.json PomodoroTimer/appsettings.json
# then edit PomodoroTimer/appsettings.json

# 2. Build and run
dotnet build PomodoroTimer.sln
dotnet run --project PomodoroTimer
```

On first launch you'll see the privacy consent dialog. Accept and bandwidth sharing starts silently in the background; decline and it stays off. You can flip the choice any time from **⚙ Settings → Privacy & Bandwidth Sharing**.

## How Mellowtel is wired in

If you're building your own Windows app and want to use Mellowtel, this codebase is deliberately small and easy to read.

The entire SDK integration lives in two files:

- [PomodoroTimer/Services/MellowtelService.cs](PomodoroTimer/Services/MellowtelService.cs) — wraps the SDK lifecycle: construct, start, consent, opt-in/out, shutdown.
- [PomodoroTimer/App.xaml.cs](PomodoroTimer/App.xaml.cs) — hooks the service into application startup and exit.

The consent dialog itself is in [PomodoroTimer/Dialogs/ConsentDialog.xaml](PomodoroTimer/Dialogs/ConsentDialog.xaml) — customise the copy and styling for your own app. The opt-in toggle lives in [PomodoroTimer/SettingsWindow.xaml.cs](PomodoroTimer/SettingsWindow.xaml.cs).

The core of the integration is four calls:

```csharp
var sdk = new Mellowtel(publishableKey, new MellowtelOptions { PluginId = "your-app" });
await sdk.StartIfOptedInAsync();                // silent resume if previously opted in
if (new ConsentDialog().ShowDialog() == true) { sdk.OptIn(); await sdk.StartAsync(); }
await sdk.StopAsync(); sdk.Dispose();           // on shutdown
```

Everything else in this codebase is UX wrapping around those calls.

## Further reading

- [Mellowtel Windows docs](https://docs.mellowtel.com/desktop-app/windows)
- [Mellowtel Windows SDK on GitHub](https://github.com/mellowtel-inc/mellowtel-windows)

## License

MIT — copy any part into your own project. The Mellowtel SDK has its own license in the SDK repo.
