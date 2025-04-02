using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.AxisRules;
using ScottPlot.Plottables;
using TDT4900_MasterThesis.Constants;
using TDT4900_MasterThesis.Helper;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Model.Graph;
using TDT4900_MasterThesis.View.Plot.Generator;
using Color = ScottPlot.Color;
using Colors = ScottPlot.Colors;

namespace TDT4900_MasterThesis.View.Plot;

public class SequencePlotView : AvaPlot, IDrawable, IUpdatable
{
    public bool EnableAutoScroll { get; set; } = false;

    public bool EnableDataUpdate { get; set; } = false;

    private readonly Lock _nodeEventsLock = new();

    private readonly int _tickWindow = 80;
    private readonly float _tickOffsetRightRatio = 0.05f;
    private int TickOffsetRight => (int)(_tickWindow * _tickOffsetRightRatio);

    private BarPlot[] _bars;
    private bool[] _isNeutral;
    private bool[] _nodeTagged;
    private long _latestTick;

    /// <summary>
    /// Vertical size of the sequence state bars
    /// </summary>
    private const float BarSize = 0.5f;

    public SequencePlotView()
    {
        Plot.Grid.IsBeneathPlottables = false;
        Plot.Grid.MajorLineColor = PlotColors.DarkGray.WithAlpha(0.5);

        Plot.Grid.YAxisStyle.MajorLineStyle.IsVisible = false;

        Plot.Axes.Bottom.TickGenerator = new IntegerTickGenerator();

        Plot.Axes.Rules.Add(
            new LockedVertical(Plot.Axes.Left, Plot.Axes.Left.Min, Plot.Axes.Left.Max)
        );

        Margin = new Thickness(0);

        _bars = [];
        _isNeutral = [];
        _nodeTagged = [];
    }

    public void Draw()
    {
        if (!EnableDataUpdate)
            return;

        lock (Plot.Sync)
        {
            Refresh();
        }
    }

    public bool IsReadyToDraw => true;

    public void AppendAlgorithmEvent(AlgorithmEvent algEvent)
    {
        if (!EnableDataUpdate)
            return;

        lock (Plot.Sync)
        {
            switch (algEvent)
            {
                case EdgeEvent edgeEvent:
                    ConsumeEdgeEvent(edgeEvent);
                    break;
                case NodeEvent nodeEvent:
                    ConsumeNodeEvent(nodeEvent);
                    break;
            }
        }
    }

    private void ConsumeEdgeEvent(EdgeEvent edgeEvent)
    {
        if (edgeEvent.EventType == EdgeEventType.Active)
        {
            PlotNodeMessage(
                edgeEvent.SourceId,
                edgeEvent.TargetId,
                edgeEvent.Tick,
                edgeEvent.ReceiveAt
            );
        }
    }

    private void ConsumeNodeEvent(NodeEvent nodeEvent)
    {
        var tick = (long)nodeEvent.Tick!;
        var nodeId = nodeEvent.NodeId;
        var eventType = nodeEvent.EventType;

        // If there exists a bar for the node, update it up until the update tick
        if (!_isNeutral[nodeId])
        {
            UpdateLastBarValue(nodeId, tick);
        }

        // Update neutral state for node
        _isNeutral[nodeId] = eventType == NodeEventType.Neutral;

        // If the node is not neutral anymore, initialize a new bar
        if (!_isNeutral[nodeId] && eventType != NodeEventType.Tagged)
            PlotNewBar(nodeId, eventType, tick, _nodeTagged[nodeId]);

        // Visualize the moment a node is tagged
        if (eventType == NodeEventType.Tagged && !_nodeTagged[nodeId])
        {
            MarkNodeAsTagged(tick, nodeId);
            _nodeTagged[nodeId] = true;
        }
    }

    private void PlotNewBar(int nodeId, NodeEventType eventType, long atTick, bool isTagged)
    {
        _bars[nodeId]
            .Bars.Add(
                new Bar()
                {
                    Size = BarSize,
                    Position = nodeId,
                    FillColor = GetStateFillColor(eventType),
                    LineWidth = 0,
                    Orientation = Orientation.Horizontal,
                    ValueBase = atTick,
                    Value = atTick,
                    FillHatch = isTagged ? new ScottPlot.Hatches.Striped() : null,
                    FillHatchColor = GetStateFillColor(eventType).Darken(0.2),
                }
            );
    }

