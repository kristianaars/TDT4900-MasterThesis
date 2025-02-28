using Avalonia;
using Avalonia.Media;
using Avalonia.Remote.Protocol.Input;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.AxisRules;
using ScottPlot.Interactivity.UserActionResponses;
using ScottPlot.Plottables;
using TDT4900_MasterThesis.constants;
using TDT4900_MasterThesis.helpers;
using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;
using TDT4900_MasterThesis.view.plot.generator;
using Color = ScottPlot.Color;
using Colors = ScottPlot.Colors;
using MouseButton = ScottPlot.Interactivity.MouseButton;

namespace TDT4900_MasterThesis.view.plot;

public class SequencePlotView : AvaPlot, IDrawable, IUpdatable
{
    public bool EnableAutoScroll { get; set; } = true;

    public bool EnableDataUpdate { get; set; } = true;

    private readonly Lock _stateHistoryQueueLock = new();
    private readonly Queue<NodeStateUpdate> _unprocessedStateHistoryQueue;
    private Queue<NodeStateUpdate> _drawBufferStateUpdate;

    private readonly Queue<NodeMessage> _unprocessedNodeMessagesQueue;
    private Queue<NodeMessage> _drawBufferMessages;

    private int _tickWindow = 80;
    private float _tickOffsetRightRatio = 0.05f;
    private int TickOffsetRight => (int)(_tickWindow * _tickOffsetRightRatio);

    public bool IsReadyToDraw
    {
        get => _drawBufferStateUpdate.Count + _drawBufferMessages.Count == 0;
    }

    private BarPlot[] _bars;
    private bool[] _isNeutral;
    private bool[] _taggedVisualized;
    private long _latestTick = 0;

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
        _unprocessedStateHistoryQueue = [];
        _drawBufferStateUpdate = [];
        _taggedVisualized = [];

