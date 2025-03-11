using System.Diagnostics;
using Avalonia.Threading;
using Serilog;
using TDT4900_MasterThesis.Model;
using TDT4900_MasterThesis.Model.Graph;
using TDT4900_MasterThesis.viewmodel;
using EventHandler = TDT4900_MasterThesis.Handler.EventHandler;

namespace TDT4900_MasterThesis.Engine;

public class SimulationEngine
{
    private readonly GraphPlotViewModel? _graphPlotViewModel;
    private readonly SequencePlotViewModel? _sequencePlotViewModel;
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

    public readonly List<IDrawable> DrawableComponents;

    public SimulationEngine(
        AppSettings appSettings,
        IEnumerable<IDrawable> drawableComponents,
        GraphPlotViewModel? graphPlotViewModel,
        SequencePlotViewModel? sequencePlotViewModel
    )
    {
        _graphPlotViewModel = graphPlotViewModel;
        _sequencePlotViewModel = sequencePlotViewModel;
        TargetFps = appSettings.Simulation.TargetFps;
        TargetTps = appSettings.Simulation.TargetTps;

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

    public async Task RunSimulationJobAsync(
        SimulationJob simulationJob,
        CancellationToken stoppingToken
    )
    {
        var algorithm = simulationJob.Algorithm;
        algorithm.EventHandler = new EventHandler()
        {
            Consumers = [algorithm, _graphPlotViewModel, _sequencePlotViewModel],
        };
        algorithm.Initialize();

        _graphPlotViewModel?.InitializeGraph(simulationJob.Simulation.Graph!);
        _sequencePlotViewModel?.InitializeGraph(simulationJob.Simulation.Graph!);

        _stopwatch = Stopwatch.StartNew();

        _currentTick = 0;

        double updateInterval;
        double renderInterval;
        var statUpdate = 1000.0;

        double nextUpdate = _stopwatch.ElapsedMilliseconds;
        double nextRender = _stopwatch.ElapsedMilliseconds;
        double nextStatUpdate = _stopwatch.ElapsedMilliseconds + statUpdate;

        if (MainWindowViewModel != null)
        {
            MainWindowViewModel.SimulationState = "Running";
        }

        while (!algorithm.IsFinished && !stoppingToken.IsCancellationRequested)
        {
            updateInterval = 1000.0 / TargetTps;
            renderInterval = 1000.0 / TargetFps;

            double currentTime = _stopwatch.ElapsedMilliseconds;

            if (currentTime >= nextUpdate)
            {
                algorithm.Update(++_currentTick);
                Update(_currentTick);
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

            if (updateInterval != 0)
                await Task.Delay(
                    (int)(Math.Min(updateInterval, renderInterval) / 4.0),
                    stoppingToken
                );
        }

        if (MainWindowViewModel != null)
            MainWindowViewModel.SimulationState = "Stopped";

        Log.Information("Simulation engine has stopped after {ticks} ticks.", _currentTick);

        simulationJob.Simulation.NodeEvents = algorithm.EventHistory;
    }

    private void Update(long currentTick)
    {
        lock (UpdateLock)
        {
            if (MainWindowViewModel != null)
                MainWindowViewModel.CurrentTick = currentTick;

            _sequencePlotViewModel?.SequencePlotView.Update(currentTick);
            _tickCounter++;
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
        }
    }
}