    private void PlotNodeMessage(int source, int target, long sentAt, long receiveAt)
    {
        var isExcitatory = true;

        var y1 = _bars[source].Bars[^1].Rect.VerticalCenter;
        var y2 = _bars[target].Bars[^1].Rect.VerticalCenter;

        var x1 = sentAt;
        var x2 = receiveAt;

        var line = Plot.Add.Line(x1, y1, x2, y2);
        var lineColor = isExcitatory ? PlotColors.GreenBorder : PlotColors.DarkRedBorder;
        var endLineColor = isExcitatory ? PlotColors.GreenBorder : PlotColors.DarkRedBorder;
        var startLineColor = isExcitatory ? Colors.Transparent : PlotColors.DarkRedBorder;

        line.LinePattern = isExcitatory ? LinePattern.Solid : LinePattern.Dashed;

        line.LineWidth = 1;
        line.LineColor = lineColor.WithAlpha(0.5f);

        var verticalStartLine = Plot.Add.Line(x1, y1 - BarSize / 2.0f, x1, y1 + BarSize / 2.0f);
        verticalStartLine.LineWidth = 1;
        verticalStartLine.LineColor = startLineColor;

        var verticaEndLine = Plot.Add.Line(x2, y2 - BarSize / 1.5f, x2, y2 + BarSize / 1.5f);
        verticaEndLine.LineWidth = 1;
        verticaEndLine.LineColor = endLineColor;
    }

    private void MarkNodeAsTagged(long atTick, int nodeId)
    {
        var yCenter = _bars[nodeId].Bars[^1].Rect.VerticalCenter;

        var verticalLine = Plot.Add.Line(
            atTick,
            yCenter - BarSize / 1.5f,
            atTick,
            yCenter + BarSize / 1.5f
        );
        verticalLine.LineWidth = 6;
        verticalLine.LineColor = PlotColors.BlueLightBorder;
    }

    public override void Render(DrawingContext context)
    {
        lock (Plot.Sync)
        {
            if (EnableAutoScroll)
            {
                Plot.Axes.SetLimitsX(
                    _latestTick - (_tickWindow - TickOffsetRight),
                    _latestTick + TickOffsetRight
                );
            }
        }

        base.Render(context);
    }

    private void UpdateLastBarValue(int nodeId, long toValue)
    {
        _bars[nodeId].Bars[^1].Value = toValue;
    }

    public void Update(long currentTick)
    {
        if (!EnableDataUpdate)
            return;

        _latestTick = currentTick;
    }

    public void InitializeGraph(Graph g)
    {
        lock (Plot.Sync)
        {
            Plot.Clear();

            // Add bars to plot
            List<(string name, double[] edges)> ranges = g
                .Nodes.Select(n =>
                {
                    return ($"Node {n.NodeId}", new double[] { 0, 0 });
                })
                .ToList();
            _bars = Plot.Add.StackedRanges(ranges, horizontal: true);

            _isNeutral = new bool[g.Nodes.Count];
            _nodeTagged = new bool[g.Nodes.Count];

            ArrayHelper.FillArray(_isNeutral, true);
            ArrayHelper.FillArray(_nodeTagged, false);

            Plot.Axes.AutoScale();

            Plot.Axes.Rules.Clear();
            Plot.Axes.Rules.Add(
                new LockedVertical(Plot.Axes.Left, Plot.Axes.Left.Min, Plot.Axes.Left.Max)
            );
        }
    }

    private Color GetStateFillColor(NodeEventType eventType) =>
        eventType switch
        {
            NodeEventType.Neutral => Colors.Transparent,
            NodeEventType.Refractory => PlotColors.LightGray,
            NodeEventType.Processing => PlotColors.Green,
            NodeEventType.Inhibited => PlotColors.DarkRed,
            NodeEventType.Tagged => throw new InvalidEnumArgumentException(
                "Tagged does not contain a fill color"
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null),
        };
}
