using TDT4900_MasterThesis.Algorithm.Alpha.Component;
using TDT4900_MasterThesis.Algorithm.Alpha.Engine;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Alpha;

public class AlphaAlgorithmAlgorithmAlgorithmAlgorithm
    : BaseAlgorithmAlgorithmAlgorithmAlgorithm<AlphaNode, AlphaEdge, AlphaGraph>
{
    private AlphaAlgorithmMessageEngine? _messageEngine;

    public required AlphaAlgorithmSpec AlgorithmSpec { init; get; }

    public override required AlphaGraph Graph { get; init; }
    public override required AlphaNode StartNode { get; init; }
    public override required AlphaNode TargetNode { get; init; }

    public override void Initialize()
    {
        Graph.Initialize(AlgorithmSpec);

        // Configure the Event Handler for all nodes in the graph
        Graph.Nodes.ForEach(n => n.EventHandler = EventHandler);

        _messageEngine = new AlphaAlgorithmMessageEngine()
        {
            Graph = Graph,
            TargetNode = Graph.Nodes.Find(n => n.NodeId == TargetNode.NodeId)!,
            StartNode = Graph.Nodes.Find(n => n.NodeId == StartNode.NodeId)!,
        };

        TargetNode.TagNode(0);
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
