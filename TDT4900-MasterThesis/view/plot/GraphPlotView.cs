using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottables;
using Serilog;
using TDT4900_MasterThesis.constants;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;

namespace TDT4900_MasterThesis.view.plot;

public class GraphPlotView : AvaPlot, IDrawable, IUpdatable
{
    private Graph _graph;

    public GraphPlotView(Graph graph, AppSettings appSettings)
    {
        _graph = graph;
        Init();
    }

    private List<Arrow> _edges = new();
    private List<Ellipse> _nodes = new();

    public void Init()
    {
        var edges = _graph.Edges;
        var nodes = _graph.Nodes;

        // Draw edges
        foreach (var e in edges)
        {
            int start = e.Source.Id;
            int end = e.Target.Id;

            double x1 = nodes[start].X,
                y1 = nodes[start].Y;
            double x2 = nodes[end].X,
                y2 = nodes[end].Y;

            var line = Plot.Add.Arrow(x1, y1, x2, y2);
            line.ArrowFillColor = Colors.Black;
            line.ArrowLineColor = Colors.Transparent;
            line.ArrowWidth = 2f;
            line.ArrowheadWidth = 15f;
            line.ArrowheadAxisLength = 10f;
            _edges.Add(line);
        }

        // Draw nodes
        foreach (var n in nodes)
        {
            var node = Plot.Add.Circle(new Coordinates(n.X, n.Y), 10);

            node.FillColor = GetStateFillColor(n.State);
            node.LineColor = GetStateBorderColor(n.State);
            node.LineWidth = 2;

            var text = Plot.Add.Text($"{n.Id}", n.X, n.Y);
            text.Alignment = Alignment.MiddleCenter;
            text.LabelFontColor = PlotColors.Black;

            _nodes.Add(node);
        }

        Plot.Grid.IsVisible = false;
        Plot.Axes.Left.TickLabelStyle.IsVisible = false;
        Plot.Axes.Bottom.TickLabelStyle.IsVisible = false;
    }

    public void Draw()
    {
        Refresh();
    }

    public void Update(long currentTick)
    {
        foreach (var n in _graph.Nodes)
        {
            var node = _nodes[n.Id];

            node.FillColor = GetStateFillColor(n.State);
            node.LineColor = GetStateBorderColor(n.State);

            if (n.IsTagged)
            {
                if (n.State == NodeState.Neutral)
                {
                    node.FillColor = PlotColors.LightBlue;
                }
                node.LineWidth = 4;
                node.LineColor = PlotColors.BlueLightBorder;
            }
        }
    }

    private Color GetStateFillColor(NodeState state) =>
        state switch
        {
            NodeState.Neutral => PlotColors.White,
            NodeState.Refractory => PlotColors.LightGray,
            NodeState.Processing => PlotColors.Green,
            NodeState.Inhibited => PlotColors.DarkRed,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
        };

    private Color GetStateBorderColor(NodeState state) =>
        state switch
        {
            NodeState.Neutral => PlotColors.DarkGray,
            NodeState.Refractory => PlotColors.DarkGray,
            NodeState.Processing => PlotColors.GreenBorder,
            NodeState.Inhibited => PlotColors.DarkRedBorder,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
        };
}
