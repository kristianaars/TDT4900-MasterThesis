using System.Diagnostics;
using Avalonia.Threading;
using Serilog;
using TDT4900_MasterThesis.model.simulation;
using TDT4900_MasterThesis.view.plot;

namespace TDT4900_MasterThesis.engine;

public class SimulationEngine
{
    private long _currentTick;

    private int _frameCounter = 0;
    private int _fps = 0;

    private int _tickCounter = 0;
    private int _tps = 0;

    private Stopwatch _stopwatch;

    public readonly List<IUpdatable> UpdatableComponents;
    public readonly List<IDrawable> DrawableComponents;

    public SimulationEngine(
        AppSettings appSettings,
        IEnumerable<IUpdatable> updatableComponents,
        IEnumerable<IDrawable> drawableComponents
    )
    {
        _targetFps = appSettings.Simulation.TargetFps;
        _targetTps = appSettings.Simulation.TargetTps;

        UpdatableComponents = updatableComponents.ToList();
        DrawableComponents = drawableComponents.ToList();
    }

    /// <summary>
    /// Target Ticks Per Second
    /// </summary>
    private readonly int _targetTps;

    /// <summary>
    /// Target Frames Per Second
    /// </summary>
    private readonly int _targetFps;

    private bool _isRunning = false;

    public async Task RunSimulation(CancellationToken stoppingToken)
    {
        _isRunning = true;
        _stopwatch = Stopwatch.StartNew();

        var updateInterval = 1000.0 / _targetTps;
        var renderInterval = 1000.0 / _targetFps;
        var statUpdate = 1000.0;

        double nextUpdate = _stopwatch.ElapsedMilliseconds;
        double nextRender = _stopwatch.ElapsedMilliseconds;
        double nextStatUpdate = _stopwatch.ElapsedMilliseconds + statUpdate;

        while (_isRunning && !stoppingToken.IsCancellationRequested)
        {
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

                Log.Information(
                    "Current Tick: {tick}, TPS: {TPS}, FPS: {FPS}",
                    _currentTick,
                    _tps,
                    _fps
                );

                _tickCounter = _frameCounter = 0;
                nextStatUpdate += statUpdate;
            }

            await Task.Delay((int)(Math.Min(updateInterval, renderInterval) / 4.0));
        }

        Log.Information("Simulation engine has stopped after {ticks} ticks.", _currentTick);
    }

    private void Update(long currentTick)
    {
        _tickCounter++;
        UpdatableComponents.ForEach(c => c.Update(currentTick));
    }

    private void Render()
    {
        DrawableComponents.ForEach(c => Dispatcher.UIThread.Invoke(c.Draw));
        _frameCounter++;
    }

    public void Pause()
    {
        _stopwatch.Stop();
    }

    public void Play()
    {
        _stopwatch.Start();
    }

    public void Restart()
    {
        throw new NotImplementedException();
    }
}
