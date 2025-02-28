using System.Diagnostics;
using Avalonia.Threading;
using Serilog;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;
using TDT4900_MasterThesis.viewmodel;

namespace TDT4900_MasterThesis.engine;

public class SimulationEngine
{
    private long _currentTick;

    private int _frameCounter = 0;
    private int _fps = 0;

    private int _tickCounter = 0;
    private int _tps = 0;

    private Stopwatch _stopwatch;

    /// <summary>
    /// Lock to prevent multiple threads from updating the simulation data at the same time
    /// </summary>
    public static readonly Lock UpdateLock = new();

    public readonly List<IUpdatable> UpdatableComponents;
    public readonly List<IDrawable> DrawableComponents;

    private List<NodeState>[] _stateHistory;

    public SimulationEngine(
        AppSettings appSettings,
        IEnumerable<IUpdatable> updatableComponents,
        IEnumerable<IDrawable> drawableComponents
    )
    {
        TargetFps = appSettings.Simulation.TargetFps;
        TargetTps = appSettings.Simulation.TargetTps;

        UpdatableComponents = updatableComponents.ToList();
        DrawableComponents = drawableComponents.ToList();
    }

    /// <summary>
    /// Target Ticks Per Second
    /// </summary>
    public int TargetTps;

    /// <summary>
    /// Target Frames Per Second
    /// </summary>
    public int TargetFps;

    private bool _isRunning = false;

    public MainWindowViewModel? MainWindowViewModel { get; set; }

    public async Task RunSimulation(CancellationToken stoppingToken)
    {
        _isRunning = true;
        _stopwatch = new Stopwatch();

        double updateInterval;
        double renderInterval;
        var statUpdate = 1000.0;

        double nextUpdate = _stopwatch.ElapsedMilliseconds;
        double nextRender = _stopwatch.ElapsedMilliseconds;
        double nextStatUpdate = _stopwatch.ElapsedMilliseconds + statUpdate;

        if (MainWindowViewModel != null)
            MainWindowViewModel.SimulationState = "Running";

        while (_isRunning && !stoppingToken.IsCancellationRequested)
        {
            updateInterval = 1000.0 / TargetTps;
            renderInterval = 1000.0 / TargetFps;

            double currentTime = _stopwatch.ElapsedMilliseconds;

            if (currentTime >= nextUpdate)
            {
                Update(++_currentTick);
                nextUpdate += updateInterval;
            }

            if (currentTime >= nextRender)
            {
                Render();
                nextRender += renderInterval;
            }

            if (currentTime >= nextStatUpdate)
            {
                _fps = (int)(_frameCounter / (statUpdate / 1000.0));
                _tps = (int)(_tickCounter / (statUpdate / 1000.0));

                if (MainWindowViewModel != null)
                {
                    MainWindowViewModel.Tps = _tps;
                    MainWindowViewModel.Fps = _fps;
                }

                _tickCounter = _frameCounter = 0;
                nextStatUpdate += statUpdate;
            }

            await Task.Delay((int)(Math.Min(updateInterval, renderInterval) / 4.0), stoppingToken);
        }

        if (MainWindowViewModel != null)
            MainWindowViewModel.SimulationState = "Stopped";

        Log.Information("Simulation engine has stopped after {ticks} ticks.", _currentTick);
    }

    private void Update(long currentTick)
    {
        lock (UpdateLock)
        {
            if (MainWindowViewModel != null)
                MainWindowViewModel.CurrentTick = currentTick;

            _tickCounter++;
            UpdatableComponents.ForEach(c => c.Update(currentTick));
        }
    }

    private void Render()
    {
        var readyToRender = DrawableComponents.All(c => c.IsReadyToDraw);

        if (!readyToRender)
            return;

        DrawableComponents.ForEach(c =>
        {
            Dispatcher.UIThread.Invoke(c.Draw, DispatcherPriority.Background);
        });
        _frameCounter++;
    }

    public void Pause()
    {
        if (MainWindowViewModel != null)
            MainWindowViewModel.SimulationState = "Paused";

        _stopwatch.Stop();
    }

    public void Play()
    {
        if (MainWindowViewModel != null)
            MainWindowViewModel.SimulationState = "Running";

        _stopwatch.Start();
    }

    public void Reset()
    {
        lock (UpdateLock)
        {
            _currentTick = 0;
            Play();
            UpdatableComponents.ForEach(c => c.ResetComponent());
        }
    }
}
