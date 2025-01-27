using System.Collections;
using System.Diagnostics;
using Gtk;
using Serilog;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using TDT4900_MasterThesis.engine;
using TDT4900_MasterThesis.model.simulation;
using TDT4900_MasterThesis.view;

namespace TDT4900_MasterThesis.simulation;

public class SimulationEngine
{
    private readonly MainCanvas _mainCanvas;

    private long _currentTick;

    private int _frameCounter = 0;
    private int _fps = 0;

    private int _tickCounter = 0;
    private int _tps = 0;

    public readonly List<IUpdatable> UpdatableComponents = [];
    public readonly List<IDrawable> DrawableComponents = [];

    public SimulationEngine(MainCanvas mainCanvas, AppSettings appSettings)
    {
        _mainCanvas = mainCanvas;
        _mainCanvas.PaintSurface += OnPaintSurface;

        _targetFps = appSettings.Simulation.TargetFps;
        _targetTps = appSettings.Simulation.TargetTps;
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

    public void RunSimulation()
    {
        _isRunning = true;
        var stopwatch = Stopwatch.StartNew();

        var updateInterval = 1000.0 / _targetTps;
        var renderInterval = 1000.0 / _targetFps;
        var statUpdate = 1000.0;

        double nextUpdate = stopwatch.ElapsedMilliseconds;
        double nextRender = stopwatch.ElapsedMilliseconds;
        double nextStatUpdate = stopwatch.ElapsedMilliseconds + statUpdate;

        while (_isRunning)
        {
            double currentTime = stopwatch.ElapsedMilliseconds;

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

                Log.Debug("Current Tick: {tick}, TPS: {TPS}, FPS: {FPS}", _currentTick, _tps, _fps);

                _tickCounter = _frameCounter = 0;
                nextStatUpdate += statUpdate;
            }

            Thread.Sleep((int)(Math.Min(updateInterval, renderInterval) / 4.0));
        }
    }

    private void Update(long currentTick)
    {
        _tickCounter++;
        UpdatableComponents.ForEach(c => c.Update(currentTick));
    }

    private void Render()
    {
        Application.Invoke(
            delegate
            {
                _mainCanvas.QueueDraw();
            }
        );
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        _frameCounter++;

        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        canvas.DrawText(
            $"Tick: {_currentTick}, TPS: {_tps}, FPS: {_fps}",
            10,
            20,
            new SKPaint { Color = SKColors.Black }
        );

        DrawableComponents.ForEach(c => c.Draw(canvas));
    }
}
