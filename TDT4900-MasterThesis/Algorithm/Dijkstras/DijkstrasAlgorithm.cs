using NetTopologySuite.Utilities;
using TDT4900_MasterThesis.Algorithm.Dijkstras.Component;
using TDT4900_MasterThesis.Algorithm.Dijkstras.Engine;
using TDT4900_MasterThesis.Model.Db;
using EventHandler = TDT4900_MasterThesis.Handler.EventHandler;

namespace TDT4900_MasterThesis.Algorithm.Dijkstras;

public class DijkstrasAlgorithm : IAlgorithm
{
    public required DijkstraGraph Graph { init; get; }
    public required DijkstraNode StartNode { init; get; }
    public required DijkstraNode TargetNode { init; get; }

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

    public EventHandler? EventHandler { get; set; }

    public void PostEvent(NodeEvent nodeEvent)
    {
        EventHandler?.PostEvent(nodeEvent);
    }

    public bool IsFinished { get; set; }

    public void Initialize()
    {
        _forwardPassEngine = new DijkstrasForwardPassEngine()
        {
            Graph = Graph,
            StartNode = StartNode,
            TargetNode = TargetNode,
        };

        _backtrackEngine = new DijkstrasBacktrackEngine() { TargetNode = TargetNode };

        Graph.Initialize();
        _forwardPassEngine.Initialize();
        _backtrackEngine.Initialize();
    }
}
