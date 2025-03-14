using TDT4900_MasterThesis.Algorithm.Dijkstras.Component;
using TDT4900_MasterThesis.Algorithm.Dijkstras.Engine;
using TDT4900_MasterThesis.Handler;

namespace TDT4900_MasterThesis.Algorithm.Dijkstras;

public class DijkstrasAlgorithm : BaseEventProducer, IAlgorithm
{
    public required DijkstraGraph Graph { init; get; }
    public required DijkstraNode StartNode { init; get; }
    public required DijkstraNode TargetNode { init; get; }

    public bool IsFinished { get; set; }

    private DijkstrasForwardPassEngine? _forwardPassEngine;
    private DijkstrasBacktrackEngine? _backtrackEngine;

    public void Update(long currentTick)
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

    public void Initialize()
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
