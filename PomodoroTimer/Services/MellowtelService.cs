using System.Diagnostics;
using System.Windows;
using MellowtelWin;
using PomodoroTimer.Dialogs;

namespace PomodoroTimer.Services;

// Mellowtel SDK integration — the whole lifecycle lives in this one file.
//
//   1. Construct Mellowtel with your publishable key (App.OnStartup).
//   2. Call InitializeAsync() at startup — resumes background work if the
//      user previously opted in. No-op otherwise.
//   3. On first run, show ConsentDialog. Record OptIn() or OptOut() so we
//      don't re-prompt on subsequent launches.
//   4. Expose an opt-in toggle in Settings so users can change their mind.
//   5. Call ShutdownAsync() on app exit to stop the WebSocket and Dispose.
//
// The SDK persists opt-in state to %LOCALAPPDATA%\MellowtelWin\config.json
// automatically — no extra storage needed on our side.

public sealed class MellowtelService
{
    private readonly Mellowtel _sdk;
    private CancellationTokenSource? _cts;

    public MellowtelService(string publishableKey, string pluginId)
    {
        _sdk = new Mellowtel(publishableKey, new MellowtelOptions
        {
            PluginId = pluginId,
            // Logs go to the Debug output. Flip to true in a release build.
            DisableLogs = false,
        });

        _sdk.ConnectionStateChanged += (_, connected) =>
            Debug.WriteLine($"[Mellowtel] Connection: {(connected ? "connected" : "disconnected")}");

        _sdk.MessageReceived += (_, msg) =>
            Debug.WriteLine($"[Mellowtel] Request received: {msg.Url}");
    }

    public bool IsOptedIn => _sdk.GetOptInStatus();

    public string SdkVersion => _sdk.GetVersion();

    public string NodeId => _sdk.GetNodeId();

    public async Task InitializeAsync()
    {
        _cts = new CancellationTokenSource();
        await _sdk.StartIfOptedInAsync(_cts.Token);
    }

    // Shows the consent dialog, persists the user's answer, and starts the
    // background service if they agreed. Either way this satisfies
    // HasCompletedFirstRun so we don't prompt again.
    public async Task<bool> ShowConsentAndOptInAsync(Window owner)
    {
        var dialog = new ConsentDialog { Owner = owner };
        var agreed = dialog.ShowDialog() == true;

        if (agreed)
        {
            _sdk.OptIn();
            _cts ??= new CancellationTokenSource();
            await _sdk.StartAsync(_cts.Token);
        }
        else
        {
            _sdk.OptOut();
        }

        return agreed;
    }

    public async Task OptInAsync(Window owner)
    {
        await ShowConsentAndOptInAsync(owner);
    }

    public async Task OptOutAsync()
    {
        await _sdk.StopAsync();
        _sdk.OptOut();
    }

    public async Task ShutdownAsync()
    {
        _cts?.Cancel();
        await _sdk.StopAsync();
        _sdk.Dispose();
    }
}
