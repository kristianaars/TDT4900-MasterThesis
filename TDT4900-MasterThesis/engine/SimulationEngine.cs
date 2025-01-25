using System.Collections;
using System.Diagnostics;
using Serilog;
using TDT4900_MasterThesis.model.simulation;

namespace TDT4900_MasterThesis.simulation;

public class SimulationEngine
{
    public required List<IUpdatable> UpdatableComponents;
    public required List<IDrawable> DrawableComponents;

    /// <summary>
    /// Target Ticks Per Second
    /// </summary>
    private readonly int _targetTps = AppSettings.TargetTPS;

    /// <summary>
    /// Target Frames Per Second
    /// </summary>
    private readonly int _targetFps = AppSettings.TargetFPS;

    private bool _isRunning = false;

    public void RunSimulation()
    {
        _isRunning = true;
        var stopwatch = Stopwatch.StartNew();

        var currentTick = 0L;

        var updateInterval = 1000.0 / _targetTps;
        var renderInterval = 1000.0 / _targetFps;
        var statUpdate = 1000.0;

        double nextUpdate = stopwatch.ElapsedMilliseconds;
        double nextRender = stopwatch.ElapsedMilliseconds;
        double nextStatUpdate = stopwatch.ElapsedMilliseconds + statUpdate;

        var frameStatCounter = 0;
        var tickStatCounter = 0;

        while (_isRunning)
        {
            double currentTime = stopwatch.ElapsedMilliseconds;

            if (currentTime >= nextUpdate)
            {
                Update(++currentTick);
                nextUpdate += updateInterval;
                tickStatCounter++;
            }

            if (currentTime >= nextRender)
            {
                Render();
                nextRender += renderInterval;
                frameStatCounter++;
            }

            if (currentTime >= nextStatUpdate)
            {
                Log.Debug(
                    "Current Tick: {tick}, TPS: {TPS}, FPS: {FPS}",
                    currentTick,
                    tickStatCounter / (statUpdate / 1000.0),
                    frameStatCounter / (statUpdate / 1000.0)
                );
                tickStatCounter = frameStatCounter = 0;
                nextStatUpdate += statUpdate;
            }

            Thread.Sleep((int)(Math.Min(updateInterval, renderInterval) / 4.0));
        }
    }

    private void Update(long currentTick)
    {
        UpdatableComponents.ForEach(c => c.Update(currentTick));
    }

    private void Render()
    {
        DrawableComponents.ForEach(c => c.Draw());
    }
}
