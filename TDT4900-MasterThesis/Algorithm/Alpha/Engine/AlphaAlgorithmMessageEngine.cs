using GLib;
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

    public void Update(long currentTick)
    {
        if (_messageQueue.Count == 0)
        {
            BeginNewWave(currentTick, currentTick + 5);
        }

        while (_messageQueue.Count > 0 && _messageQueue.Peek().ReceiveAt <= currentTick)
        {
            var message = _messageQueue.Dequeue();
            var sender = message.Sender;

            // Begin refraction if sender exists (It does not exist if the message is a "start" message)
            sender?.BeginRefraction(currentTick);

            ExecuteNodeMessage(message);
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

        IEnumerable<AlphaNodeMessage> newMessages = nodeMessage.Type switch
        {
            AlphaNodeMessage.MessageType.Excitatory => receiver.Excite(currentTick),
            AlphaNodeMessage.MessageType.Inhibitory => receiver.Inhibit(currentTick),
            _ => throw new ArgumentOutOfRangeException(),
        };

        QueueMessages(newMessages!);
    }

    public void QueueMessages(IEnumerable<AlphaNodeMessage> messages)
    {
        foreach (var message in messages)
        {
            QueueMessage(message);
        }
    }

    public void QueueMessage(AlphaNodeMessage message)
    {
        Log.Information("Queuing the following message: {message}", message);
        _messageQueue.Enqueue(message, message.ReceiveAt);
    }

    public void BeginNewWave(long currentTick, long atTick)
    {
        Graph.Nodes.ForEach(node => node.DisinhibitNode(currentTick));

        QueueMessage(
            new AlphaNodeMessage()
            {
                Type = AlphaNodeMessage.MessageType.Excitatory,
                ReceiveAt = atTick,
                SentAt = atTick,
                Receiver = StartNode,
                Sender = null,
            }
        );
    }

    public void ResetComponent()
    {
        throw new NotImplementedException();
    }
}
