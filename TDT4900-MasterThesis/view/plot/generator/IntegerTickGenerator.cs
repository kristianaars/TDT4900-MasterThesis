using ScottPlot;
using SkiaSharp;

namespace TDT4900_MasterThesis.view.plot.generator;

public class IntegerTickGenerator : ITickGenerator
{
    public void Regenerate(
        CoordinateRange range,
        Edge edge,
        PixelLength size,
        SKPaint paint,
        LabelStyle labelStyle
    )
    {
        var ticks = new List<Tick>();

        var min = (int)Math.Floor(range.Min);
        var max = (int)Math.Ceiling(range.Max);
        var spacing = (int)Math.Max(Math.Floor(range.Length / size.Length * 40), 1);

        for (int i = min; i <= max; i += spacing)
        {
            var tick = new Tick(i, i.ToString());
            ticks.Add(tick);
        }

        Ticks = ticks.ToArray();
    }

    public Tick[] Ticks { get; set; }
    public int MaxTickCount { get; set; }
}
