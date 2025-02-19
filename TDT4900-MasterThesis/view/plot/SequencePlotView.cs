using Avalonia;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottables;
using TDT4900_MasterThesis.constants;
using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;
using TDT4900_MasterThesis.view.plot.generator;

namespace TDT4900_MasterThesis.view.plot;

public class SequencePlotView : AvaPlot, IDrawable, IUpdatable
{
    public bool EnableAutoScroll { get; set; } = true;

    private Graph? _graph;
    public Graph? Graph
    {
        get => _graph;
        set
        {
            _graph = value;
            ResetValues();
        }
    }

    private BarPlot[]? _bars;

    private NodeState[]? _lastState;
    private bool[]? _tagged;

    /// <summary>
    /// Vertical size of the sequence state bars
    /// </summary>
    public float BarSize = 0.5f;

    public SequencePlotView()
    {
        Plot.Grid.IsBeneathPlottables = false;
        Plot.Grid.MajorLineColor = PlotColors.DarkGray.WithAlpha(0.5);

        Plot.Grid.YAxisStyle.MajorLineStyle.IsVisible = false;

        Plot.Axes.Bottom.TickGenerator = new IntegerTickGenerator();

        Margin = new Thickness(0);
    }

    public void PlotNodeMessage(NodeMessage message)
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
        var lineColor = isExcitatory ? PlotColors.GreenBorder : Colors.Transparent;
        var endLineColor = isExcitatory ? PlotColors.GreenBorder : PlotColors.DarkRedBorder;
        var startLineColor = isExcitatory ? Colors.Transparent : PlotColors.DarkRedBorder;

        line.LineWidth = 2;
        line.LineColor = lineColor;

        var verticalStartLine = Plot.Add.Line(x1, y1 - BarSize / 2.0f, x1, y1 + BarSize / 2.0f);
        verticalStartLine.LineWidth = 2;
        verticalStartLine.LineColor = startLineColor;

        var verticaEndLine = Plot.Add.Line(x2, y2 - BarSize / 1.5f, x2, y2 + BarSize / 1.5f);
        verticaEndLine.LineWidth = 2;
        verticaEndLine.LineColor = endLineColor;
    }

    public void MarkNodeAsTagged(long atTick, Node n)
    {
        var yCenter = _bars![n.Id].Bars[^1].Rect.VerticalCenter;

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
        if (_graph == null)
            return;

        foreach (var n in _graph.Nodes)
        {
            // Set end-value of the latest bar to latest tick
            UpdateLastBarValue(n.Id);
        }

        if (EnableAutoScroll)
            Plot.Axes.SetLimitsX(_currentTick - 50, _currentTick + 25);

        Refresh();
    }

    private void UpdateLastBarValue(int nodeId)
    {
        _bars![nodeId].Bars[^1].Value = _currentTick;
    }

    private long _currentTick = 0;

    public void Update(long currentTick)
    {
        if (_graph == null)
            return;

        _currentTick = currentTick;

        foreach (var n in _graph!.Nodes)
        {
            var state = n.State;
            var isTagged = n.IsTagged;

            if (isTagged != _tagged![n.Id])
            {
                _tagged[n.Id] = isTagged;
                MarkNodeAsTagged(currentTick, n);
            }

            if (state == _lastState![n.Id])
                continue;

            UpdateLastBarValue(n.Id);

            _bars!
                [n.Id]
                .Bars.Add(
                    new Bar()
                    {
                        Size = BarSize,
                        Position = n.Id,
                        FillColor = GetStateFillColor(state),
                        LineWidth = 0,
                        Orientation = Orientation.Horizontal,
                        ValueBase = currentTick,
                        Value = currentTick,
                    }
                );

            _lastState[n.Id] = state;
        }
    }

    private void ResetValues()
    {
        Plot.Clear();

        // Reset data-structures
        _tagged = new bool[_graph!.Nodes.Count];
        _lastState = _graph.Nodes.Select(n => n.State).ToArray();

        // Add bars to plot
        List<(string name, double[] edges)> ranges = _graph
            .Nodes.Select(n =>
            {
                return ($"Node {n.Id}", new double[] { 0, 0 });
            })
            .ToList();
        _bars = Plot.Add.StackedRanges(ranges, horizontal: true);

        _graph.Nodes.ForEach(n =>
        {
            _bars!
                [n.Id]
                .Bars.Add(
                    new Bar
                    {
                        Size = BarSize,
                        Position = n.Id,
                        FillColor = GetStateFillColor(n.State),
                        Orientation = Orientation.Horizontal,
                        LineColor = GetStateBorderColor(n.State, n.IsTagged),
                        ValueBase = 0,
                        Value = 1,
                    }
                );

            _lastState[n.Id] = n.State;
        });

        Plot.Axes.AutoScale();
        Refresh();
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

    private Color GetStateBorderColor(NodeState state, bool isTagged) =>
        state switch
        {
            NodeState.Neutral => Colors.Transparent,
            _ => isTagged ? Colors.Red : Colors.Black,
        };
}
