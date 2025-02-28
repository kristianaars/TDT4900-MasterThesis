using Avalonia;
using Avalonia.Media;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Hatches;
using ScottPlot.Plottables;
using Serilog;
using TDT4900_MasterThesis.constants;
using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;
using Color = ScottPlot.Color;

namespace TDT4900_MasterThesis.view.plot;

public class GraphPlotView : AvaPlot, IDrawable
{
    private List<LinePlot> _edges = new();
    private List<Ellipse> _nodes = new();

    private readonly Lock _stateHistoryQueueLock = new();
    private Queue<NodeStateUpdate> _unprocessedStateHistoryQueue = new();
    private Queue<NodeStateUpdate> _drawBuffer = new();

    public bool IsReadyToDraw
    {
        get => _drawBuffer.Count == 0;
    }

    public GraphPlotView()
    {
        Plot.Grid.IsVisible = false;
        Plot.Axes.Left.TickLabelStyle.IsVisible = false;
        Margin = new Thickness(0);

        SizeChanged += (sender, args) => MaintainAspectRatio();
        Loaded += (sender, args) => MaintainAspectRatio();
        MaintainAspectRatio();
    }

    public void AppendStateHistory(NodeStateUpdate update)
    {
        lock (_stateHistoryQueueLock)
        {
            _unprocessedStateHistoryQueue.Enqueue(update);
        }
    }

    public void Init(Graph graph)
    {
        lock (_unprocessedStateHistoryQueue)
        {
            _unprocessedStateHistoryQueue.Clear();
            _drawBuffer.Clear();
        }

        Plot.Clear();

        var edges = graph!.Edges;
        var nodes = graph.Nodes;

        _edges.Clear();
        _nodes.Clear();

        // Draw edges
        foreach (var e in edges)
        {
            var start = e.Source.Id;
            var end = e.Target.Id;

            double x1 = nodes[start].X,
                y1 = nodes[start].Y;
            double x2 = nodes[end].X,
                y2 = nodes[end].Y;

            var line = Plot.Add.Line(x1, y1, x2, y2);
            line.LineWidth = 2;
            line.LineColor = PlotColors.DarkGray;
            _edges!.Add(line);
        }

        // Draw nodes
        foreach (var n in nodes)
        {
            var node = Plot.Add.Circle(new Coordinates(n.X, n.Y), 16);

            node.FillColor = GetStateFillColor(n.State);
            node.LineColor = GetStateBorderColor(n.State);
            node.LineWidth = 4;

            var text = Plot.Add.Text($"{n.Id}", n.X, n.Y);
            text.LabelFontSize = 12;
            text.OffsetY = 2;
            text.LabelFontColor = PlotColors.Black;
            text.Alignment = Alignment.MiddleCenter;

            _nodes!.Add(node);
        }

        Plot.Axes.AutoScale();
        MaintainAspectRatio();
    }

    public void Draw()
    {
        lock (_stateHistoryQueueLock)
        {
            if (!IsReadyToDraw)
                return;
            _drawBuffer = new Queue<NodeStateUpdate>(_unprocessedStateHistoryQueue!);
            _unprocessedStateHistoryQueue.Clear();
        }

        InvalidateVisual();
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

    public override void Render(DrawingContext context)
    {
        while (_drawBuffer.Count != 0)
        {
            var update = _drawBuffer.Dequeue();
            var node = _nodes![update.NodeId];

            node.FillColor = GetStateFillColor(update.State);
            node.LineColor = GetStateBorderColor(update.State);

            if (update.IsTagged)
            {
                if (update.State == NodeState.Neutral)
                {
                    node.FillColor = PlotColors.LightBlue;
                }

                node.FillHatch = new Striped();
                node.FillHatchColor = PlotColors.LightBlue.Darken(0.2);

                node.LineWidth = 4;
                node.LineColor = PlotColors.BlueLightBorder;
            }
            else
            {
                node.FillHatch = null;
                node.LineWidth = 2;
            }
        }

        base.Render(context);
    }

    /// <summary>
    /// Resets the historical data of the graph. Initialized when a new graph is sat
    /// </summary>
    public void ResetView()
    {
        _unprocessedStateHistoryQueue = new Queue<NodeStateUpdate>();
        _drawBuffer = new Queue<NodeStateUpdate>();

        MarkAllNodesAsNeutral();
    }

    private void MarkAllNodesAsNeutral()
    {
        var nodeCount = _nodes.Count;
        for (int n = 0; n < nodeCount; n++)
        {
            AppendStateHistory(new NodeStateUpdate(n, NodeState.Neutral, false, 0));
        }
    }
}