        _unprocessedNodeMessagesQueue = [];
        _drawBufferMessages = [];
    }

    public void AppendStateUpdate(NodeStateUpdate update)
    {
        if (!EnableDataUpdate)
            return;

        lock (_stateHistoryQueueLock)
        {
            _unprocessedStateHistoryQueue.Enqueue(update);
        }
    }

    public void AppendNodeMessage(NodeMessage message)
    {
        if (!EnableDataUpdate)
            return;

        lock (_stateHistoryQueueLock)
        {
            _unprocessedNodeMessagesQueue.Enqueue(message);
        }
    }

    private void PlotNewBar(int nodeId, NodeState state, long atTick, bool isTagged)
    {
        _bars!
            [nodeId]
            .Bars.Add(
                new Bar()
                {
                    Size = BarSize,
                    Position = nodeId,
                    FillColor = GetStateFillColor(state),
                    LineWidth = 0,
                    Orientation = Orientation.Horizontal,
                    ValueBase = atTick,
                    Value = atTick,
                    FillHatch = isTagged ? new ScottPlot.Hatches.Striped() : null,
                    FillHatchColor = GetStateFillColor(state).Darken(0.2),
                }
            );
    }

    private void PlotNodeMessage(NodeMessage message)
    {
        var source = message.Sender;
        var target = message.Receiver;
        var isExcitatory = message.Type == NodeMessage.MessageType.Excitatory;

        if (source == null)
            return;

        var y1 = _bars![source.Id].Bars[^1].Rect.VerticalCenter;
        var y2 = _bars[target.Id].Bars[^1].Rect.VerticalCenter;

        var x1 = message.SentAt;
        var x2 = message.ReceiveAt;

        var line = Plot.Add.Line(x1, y1, x2, y2);
        var lineColor = isExcitatory ? PlotColors.GreenBorder : PlotColors.DarkRedBorder;
        var endLineColor = isExcitatory ? PlotColors.GreenBorder : PlotColors.DarkRedBorder;
        var startLineColor = isExcitatory ? Colors.Transparent : PlotColors.DarkRedBorder;

        line.LinePattern = isExcitatory ? LinePattern.Solid : LinePattern.Dashed;

        line.LineWidth = 2;
        line.LineColor = lineColor;

        var verticalStartLine = Plot.Add.Line(x1, y1 - BarSize / 2.0f, x1, y1 + BarSize / 2.0f);
        verticalStartLine.LineWidth = 2;
        verticalStartLine.LineColor = startLineColor;

        var verticaEndLine = Plot.Add.Line(x2, y2 - BarSize / 1.5f, x2, y2 + BarSize / 1.5f);
        verticaEndLine.LineWidth = 2;
        verticaEndLine.LineColor = endLineColor;
    }

    public void MarkNodeAsTagged(long atTick, int nodeId)
    {
        var yCenter = _bars![nodeId].Bars[^1].Rect.VerticalCenter;

        var verticalLine = Plot.Add.Line(
            atTick,
            yCenter - BarSize / 1.5f,
            atTick,
            yCenter + BarSize / 1.5f
        );
        verticalLine.LineWidth = 6;
        verticalLine.LineColor = PlotColors.BlueLightBorder;
    }

    public void Draw()
    {
        lock (_stateHistoryQueueLock)
        {
            if (!IsReadyToDraw)
                return;

            _drawBufferStateUpdate = new Queue<NodeStateUpdate>(_unprocessedStateHistoryQueue!);
            _unprocessedStateHistoryQueue.Clear();

            _drawBufferMessages = new Queue<NodeMessage>(_unprocessedNodeMessagesQueue);
            _unprocessedNodeMessagesQueue.Clear();
        }

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        while (_drawBufferStateUpdate.Count != 0)
        {
            var update = _drawBufferStateUpdate.Dequeue();
            var nodeId = update.NodeId;
            var updateTick = update.Tick;

            // If there exists a bar for the node, update it up until the update tick
            if (!_isNeutral[update.NodeId])
            {
                UpdateLastBarValue(nodeId, updateTick);
            }

            _isNeutral[update.NodeId] = update.State == NodeState.Neutral;

            if (!_isNeutral[update.NodeId])
                PlotNewBar(update.NodeId, update.State, update.Tick, update.IsTagged);

            // Visualize the moment a node is tagged
            if (update.IsTagged && !_taggedVisualized[update.NodeId])
            {
                MarkNodeAsTagged(update.Tick, nodeId);
                _taggedVisualized[update.NodeId] = true;
            }
        }

        // Draw new node-messages
        while (_drawBufferMessages.Count != 0)
        {
            var message = _drawBufferMessages.Dequeue();
            PlotNodeMessage(message);
        }

        // Update active bars to latest tick
        for (var n = 0; n < _isNeutral.Length; n++)
        {
            if (_isNeutral[n])
                continue;
            UpdateLastBarValue(n, _latestTick);
        }

        if (EnableAutoScroll)
        {
            Plot.Axes.SetLimitsX(
                _latestTick - (_tickWindow - TickOffsetRight),
                _latestTick + TickOffsetRight
            );
        }

        base.Render(context);
    }

    private void UpdateLastBarValue(int nodeId, long toValue)
    {
        _bars![nodeId].Bars[^1].Value = toValue;
    }

    public void Update(long currentTick)
    {
        if (!EnableDataUpdate)
            return;

        _latestTick = currentTick;
    }

    public void InitGraph(Graph g)
    {
        Plot.Clear();

        // Add bars to plot
        List<(string name, double[] edges)> ranges = g
            .Nodes.Select(n =>
            {
                return ($"Node {n.Id}", new double[] { 0, 0 });
            })
            .ToList();
        _bars = Plot.Add.StackedRanges(ranges, horizontal: true);

        _isNeutral = new bool[g.Nodes.Count];
        _taggedVisualized = new bool[g.Nodes.Count];

        ArrayHelper.FillArray(_isNeutral, true);
        ArrayHelper.FillArray(_taggedVisualized, false);

        Plot.Axes.AutoScale();

        foreach (Node n in g.Nodes)
        {
            AppendStateUpdate(new NodeStateUpdate(n.Id, n.State, n.IsTagged, 0));
        }

        Plot.Axes.Rules.Clear();
        Plot.Axes.Rules.Add(
            new LockedVertical(Plot.Axes.Left, Plot.Axes.Left.Min, Plot.Axes.Left.Max)
        );
    }

    /// <summary>
    /// Reset method implemented by <see cref="IUpdatable"/> to reset simulation related data
    /// </summary>
    public void ResetComponent()
    {
        _latestTick = 0;
    }

    /// <summary>
    /// Reset the view-related data to its initial state
    /// </summary>
    public void ResetView()
    {
        foreach (var b in _bars)
        {
            b.Bars.Clear();
        }

        Plot.Remove<LinePlot>();

        ArrayHelper.FillArray(_isNeutral, true);
    }

    private Color GetStateFillColor(NodeState state) =>
        state switch
        {
            NodeState.Neutral => Colors.Transparent,
            NodeState.Refractory => PlotColors.LightGray,
            NodeState.Processing => PlotColors.Green,
            NodeState.Inhibited => PlotColors.DarkRed,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
        };
}
