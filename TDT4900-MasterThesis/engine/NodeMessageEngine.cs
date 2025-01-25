using Serilog;
using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;

namespace TDT4900_MasterThesis.engine;

public class NodeMessageEngine : IUpdatable
{
    private PriorityQueue<Message, long> _messageQueue = new();
    private Graph graph;

    public NodeMessageEngine(Graph graph)
    {
        this.graph = graph;
    }

    public void Update(long currentTick)
    {
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
        Log.Information("Received message {msg}", message);
        MessageBurst(message.Receiver, message.ExecuteAt);
    }

    /// <summary>
    /// Bursts a message to all connected nodes of the source node.
    /// </summary>
    /// <param name="source">Source node</param>
    /// <param name="currentTick">The tick in which the message is sent from the source node</param>
    private void MessageBurst(Node source, long currentTick)
    {
        var outEdges = graph.GetOutEdges(source);
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
