using System.Diagnostics;
using Avalonia.Threading;
using Serilog;
using TDT4900_MasterThesis.Model;
using TDT4900_MasterThesis.Model.Graph;
using TDT4900_MasterThesis.ViewModel;
using EventHandler = TDT4900_MasterThesis.Handler.EventHandler;

namespace TDT4900_MasterThesis.Engine;

public class SimulationEngine(
    AppSettings appSettings,
    IEnumerable<IDrawable> drawableComponents,
    SimulationStatsViewModel? simulationStatsViewModel,
    GraphPlotViewModel? graphPlotViewModel,
    SequencePlotViewModel? sequencePlotViewModel
)
{
    private long _currentTick;

    private int _frameCounter = 0;
    private int _fps = 0;

    private int _tickCounter = 0;
    private int _tps = 0;

    private readonly Stopwatch _stopwatch = new();
    private double _nextStatUpdate = 0;
    private double _nextRender = 0;

    private readonly List<IDrawable> _drawableComponents = drawableComponents.ToList();

    /// <summary>
    /// Target Ticks Per Second
    /// </summary>
    public int TargetTps = appSettings.Simulation.TargetTps;

    /// <summary>
    /// Target Frames Per Second
    /// </summary>
    public int TargetFps = appSettings.Simulation.TargetFps;

    private bool _isRunning = false;

    public async Task RunSimulationJobAsync(
        SimulationJob simulationJob,
        CancellationToken stoppingToken
    )
    {
        var algorithm = simulationJob.Algorithm;
        algorithm.EventHandler = new EventHandler()
        {
            Consumers = [graphPlotViewModel, sequencePlotViewModel, algorithm],
        };
        algorithm.Initialize();

        if (graphPlotViewModel != null)
        {
            graphPlotViewModel.Graph = simulationJob.Simulation.Graph!;
            graphPlotViewModel.StartNode = simulationJob.Simulation.StartNode!;
            graphPlotViewModel.TargetNode = simulationJob.Simulation.TargetNode!;
        }
        sequencePlotViewModel?.InitializeGraph(simulationJob.Simulation.Graph!);

        _stopwatch.Start();

        _currentTick = 0;

        double updateInterval;
        double renderInterval;
        const double statUpdate = 1000.0;

        double nextUpdate = _stopwatch.ElapsedMilliseconds;

        if (simulationStatsViewModel != null)
            simulationStatsViewModel.SimulationState = "Running";

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
                Render(force: true);
                _nextRender += renderInterval;
            }

            if (currentTime >= _nextStatUpdate)
            {
                _fps = (int)(_frameCounter / (statUpdate / 1000.0));
                _tps = (int)(_tickCounter / (statUpdate / 1000.0));

                if (simulationStatsViewModel != null)
                {
                    simulationStatsViewModel.Tps = _tps;
                    simulationStatsViewModel.Fps = _fps;
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

        Render(force: true);

        Log.Information("Simulation engine has stopped after {ticks} ticks.", _currentTick);
    }

    private void Update(long currentTick)
    {
        if (simulationStatsViewModel != null)
            simulationStatsViewModel.CurrentTick = currentTick;

        sequencePlotViewModel?.SequencePlotView.Update(currentTick);
        _tickCounter++;
    }

    private void Render(bool force = false)
    {
        var readyToRender = _drawableComponents.All(c => c.IsReadyToDraw);

        // Not ready to render yet? Skip this frame
        if (!readyToRender && !force)
            return;

        _drawableComponents.ForEach(c =>
        {
            Dispatcher.UIThread.Invoke(c.Draw, DispatcherPriority.Render);
        });
        _frameCounter++;
    }

    public void Pause()
    {
        if (simulationStatsViewModel != null)
            simulationStatsViewModel.SimulationState = "Paused";

        _stopwatch.Stop();
    }

    public void Resume()
    {
        if (simulationStatsViewModel != null)
            simulationStatsViewModel.SimulationState = "Running";

        _stopwatch.Start();
    }

    public void Reset()
    {
        _currentTick = 0;
        Resume();
    }
}
