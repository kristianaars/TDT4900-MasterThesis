using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Hatches;
using ScottPlot.Plottables;
using TDT4900_MasterThesis.Constants;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Model.Graph;
using Color = ScottPlot.Color;

namespace TDT4900_MasterThesis.View.Plot;

public class GraphPlotView : AvaPlot, IDrawable
{
    private List<LinePlot> _edges = new();
    private List<Ellipse> _nodes = new();

    private readonly Lock _nodeEventsLock = new();
    private readonly Queue<NodeEvent> _unprocessedNodeEventsQueue = new();
    private Queue<NodeEvent> _drawBuffer = new();

    public bool IsReadyToDraw
    {
        get => _drawBuffer.Count == 0;
    }

    public bool EnableDataUpdate { get; set; }

    public GraphPlotView()
    {
        Plot.Grid.IsVisible = false;
        Plot.Axes.Left.TickLabelStyle.IsVisible = false;
        Margin = new Thickness(0);
        EnableDataUpdate = true;

        SizeChanged += (sender, args) => MaintainAspectRatio();
        Loaded += (sender, args) => MaintainAspectRatio();
        MaintainAspectRatio();
    }

    public void AppendNodeEvent(NodeEvent nodeEvent)
    {
        if (!EnableDataUpdate)
            return;

        lock (_nodeEventsLock)
        {
            _unprocessedNodeEventsQueue.Enqueue(nodeEvent);
        }
    }

    public void InitializeGraph(Graph graph)
    {
        if (!EnableDataUpdate)
            return;

        lock (_unprocessedNodeEventsQueue)
        {
            _unprocessedNodeEventsQueue.Clear();
            _drawBuffer.Clear();
        }

        Plot.Clear();

        var edges = graph.Edges;
        var nodes = graph.Nodes;

        _edges.Clear();
        _nodes.Clear();

        // Draw edges
        foreach (var e in edges)
        {
            var start = e.Source.NodeId;
            var end = e.Target.NodeId;

            double x1 = nodes[start].X,
                y1 = nodes[start].Y;
            double x2 = nodes[end].X,
                y2 = nodes[end].Y;

            var line = Plot.Add.Line(x1, y1, x2, y2);
            line.LineWidth = 2;
            line.LineColor = PlotColors.DarkGray;
            _edges.Add(line);
        }

        // Draw nodes
        foreach (var n in nodes)
        {
            var node = Plot.Add.Circle(new Coordinates(n.X, n.Y), 16);

            node.FillColor = GetStateFillColor(EventType.Neutral);
            node.LineColor = GetStateBorderColor(EventType.Neutral);
            node.LineWidth = 4;

            var text = Plot.Add.Text($"{n.NodeId}", n.X, n.Y);
            text.LabelFontSize = 12;
            text.OffsetY = 2;
            text.LabelFontColor = PlotColors.Black;
            text.Alignment = Alignment.MiddleCenter;

            _nodes!.Add(node);
        }

        Plot.Axes.AutoScale();
        MaintainAspectRatio();

        if (IsReadyToDraw)
            Refresh();
    }

    /// <summary>
    /// Draw fills the draw buffer with the unprocessed node events and invalidates the visual to trigger <see cref="Render"/>
    /// </summary>
    public void Draw()
    {
        if (!IsReadyToDraw)
            return;

        lock (_nodeEventsLock)
        {
            _drawBuffer = new Queue<NodeEvent>(_unprocessedNodeEventsQueue!);
            _unprocessedNodeEventsQueue.Clear();
        }

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        if (!EnableDataUpdate)
            return;

        while (_drawBuffer.Count != 0)
        {
            var update = _drawBuffer.Dequeue();
            var nodeComp = _nodes[update.NodeId];
            var eventType = update.EventType!;

            if (eventType == EventType.Tagged)
            {
                nodeComp.FillHatch = new Striped();
                nodeComp.FillHatchColor = PlotColors.LightBlue.Darken(0.2);
                nodeComp.LineWidth = 4;
            }
            else
            {
                nodeComp.FillColor = GetStateFillColor(eventType);
                nodeComp.LineColor = GetStateBorderColor(eventType);
            }
        }

        base.Render(context);
    }

    private Color GetStateFillColor(EventType eventType) =>
        eventType switch
        {
            EventType.Neutral => PlotColors.White,
            EventType.Refractory => PlotColors.LightGray,
            EventType.Processing => PlotColors.Green,
            EventType.Inhibited => PlotColors.DarkRed,
            EventType.Tagged => throw new InvalidEnumArgumentException(
                "Tagged does not contain a fill color"
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null),
        };

    private Color GetStateBorderColor(EventType eventType) =>
        eventType switch
        {
            EventType.Neutral => PlotColors.DarkGray,
            EventType.Refractory => PlotColors.DarkGray,
            EventType.Processing => PlotColors.GreenBorder,
            EventType.Inhibited => PlotColors.DarkRedBorder,
            EventType.Tagged => throw new InvalidEnumArgumentException(
                "Tagged does not contain a border color"
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null),
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
