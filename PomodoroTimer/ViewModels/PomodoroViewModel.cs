using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace PomodoroTimer.ViewModels;

public enum Phase { Work, Break }

public sealed class PomodoroViewModel : INotifyPropertyChanged
{
    private readonly DispatcherTimer _timer;
    private TimeSpan _remaining;
    private Phase _phase = Phase.Work;
    private int _sessionsCompleted;
    private bool _isRunning;

    public int WorkMinutes { get; set; } = 25;
    public int BreakMinutes { get; set; } = 5;
    public int SessionsPerCycle { get; set; } = 4;

    public PomodoroViewModel()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += OnTick;
        Reset();
    }

    public string TimeDisplay => $"{(int)_remaining.TotalMinutes:D2}:{_remaining.Seconds:D2}";
    public string PhaseLabel => _phase == Phase.Work ? "Focus" : "Break";
    public string SessionLabel => $"Session {(_sessionsCompleted % SessionsPerCycle) + 1} of {SessionsPerCycle}";
    public bool IsRunning => _isRunning;
    public Phase CurrentPhase => _phase;

    public void Start()
    {
        if (_isRunning) return;
        _isRunning = true;
        _timer.Start();
        OnPropertyChanged(nameof(IsRunning));
    }

    public void Pause()
    {
        if (!_isRunning) return;
        _isRunning = false;
        _timer.Stop();
        OnPropertyChanged(nameof(IsRunning));
    }

    public void Reset()
    {
        _timer.Stop();
        _isRunning = false;
        _phase = Phase.Work;
        _sessionsCompleted = 0;
        _remaining = TimeSpan.FromMinutes(WorkMinutes);
        OnPropertyChanged(nameof(TimeDisplay));
        OnPropertyChanged(nameof(PhaseLabel));
        OnPropertyChanged(nameof(SessionLabel));
        OnPropertyChanged(nameof(IsRunning));
        OnPropertyChanged(nameof(CurrentPhase));
    }

    private void OnTick(object? sender, EventArgs e)
    {
        _remaining -= TimeSpan.FromSeconds(1);
        if (_remaining.TotalSeconds <= 0)
        {
            AdvancePhase();
        }
        OnPropertyChanged(nameof(TimeDisplay));
    }

    private void AdvancePhase()
    {
        if (_phase == Phase.Work)
        {
            _sessionsCompleted++;
            _phase = Phase.Break;
            _remaining = TimeSpan.FromMinutes(BreakMinutes);
        }
        else
        {
            _phase = Phase.Work;
            _remaining = TimeSpan.FromMinutes(WorkMinutes);
        }
        OnPropertyChanged(nameof(PhaseLabel));
        OnPropertyChanged(nameof(SessionLabel));
        OnPropertyChanged(nameof(CurrentPhase));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
