using TDT4900_MasterThesis.Algorithm.Dijkstras.Component;
using TDT4900_MasterThesis.Algorithm.Dijkstras.Engine;
using TDT4900_MasterThesis.View.Plot;

namespace TDT4900_MasterThesis.Algorithm.Dijkstras;

public class DijkstrasAlgorithm : BaseAlgorithm<DijkstraNode, DijkstraEdge, DijkstraGraph>
{
    public override required DijkstraGraph Graph { get; init; }
    public override required DijkstraNode StartNode { get; init; }
    public override required DijkstraNode TargetNode { get; init; }

    private DijkstrasForwardPassEngine? _forwardPassEngine;
    private DijkstrasBacktrackEngine? _backtrackEngine;

    public override void Update(long currentTick)
    {
        //1. Perform forward pass
        if (!_forwardPassEngine!.IsFinished)
            _forwardPassEngine.Update(currentTick);
        // 2. Perform backtracking to locate shortest path
        else if (!_backtrackEngine!.IsFinished)
            _backtrackEngine.Update(currentTick);
        else
            IsFinished = true;
    }

    public override void Initialize()
    {
        _forwardPassEngine = new DijkstrasForwardPassEngine()
        {
            Graph = Graph,
            StartNode = StartNode,
            TargetNode = TargetNode,
            EventHandler = EventHandler,
        };

        _backtrackEngine = new DijkstrasBacktrackEngine()
        {
            TargetNode = TargetNode,
            EventHandler = EventHandler,
        };

        Graph.Initialize();
        _forwardPassEngine.Initialize();
        _backtrackEngine.Initialize();
    }
}
