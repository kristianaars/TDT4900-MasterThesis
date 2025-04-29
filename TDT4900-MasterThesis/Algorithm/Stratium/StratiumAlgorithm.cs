using TDT4900_MasterThesis.Algorithm.Stratium.Component;
using TDT4900_MasterThesis.Algorithm.Stratium.Component;
using TDT4900_MasterThesis.Algorithm.Stratium.Engine;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Stratium;

/// <summary>
/// TODO: Rename to Multistrata?
/// </summary>
public class StratiumAlgorithm : BaseAlgorithm<StratiumNode, StratiumEdge, StratiumGraph>
{
    private StratiumAlgorithmMessageEngine? _messageEngine;

    public required StratiumAlgorithmSpec AlgorithmSpec { init; get; }

    public override required StratiumGraph Graph { get; init; }
    public override required StratiumNode StartNode { get; init; }
    public override required StratiumNode TargetNode { get; init; }

    public override void Initialize()
    {
        Graph.Initialize(AlgorithmSpec);

        _messageEngine = new StratiumAlgorithmMessageEngine()
        {
            Graph = Graph,
            TargetNode = Graph.Nodes.Find(n => n.NodeId == TargetNode.NodeId)!,
            StartNode = Graph.Nodes.Find(n => n.NodeId == StartNode.NodeId)!,
        };

        // Configure the Event Handler for all nodes in the graph
        Graph.Nodes.ForEach(n => n.EventHandler = EventHandler);
        _messageEngine.EventHandler = EventHandler;

        TargetNode.TagNode(0);
        StartNode.WaveInitiator = true;
    }

    /// <summary>
    /// Update-loop for the algorithm. This will be executed on every tick.
    /// </summary>
    /// <param name="currentTick"></param>
    public override void Update(long currentTick)
    {
        // Update nodes in the graph
        Graph.Nodes.ForEach(n => n.Update(currentTick));

        // Update the message engine to process node-messages
        _messageEngine!.Update(currentTick);

        // Algorithm is complete when TargetNode is tagged
        IsFinished = _messageEngine.IsFinished;
    }
}
