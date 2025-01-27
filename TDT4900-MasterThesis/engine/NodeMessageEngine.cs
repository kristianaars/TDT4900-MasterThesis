using Serilog;
using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;
using TDT4900_MasterThesis.view;

namespace TDT4900_MasterThesis.engine;

public class NodeMessageEngine : IUpdatable
{
    /// <summary>
    /// Holds the cooldown period of a node in number of remaining ticks. A node in cooldown period cannot be
    /// activated
    /// </summary>
    private readonly int[] _cooldownPeriods;

    private readonly int _nodeCooldownPeriod;

    private readonly PriorityQueue<Message, long> _messageQueue = new();
    private readonly Graph _graph;

    private readonly GraphView _graphView;

    public NodeMessageEngine(Graph graph, GraphView graphView, AppSettings appSettings)
    {
        _graph = graph;
        _graphView = graphView;

        _cooldownPeriods = new int[_graph.Nodes.Count];
        _nodeCooldownPeriod = appSettings.Simulation.NodeCooldownPeriod;
    }

    public void Update(long currentTick)
    {
        // Decrease cooldown period for a node if it is in cooldown
        for (var i = 0; i < _cooldownPeriods.Length; i++)
        {
            if (_cooldownPeriods[i] > 0)
                _cooldownPeriods[i]--;
        }

        while (_messageQueue.Count > 0 && _messageQueue.Peek().ExecuteAt <= currentTick)
        {
            ExecuteMessage(_messageQueue.Dequeue());
        }
    }

    /// <summary>
    /// Execute the message
    /// </summary>
    /// <param name="message">Message to execute</param>
    private void ExecuteMessage(Message message)
    {
        var receiver = message.Receiver;

        //Log.Information("Received message {msg}", message);

        // If the source node is in cooldown, do not burst messages
        var isInCooldown = _cooldownPeriods[receiver.Id] > 0;
        if (isInCooldown)
            return;

        // Visualize the activation of the node
        _graphView.ActivateNode(receiver);

        MessageBurst(message.Receiver, [message.Sender], message.ExecuteAt);
    }

    /// <summary>
    /// Bursts a message to all connected nodes of the source node.
    /// </summary>
    /// <param name="source">Source node</param>
    /// <param name="exclude">Nodes to exclude from the burst</param>
    /// <param name="currentTick">The tick in which the message is sent from the source node</param>
    private void MessageBurst(Node source, Node[] exclude, long currentTick)
    {
        _cooldownPeriods[source.Id] = _nodeCooldownPeriod;

        var outEdges = _graph.GetOutEdges(source);
        foreach (var edge in outEdges)
        {
            SendMessage(new Message(currentTick + edge.Weight, source, edge.Target));
        }
    }

    /// <summary>
    /// Adds a message to the message queue with the specified execution time
    /// </summary>
    /// <param name="message"></param>
    public void SendMessage(Message message)
    {
        _messageQueue.Enqueue(message, message.ExecuteAt);
    }
}
