using Avalonia;
using Avalonia.Media;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottables;
using TDT4900_MasterThesis.Constants;
using TDT4900_MasterThesis.Helper;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Model.Graph;
using Color = ScottPlot.Color;

namespace TDT4900_MasterThesis.View.Plot;

public class GraphPlotView : AvaPlot, IDrawable
{
    private Graph _graph;
    private NodeState[] _nodeStates;
    private EdgeEventType[,] _edgeStates;
    private bool[] _nodeTags;

    private Node? _startNode;
    private Node? _targetNode;

    public bool IsReadyToDraw { get; set; } = true;

    public GraphPlotView()
    {
        Plot.Grid.IsVisible = false;
        Plot.Axes.Left.TickLabelStyle.IsVisible = false;
        Margin = new Thickness(0);

        /*SizeChanged += (sender, args) => MaintainAspectRatio();
        Loaded += (sender, args) => MaintainAspectRatio();
        MaintainAspectRatio();*/
    }

    public void InitializeGraph(Graph graph)
    {
        _graph = graph;

        _nodeStates = new NodeState[graph.Nodes.Count];
        ArrayHelper.FillArray(_nodeStates, NodeState.Neutral);

        _nodeTags = new bool[graph.Nodes.Count];
        ArrayHelper.FillArray(_nodeTags, false);

        _edgeStates = new EdgeEventType[graph.Nodes.Count, graph.Nodes.Count];
        ArrayHelper.FillArray(_edgeStates, EdgeEventType.Neutral);

        Plot.Axes.AutoScale();
        MaintainAspectRatio();

        _startNode = null;
        _targetNode = null;

        Draw();

        Plot.Axes.AutoScale();
    }

    public void SetStartNode(Node? startNode)
    {
        _startNode = startNode;
        Draw();
    }

    public void SetTargetNode(Node? endNode)
    {
        _targetNode = endNode;
        Draw();
    }

