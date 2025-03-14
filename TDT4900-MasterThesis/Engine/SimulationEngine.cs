using System.Diagnostics;
using Avalonia.Threading;
using Serilog;
using TDT4900_MasterThesis.Model;
using TDT4900_MasterThesis.Model.Graph;
using TDT4900_MasterThesis.ViewModel;
using EventHandler = TDT4900_MasterThesis.Handler.EventHandler;

namespace TDT4900_MasterThesis.Engine;

public class SimulationEngine
{
    private readonly GraphPlotViewModel? _graphPlotViewModel;
    private readonly SequencePlotViewModel? _sequencePlotViewModel;
    private readonly SimulationStatsViewModel? _simulationStatsViewModel;

    private long _currentTick;

    private int _frameCounter = 0;
    private int _fps = 0;

    private int _tickCounter = 0;
    private int _tps = 0;

    private Stopwatch _stopwatch;
    private double _nextStatUpdate = 0;
    private double _nextRender = 0;

    public readonly List<IDrawable> DrawableComponents;

    public SimulationEngine(
        AppSettings appSettings,
        IEnumerable<IDrawable> drawableComponents,
        SimulationStatsViewModel? simulationStatsViewModel,
        GraphPlotViewModel? graphPlotViewModel,
        SequencePlotViewModel? sequencePlotViewModel
    )
    {
        _graphPlotViewModel = graphPlotViewModel;
        _sequencePlotViewModel = sequencePlotViewModel;
        _simulationStatsViewModel = simulationStatsViewModel;

        TargetFps = appSettings.Simulation.TargetFps;
        TargetTps = appSettings.Simulation.TargetTps;

        DrawableComponents = drawableComponents.ToList();

        _stopwatch = new Stopwatch();
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

    public async Task RunSimulationJobAsync(
        SimulationJob simulationJob,
        CancellationToken stoppingToken
    )
    {
        var algorithm = simulationJob.Algorithm;
        algorithm.EventHandler = new EventHandler()
        {
            Consumers = [_graphPlotViewModel, _sequencePlotViewModel],
        };
        algorithm.Initialize();

        _graphPlotViewModel?.InitializeGraph(simulationJob.Simulation.Graph!);
        _sequencePlotViewModel?.InitializeGraph(simulationJob.Simulation.Graph!);

        _stopwatch.Start();

        _currentTick = 0;

        double updateInterval;
        double renderInterval;
        var statUpdate = 1000.0;

        double nextUpdate = _stopwatch.ElapsedMilliseconds;

        if (_simulationStatsViewModel != null)
            _simulationStatsViewModel.SimulationState = "Running";

        while (!algorithm.IsFinished && !stoppingToken.IsCancellationRequested)
        {
            updateInterval = TargetTps != 0 ? 1000.0 / TargetTps : 0;
            renderInterval = 1000.0 / TargetFps;

            double currentTime = _stopwatch.ElapsedMilliseconds;

            if (currentTime >= nextUpdate)
            {
                algorithm.Update(++_currentTick);
                Update(_currentTick);
                nextUpdate += updateInterval;
            }

            if (currentTime >= _nextRender)
            {
                Render();
                _nextRender += renderInterval;
            }

            if (currentTime >= _nextStatUpdate)
            {
                _fps = (int)(_frameCounter / (statUpdate / 1000.0));
                _tps = (int)(_tickCounter / (statUpdate / 1000.0));

                if (_simulationStatsViewModel != null)
                {
                    _simulationStatsViewModel.Tps = _tps;
                    _simulationStatsViewModel.Fps = _fps;
                }

                _tickCounter = _frameCounter = 0;
                _nextStatUpdate += statUpdate;
            }

            if (updateInterval != 0)
            {
                try
                {
                    await Task.Delay(
                        (int)(Math.Min(updateInterval, renderInterval) / 4.0),
                        stoppingToken
                    );
                }
                catch (OperationCanceledException) { }
            }

            await Task.Yield();
        }

        if (_simulationStatsViewModel != null)
            _simulationStatsViewModel.SimulationState = "Stopped";

        Log.Information("Simulation engine has stopped after {ticks} ticks.", _currentTick);
    }

    private void Update(long currentTick)
    {
        if (_simulationStatsViewModel != null)
            _simulationStatsViewModel.CurrentTick = currentTick;

        _sequencePlotViewModel?.SequencePlotView.Update(currentTick);
        _tickCounter++;
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
        if (_simulationStatsViewModel != null)
            _simulationStatsViewModel.SimulationState = "Paused";

        _stopwatch.Stop();
    }

    public void Resume()
    {
        if (_simulationStatsViewModel != null)
            _simulationStatsViewModel.SimulationState = "Running";

        _stopwatch.Start();
    }

    public void Reset()
    {
        _currentTick = 0;
        Resume();
    }
}
