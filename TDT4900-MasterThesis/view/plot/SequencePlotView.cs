using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottables;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;

namespace TDT4900_MasterThesis.view.plot;

public class SequencePlotView : AvaPlot, IDrawable, IUpdatable
{
    private readonly Graph _graph;
    private readonly BarPlot[] _bars;

    private readonly NodeState[] _lastState;

    public SequencePlotView(Graph graph)
    {
        _graph = graph;

        _lastState = new NodeState[graph.Nodes.Count];

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
                    new Bar()
                    {
                        FillColor = GetStateColor(n.State),
                        Orientation = Orientation.Horizontal,
                        ValueBase = 0,
                        Value = 0,
                    }
                );

            _lastState[n.Id] = n.State;
        });
    }

    public void Draw()
    {
        foreach (var t in _bars)
        {
            // Set end-value of the latest bar to latest tick
            t.Bars[^1].Value = _currentTick;
        }

        Plot.Axes.SetLimitsX(_currentTick - 100, _currentTick + 50);

        Refresh();
    }

    private long _currentTick = 0;

    public void Update(long currentTick)
    {
        _currentTick = currentTick;

        foreach (var n in _graph.Nodes)
        {
            var state = n.State;

            if (state != _lastState[n.Id])
            {
                _bars[n.Id]
                    .Bars.Add(
                        new Bar()
                        {
                            FillColor = GetStateColor(state),
                            Orientation = Orientation.Horizontal,
                            ValueBase = currentTick,
                            Value = currentTick,
                        }
                    );

                _lastState[n.Id] = state;
            }
        }
    }

    private Color GetStateColor(NodeState state) =>
        state switch
        {
            NodeState.Neutral => Colors.Transparent,
            NodeState.Cooldown => Colors.Aqua,
            NodeState.Processing => Colors.Green,
            NodeState.Inhibited => Colors.Red,
            NodeState.Tagged => Colors.Pink,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
        };
}