    public void AppendAlgorithmEvent(AlgorithmEvent algEvent)
    {
        switch (algEvent)
        {
            case NodeEvent nodeEvent:
                switch (nodeEvent.EventType)
                {
                    case NodeEventType.Tagged:
                        _nodeTags[nodeEvent.NodeId] = true;
                        break;
                    case NodeEventType.Neutral:
                        _nodeStates[nodeEvent.NodeId] = NodeState.Neutral;
                        break;
                    case NodeEventType.Refractory:
                        _nodeStates[nodeEvent.NodeId] = NodeState.Refractory;
                        break;
                    case NodeEventType.Processing:
                        _nodeStates[nodeEvent.NodeId] = NodeState.Processing;
                        break;
                    case NodeEventType.Inhibited:
                        _nodeStates[nodeEvent.NodeId] = NodeState.Inhibited;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                break;
            case EdgeEvent edgeEvent:
                var i = edgeEvent.SourceId;
                var j = edgeEvent.TargetId;

                _edgeStates[i, j] = edgeEvent.EventType;
                _edgeStates[j, i] = edgeEvent.EventType;
                break;
        }
    }

    public void Draw()
    {
        if (!IsReadyToDraw)
            return;

        lock (Plot.Sync)
        {
            IsReadyToDraw = false;
            Plot.Clear();

            var edges = _graph.Edges;
            var nodes = _graph.Nodes;

            // Draw edges
            foreach (var e in edges)
            {
                var start = e.SourceNodeId;
                var end = e.TargetNodeId;

                double x1 = nodes[start].X,
                    y1 = nodes[start].Y;
                double x2 = nodes[end].X,
                    y2 = nodes[end].Y;

                var line = AddBezier(
                    (x1, y1),
                    (x2, y2),
                    (e.Level == 0 ? 0 : Math.Pow(-1, e.Level)) * 1
                );

                var alpha = e.Level == 0 ? 0.8f : 0.5f / e.Level;
                line.LineColor = (
                    _edgeStates[start, end] switch
                    {
                        EdgeEventType.Excitatory => PlotColors.GreenBorder,
                        EdgeEventType.Inhibitory => PlotColors.DarkRed,
                        _ => PlotColors.Blue,
                    }
                ).WithAlpha(alpha);

                line.LineWidth = 1;
            }

            // Draw start node and target-node
            if (_startNode != null)
            {
                Plot.Add.Marker(
                    _startNode.X,
                    _startNode.Y,
                    shape: MarkerShape.FilledCircle,
                    color: PlotColors.Green,
                    size: 15f
                );
            }

            if (_targetNode != null)
            {
                Plot.Add.Marker(
                    _targetNode.X,
                    _targetNode.Y,
                    shape: MarkerShape.FilledCircle,
                    color: PlotColors.Pink,
                    size: 15f
                );
            }

            // Draw nodes
            nodes.ForEach(n =>
                Plot.Add.Marker(
                    n.X,
                    n.Y,
                    shape: _nodeTags[n.NodeId]
                        ? MarkerShape.FilledDiamond
                        : MarkerShape.FilledCircle,
                    color: GetStateFillColor(_nodeStates[n.NodeId], _nodeTags[n.NodeId]),
                    size: 10f
                )
            );

            /*var text = Plot.Add.Text($"{n.NodeId}", n.X, n.Y);
            text.LabelFontSize = 8;
            text.OffsetY = 2;
            text.LabelFontColor = PlotColors.DarkGray;
            text.Alignment = Alignment.MiddleCenter;*/

            Refresh();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        IsReadyToDraw = true;
    }

    private Scatter AddBezier((double x, double y) start, (double x, double y) end, double height)
    {
        (double x, double y) control = ((start.x + end.x) / 2, start.y + height);

        List<double> xs = new();
        List<double> ys = new();

        for (double t = 0; t <= 1; t += 0.05)
        {
            double x = (1 - t) * (1 - t) * start.x + 2 * (1 - t) * t * control.x + t * t * end.x;
            double y = (1 - t) * (1 - t) * start.y + 2 * (1 - t) * t * control.y + t * t * end.y;
            xs.Add(x);
            ys.Add(y);
        }

        xs.Add(end.x);
        ys.Add(end.y);

        var scatter = Plot.Add.Scatter(xs.ToArray(), ys.ToArray(), PlotColors.Blue);
        scatter.MarkerShape = MarkerShape.None;

        return scatter;
    }

    private Color GetStateFillColor(NodeState state, bool tagged) =>
        state switch
        {
            NodeState.Neutral => tagged ? PlotColors.Blue : PlotColors.Black,
            NodeState.Refractory => PlotColors.DarkGray,
            NodeState.Processing => PlotColors.GreenBorder,
            NodeState.Inhibited => PlotColors.DarkRed,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
        };

    private void MaintainAspectRatio()
    {
        var plot = Plot;
        double width = Bounds.Width;
        double height = Bounds.Height;

        if (width == 0 || height == 0)
            return; // Prevent division by zero

        // Preserve square aspect ratio
        double aspectRatio = width / height;

        double xCenter = (plot.Axes.Bottom.Max + plot.Axes.Bottom.Min) / 2;
        double yCenter = (plot.Axes.Left.Max + plot.Axes.Left.Min) / 2;

        double xSpan = plot.Axes.Bottom.Max - plot.Axes.Bottom.Min;
        double ySpan = plot.Axes.Left.Max - plot.Axes.Left.Min;

        double newXSpan,
            newYSpan;

        if (aspectRatio > 1) // Wide window
        {
            newXSpan = ySpan * aspectRatio;
            newYSpan = ySpan;
        }
        else // Tall window
        {
            newXSpan = xSpan;
            newYSpan = xSpan / aspectRatio;
        }

        plot.Axes.Bottom.Min = xCenter - newXSpan / 2;
        plot.Axes.Bottom.Max = xCenter + newXSpan / 2;
        plot.Axes.Left.Min = yCenter - newYSpan / 2;
        plot.Axes.Left.Max = yCenter + newYSpan / 2;
    }
}
