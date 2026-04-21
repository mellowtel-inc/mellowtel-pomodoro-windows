using System.Windows;
using PomodoroTimer.ViewModels;

namespace PomodoroTimer;

public partial class SettingsWindow : Window
{
    private readonly PomodoroViewModel _vm;
    private bool _suppressToggleEvents;

    public SettingsWindow(PomodoroViewModel vm)
    {
        InitializeComponent();
        _vm = vm;

        WorkMinutesBox.Text = _vm.WorkMinutes.ToString();
        BreakMinutesBox.Text = _vm.BreakMinutes.ToString();
        SessionsBox.Text = _vm.SessionsPerCycle.ToString();

        SdkVersionLabel.Text = $"Version: {App.Mellowtel.SdkVersion}";
        NodeIdLabel.Text = $"Node ID: {App.Mellowtel.NodeId}";

        RefreshOptInUi();
    }

    private void RefreshOptInUi()
    {
        _suppressToggleEvents = true;
        var optedIn = App.Mellowtel.IsOptedIn;
        OptInToggle.IsChecked = optedIn;
        OptInStatusLabel.Text = optedIn ? "Sharing is on" : "Sharing is off";
        OptInHintLabel.Text = optedIn
            ? "Thanks for supporting the app."
            : "Turn on to help support development.";
        _suppressToggleEvents = false;
    }

    private async void OptInToggle_Checked(object sender, RoutedEventArgs e)
    {
        if (_suppressToggleEvents) return;

        // Re-show the consent dialog before enabling so the user reaffirms
        // their choice. If they decline, flip the toggle back off.
        await App.Mellowtel.OptInAsync(this);
        RefreshOptInUi();
    }

    private async void OptInToggle_Unchecked(object sender, RoutedEventArgs e)
    {
        if (_suppressToggleEvents) return;
        await App.Mellowtel.OptOutAsync();
        RefreshOptInUi();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(WorkMinutesBox.Text, out var work) && work > 0)
            _vm.WorkMinutes = work;
        if (int.TryParse(BreakMinutesBox.Text, out var brk) && brk > 0)
            _vm.BreakMinutes = brk;
        if (int.TryParse(SessionsBox.Text, out var sessions) && sessions > 0)
            _vm.SessionsPerCycle = sessions;

        _vm.Reset();
        Close();
    }
}
