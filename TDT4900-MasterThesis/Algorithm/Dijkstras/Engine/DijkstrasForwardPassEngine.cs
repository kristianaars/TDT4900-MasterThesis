using TDT4900_MasterThesis.Algorithm.Dijkstras.Component;
using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Model.Graph;
using EventHandler = TDT4900_MasterThesis.Handler.EventHandler;

namespace TDT4900_MasterThesis.Algorithm.Dijkstras.Engine;

public class DijkstrasForwardPassEngine : IUpdatable, IEventProducer
{
    public bool IsFinished { get; set; }

    private readonly PriorityQueue<DijkstraNode, int> _priorityQueue = new();

    public required DijkstraGraph Graph { init; get; }
    public required DijkstraNode StartNode { init; get; }
    public required DijkstraNode TargetNode { init; get; }

    public void Update(long currentTick)
    {
        if (_priorityQueue.Count == 0)
        {
            IsFinished = true;
            return;
        }

        var node = _priorityQueue.Dequeue();
        if (node == TargetNode)
        {
            IsFinished = true;
            return;
        }

        var adjecentNodes = Graph.GetOutNeighbours(node);
        foreach (var neighbor in adjecentNodes)
        {
            var distance = node.Distance + 1; //1 Since weight is uniform

            if (distance < neighbor.Distance)
            {
                neighbor.Distance = distance;
                neighbor.Prior = node;
                EnqueueNode(neighbor);

                PostEvent(
                    new NodeEvent()
                    {
                        EventType = EventType.Processing,
                        NodeId = neighbor.NodeId,
                        Tick = currentTick,
                    }
                );
            }
        }
    }

    public void Initialize()
    {
        Graph.Nodes.ForEach(n =>
        {
            n.Distance = int.MaxValue;
            n.Prior = null;
        });

        StartNode.Distance = 0;
        EnqueueNode(StartNode);
    }

    private void EnqueueNode(DijkstraNode node) => _priorityQueue.Enqueue(node, node.Distance);

    public EventHandler? EventHandler { get; set; }

    public void PostEvent(NodeEvent nodeEvent) => EventHandler?.PostEvent(nodeEvent);
}
