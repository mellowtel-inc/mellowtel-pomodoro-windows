using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using PomodoroTimer.ViewModels;

namespace PomodoroTimer;

public partial class MainWindow : Window
{
    private readonly PomodoroViewModel _vm = new();

    public MainWindow()
    {
        InitializeComponent();
        _vm.PropertyChanged += OnViewModelChanged;
        RefreshUi();
    }

    private void OnViewModelChanged(object? sender, PropertyChangedEventArgs e)
    {
        Dispatcher.Invoke(RefreshUi);
    }

    private void RefreshUi()
    {
        TimeDisplay.Text = _vm.TimeDisplay;
        PhaseLabel.Text = _vm.PhaseLabel;
        SessionLabel.Text = _vm.SessionLabel;

        var accent = _vm.CurrentPhase == Phase.Work
            ? (Brush)FindResource("AccentBrush")
            : (Brush)FindResource("BreakBrush");
        PhaseLabel.Foreground = accent;

        StartButton.IsEnabled = !_vm.IsRunning;
        PauseButton.IsEnabled = _vm.IsRunning;
    }

    private void StartButton_Click(object sender, RoutedEventArgs e) => _vm.Start();
    private void PauseButton_Click(object sender, RoutedEventArgs e) => _vm.Pause();
    private void ResetButton_Click(object sender, RoutedEventArgs e) => _vm.Reset();

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settings = new SettingsWindow(_vm) { Owner = this };
        settings.ShowDialog();
        RefreshUi();
    }
}
