using TDT4900_MasterThesis.Algorithm.Alpha.Component;
using TDT4900_MasterThesis.Algorithm.Alpha.Engine;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Alpha;

public class AlphaAlgorithm : BaseAlgorithm
{
    private AlphaAlgorithmMessageEngine? _messageEngine;

    public required AlphaAlgorithmSpec AlgorithmSpec { init; get; }
    public required AlphaGraph Graph { init; get; }
    public required AlphaNode StartNode { init; get; }
    public required AlphaNode TargetNode { init; get; }

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

        // Algorithm is complete when StartNode is tagged
        if (StartNode.IsTagged)
            IsFinished = true;
    }
}
