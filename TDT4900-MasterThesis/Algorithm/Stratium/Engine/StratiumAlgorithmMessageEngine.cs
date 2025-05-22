using TDT4900_MasterThesis.Algorithm.Stratium.Component;
using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Model.Graph;

namespace TDT4900_MasterThesis.Algorithm.Stratium.Engine;

public class StratiumAlgorithmMessageEngine : IUpdatable, IAlgorithmEventProducer
{
    public required StratiumGraph Graph { init; get; }
    public required StratiumNode TargetNode { init; get; }
    public required StratiumNode StartNode { init; get; }

    public bool IsFinished;

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

    public int WavesBeforeFinish = 1;

    public void Update(long currentTick)
    {
        lock (_processingQueue)
        {
            if (_messageQueue.Count + _processingQueue.Count == 0)
            {
                Graph.Nodes.ForEach(node => node.DisinhibitNode(currentTick));

                if (StartNode is { IsTagged: true })
                    IsFinished = WavesBeforeFinish-- < 0;

                if (!IsFinished)
                    BeginNewWave(currentTick, currentTick + 5);
            }
        }

        lock (_processingQueue)
        {
            while (_processingQueue.Count > 0 && _processingQueue.Peek().ReceiveAt <= currentTick)
            {
                var message = _processingQueue.Dequeue().SendMessage;

                _messageQueue.Enqueue(message, message.ReceiveAt);

                if (message.SourceEdge != null)
                    PostEvent(
                        new EdgeEvent
                        {
                            SourceId = message.SourceEdge.GetOtherNode(message.Receiver).NodeId,
                            TargetId = message.Receiver.NodeId,
                            Level = message.SourceEdge.Level,
                            EventType =
                                message.Type == StratiumNodeMessage.MessageType.Excitatory
                                    ? EdgeEventType.Excitatory
                                    : EdgeEventType.Inhibitory,
                            Tick = message.SentAt,
                            ReceiveAt = message.ReceiveAt,
                            Charge = message.Charge,
                        }
                    );

                // Begin refraction if sender exists (It does not exist if the message is a "start" message)
                message.Source?.BeginRefraction(currentTick);
            }
        }

        var tasks = new List<Task>();

        int jobsPerThread = 1000;

        while (_messageQueue.Count > 0 && _messageQueue.Peek().ReceiveAt <= currentTick)
        {
            var messages = new List<StratiumNodeMessage>();
            for (
                int i = 0;
                i < jobsPerThread
                    && _messageQueue.Count > 0
                    && _messageQueue.Peek().ReceiveAt <= currentTick;
                i++
            )
            {
                messages.Add(_messageQueue.Dequeue());
            }

            tasks.Add(
                Task.Run(() =>
                {
                    messages.ForEach(message =>
                    {
                        if (message.SourceEdge != null)
                            PostEvent(
                                new EdgeEvent
                                {
                                    SourceId = message.SourceEdge.GetOtherNodeId(
                                        message.Receiver.NodeId
                                    ),
                                    TargetId = message.Receiver.NodeId,
                                    Level = message.SourceEdge.Level,
                                    EventType = EdgeEventType.Neutral,
                                    Tick = message.ReceiveAt,
                                    ReceiveAt = message.ReceiveAt,
                                    Charge = 1,
                                }
                            );

                        ExecuteNodeMessage(message);
                    });
                })
            );
        }

        Task.WaitAll(tasks);
    }

    /// <summary>
    /// Executes the message and queues successive messages as a result of the message
    /// </summary>
    /// <param name="nodeMessage">Message to execute</param>
    private void ExecuteNodeMessage(StratiumNodeMessage nodeMessage)
    {
        var receiver = nodeMessage.Receiver;
        var currentTick = nodeMessage.ReceiveAt;
        var edgeLevel = nodeMessage.SourceEdge?.Level ?? 0;

        IEnumerable<StratiumProcessingMessage> newMessages = nodeMessage.Type switch
        {
            StratiumNodeMessage.MessageType.Excitatory => receiver.Excite(
                currentTick,
                nodeMessage.Charge
            ),
            StratiumNodeMessage.MessageType.Inhibitory => receiver.Inhibit(currentTick),
            StratiumNodeMessage.MessageType.InitiateWave => receiver.InitiateWave(currentTick),
            _ => throw new ArgumentOutOfRangeException(),
        };

        QueueMessages(newMessages);
    }

    public void QueueMessages(IEnumerable<StratiumProcessingMessage> messages)
    {
        lock (_procesingQueueLock)
        {
            foreach (var message in messages)
            {
                QueueMessage(message);
            }
        }
    }

    private Lock _procesingQueueLock = new();

    public void QueueMessage(StratiumProcessingMessage message)
    {
        lock (_processingQueue)
        {
            _processingQueue.Enqueue(message, message.ReceiveAt);
        }
    }

    public void BeginNewWave(long currentTick, long atTick)
    {
        // Create a new message for all nodes marked as wave initiators
        var waveInitiatorMessages = Graph
            .Nodes.Where(n => n.WaveInitiator)
            .SelectMany(n => n.InitiateWave(currentTick));

        // Queue the messages to execute the wave
        QueueMessages(waveInitiatorMessages);
    }

    public AlgorithmEventHandler? EventHandler { get; set; }

    public void PostEvent(AlgorithmEvent algEvent)
    {
        EventHandler?.PostEvent(algEvent);
    }
}
