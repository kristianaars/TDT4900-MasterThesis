using TDT4900_MasterThesis.Algorithm.Alpha.Component;
using TDT4900_MasterThesis.Model.Graph;
using Log = Serilog.Log;

namespace TDT4900_MasterThesis.Algorithm.Alpha.Engine;

public class AlphaAlgorithmMessageEngine : IUpdatable
{
    public required AlphaGraph Graph { init; get; }
    public required AlphaNode TargetNode { init; get; }
    public required AlphaNode StartNode { init; get; }

    /// <summary>
    /// Queue which holds all processed messages ready to be executed at a specific tick.
    /// Structure is (message, executionTick) where executionTick is the tick at which the message is to be executed
    /// </summary>
    private readonly PriorityQueue<AlphaNodeMessage, long> _messageQueue = new();

    /// <summary>
    /// Queue which holds all messages currently being "processed". A processed message should be added to the queue
    /// if the sender is not inhibited in the meantime.
    /// </summary>
    private readonly PriorityQueue<AlphaProcessingMessage, long> _processingQueue = new();

    public void Update(long currentTick)
    {
        if (_messageQueue.Count + _processingQueue.Count == 0)
        {
            Graph.Nodes.ForEach(node => node.DisinhibitNode(currentTick));
            BeginNewWave(currentTick, currentTick + 5);
        }

        while (_processingQueue.Count > 0 && _processingQueue.Peek().ReceiveAt <= currentTick)
        {
            var message = _processingQueue.Dequeue().SendMessage;
            var sender = message.Sender;

            _messageQueue.Enqueue(message, message.ReceiveAt);

            // Begin refraction if sender exists (It does not exist if the message is a "start" message)
            sender?.BeginRefraction(currentTick);
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
    private void ExecuteNodeMessage(AlphaNodeMessage nodeMessage)
    {
        var receiver = nodeMessage.Receiver;
        var currentTick = nodeMessage.ReceiveAt;

        IEnumerable<AlphaProcessingMessage> newMessages = nodeMessage.Type switch
        {
            AlphaNodeMessage.MessageType.Excitatory => receiver.Excite(currentTick),
            AlphaNodeMessage.MessageType.Inhibitory => receiver.Inhibit(currentTick),
            _ => throw new ArgumentOutOfRangeException(),
        };

        QueueMessages(newMessages!);
    }

    public void QueueMessages(IEnumerable<AlphaProcessingMessage> messages)
    {
        foreach (var message in messages)
        {
            QueueMessage(message);
        }
    }

    public void QueueMessage(AlphaProcessingMessage message)
    {
        _processingQueue.Enqueue(message, message.ReceiveAt);
    }

    public void BeginNewWave(long currentTick, long atTick)
    {
        QueueMessage(
            new AlphaProcessingMessage()
            {
                SentAt = currentTick,
                ReceiveAt = atTick,
                SendMessage = new AlphaNodeMessage()
                {
                    Type = AlphaNodeMessage.MessageType.Excitatory,
                    ReceiveAt = atTick,
                    SentAt = atTick,
                    Receiver = StartNode,
                    Sender = null,
                },
            }
        );
    }

    public void ResetComponent()
    {
        throw new NotImplementedException();
    }
}
