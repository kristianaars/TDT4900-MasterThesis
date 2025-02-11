using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottables;
using Serilog;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;

namespace TDT4900_MasterThesis.view.plot;

public class GraphPlotView : AvaPlot, IDrawable, IUpdatable
{
    private Graph _graph;
    private readonly int[] _activeNodes;
    private readonly int _nodeActivationDuration;

    public GraphPlotView(Graph graph, AppSettings appSettings)
    {
        _graph = graph;
        _nodeActivationDuration = appSettings.Simulation.TargetTps / 4;
        _activeNodes = new int[graph.Nodes.Count];

        Init();
    }

    private List<Arrow> _edges = new();
    private List<Marker> _nodes = new();

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
            var node = Plot.Add.Marker(n.X, n.Y);

            node.Color = Colors.Cyan;
            node.MarkerSize = 40;

            var text = Plot.Add.Text($"{n.Id}", n.X, n.Y);
            text.Alignment = Alignment.MiddleCenter;

            _nodes.Add(node);
        }
    }

    public void Draw()
    {
        Refresh();
    }

    public void ActivateNode(int i)
    {
        _nodes[i].Color = Colors.Green;
        _activeNodes[i] = _nodeActivationDuration;
    }

    public void Update(long currentTick)
    {
        foreach (var n in _graph.Nodes)
        {
            switch (n.State)
            {
                case NodeState.Neutral:
                    _nodes[n.Id].Color = Colors.White;
                    break;
                case NodeState.Refractory:
                    _nodes[n.Id].Color = Colors.Cyan;
                    break;
                case NodeState.Processing:
                    _nodes[n.Id].Color = Colors.Green;
                    break;
                case NodeState.Inhibited:
                    _nodes[n.Id].Color = Colors.Red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (n.IsTagged)
            {
                _nodes[n.Id].LineWidth = 2;
            }
        }
    }
}
