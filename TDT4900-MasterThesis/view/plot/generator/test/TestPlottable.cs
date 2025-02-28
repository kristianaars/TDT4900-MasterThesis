using ScottPlot;

namespace TDT4900_MasterThesis.view.plot.generator.test;

public class TestPlottable : IPlottable
{
    public AxisLimits GetAxisLimits()
    {
        throw new NotImplementedException();
    }

    public void Render(RenderPack rp)
    {
        throw new NotImplementedException();
    }

    public bool IsVisible { get; set; }
    public IAxes Axes { get; set; }
    public IEnumerable<LegendItem> LegendItems { get; }
}
