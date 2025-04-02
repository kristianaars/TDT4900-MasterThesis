using TDT4900_MasterThesis.Algorithm.Stratium.Component;
using TDT4900_MasterThesis.Algorithm.Stratium.Component;
using TDT4900_MasterThesis.Model.Graph;

namespace TDT4900_MasterThesis.Algorithm.Stratium.Engine;

public class StratiumAlgorithmMessageEngine : IUpdatable
{
    public required StratiumGraph Graph { init; get; }
    public required StratiumNode TargetNode { init; get; }
    public required StratiumNode StartNode { init; get; }

    public bool IsFinished = false;

    /// <summary>
    /// Queue which holds all processed messages ready to be executed at a specific tick.
    /// Structure is (message, executionTick) where executionTick is the tick at which the message is to be executed
    /// </summary>
    private readonly PriorityQueue<StratiumNodeMessage, long> _messageQueue = new();

    /// <summary>
    /// Queue which holds all messages currently being "processed". A processed message should be added to the queue
    /// if the sender is not inhibited in the meantime.
    /// </summary>
    private readonly PriorityQueue<StratiumProcessingMessage, long> _processingQueue = new();

    public void Update(long currentTick)
    {
        if (_messageQueue.Count + _processingQueue.Count == 0)
        {
            Graph.Nodes.ForEach(node => node.DisinhibitNode(currentTick));

            IsFinished = StartNode.IsTagged;

            if (!IsFinished)
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
    private void ExecuteNodeMessage(StratiumNodeMessage nodeMessage)
    {
        var receiver = nodeMessage.Receiver;
        var currentTick = nodeMessage.ReceiveAt;

        IEnumerable<StratiumProcessingMessage> newMessages = nodeMessage.Type switch
        {
            StratiumNodeMessage.MessageType.Excitatory => receiver.Excite(currentTick),
            StratiumNodeMessage.MessageType.Inhibitory => receiver.Inhibit(currentTick),
            _ => throw new ArgumentOutOfRangeException(),
        };

        QueueMessages(newMessages!);
    }

    public void QueueMessages(IEnumerable<StratiumProcessingMessage> messages)
    {
        foreach (var message in messages)
        {
            QueueMessage(message);
        }
    }

    public void QueueMessage(StratiumProcessingMessage message)
    {
        _processingQueue.Enqueue(message, message.ReceiveAt);
    }

    public void BeginNewWave(long currentTick, long atTick)
    {
        QueueMessage(
            new StratiumProcessingMessage()
            {
                SentAt = currentTick,
                ReceiveAt = atTick,
                SendMessage = new StratiumNodeMessage()
                {
                    Type = StratiumNodeMessage.MessageType.Excitatory,
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
