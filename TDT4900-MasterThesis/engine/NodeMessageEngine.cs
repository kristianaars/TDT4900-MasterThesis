using Serilog;
using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;

namespace TDT4900_MasterThesis.engine;

public class NodeMessageEngine : IUpdatable
{
    /// <summary>
    /// Queue which holds all processed messages ready to be executed at a specific tick.
    /// Structure is (message, executionTick) where executionTick is the tick at which the message is to be executed
    /// </summary>
    private readonly PriorityQueue<NodeMessage, long> _messageQueue = new();

    /// <summary>
    /// Queue which holds all messages currently being "processed". A processed message should be added to the queue
    /// if the sender is not inhibited in the meantime.
    /// </summary>
    private readonly PriorityQueue<ProcessMessage, long> _processingQueue = new();

    /// <summary>
    /// Graph to be used for the simulation
    /// </summary>
    private readonly Graph _graph;

    public NodeMessageEngine(Graph graph)
    {
        _graph = graph;
    }

    public void Update(long currentTick)
    {
        while (_processingQueue.Count > 0 && _processingQueue.Peek().ReceiveAt <= currentTick)
        {
            var message = _processingQueue.Dequeue().SendMessage;
            var sender = message.Sender;

            _messageQueue.Enqueue(message, message.ReceiveAt);

            // Begin refraction if sender exists (It does not exist if the message is a "start" message)
            sender?.BeginRefraction();
        }

        while (_messageQueue.Count > 0 && _messageQueue.Peek().ReceiveAt <= currentTick)
        {
            ExecuteNodeMessage(_messageQueue.Dequeue());
        }
    }

    /// <summary>
    /// Executes the message and queues successive messages as a result of the message
    /// </summary>
    /// <param name="nodeMessage">Message to execute</param>
    private void ExecuteNodeMessage(NodeMessage nodeMessage)
    {
        var receiver = nodeMessage.Receiver;
        var currentTick = nodeMessage.ReceiveAt;

        ProcessMessage[] newMessages;

        switch (nodeMessage.Type)
        {
            case NodeMessage.MessageType.Excitatory:
                newMessages = receiver.Excite(currentTick);
                break;
            case NodeMessage.MessageType.Inhibitory:
                newMessages = receiver.Excite(currentTick);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        QueueProcessMessages(newMessages!);
    }

    /// <summary>
    /// Adds a message to the message-processing queue with the specified execution time
    /// </summary>
    /// <param name="message">Message to be sent</param>
    public void QueueProcessMessage(ProcessMessage message)
    {
        Log.Information("Queuing message {message}", message);

        _processingQueue.Enqueue(message, message.ReceiveAt);
    }

    public void QueueProcessMessages(ProcessMessage[] messages)
    {
        foreach (var processMessage in messages)
        {
            QueueProcessMessage(processMessage);
        }
    }
}
