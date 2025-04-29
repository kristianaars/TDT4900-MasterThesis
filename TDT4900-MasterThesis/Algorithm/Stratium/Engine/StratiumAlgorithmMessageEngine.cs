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

    public bool IsFinished = false;

    /// <summary>
    /// Buffer to allow search to complete before the algorithm is marked as finished.
    /// </summary>
    private int _isFinishedWaveCountBuffer = 8;

    /// <summary>
    /// Experimental!
    /// </summary>
    private int _wavesPerLevel = 5;

    private int _wavesLeftForLevel = 5;

    public int CurrentSearchLevel { get; set; } = 2;

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

    /// <summary>
    /// Keeps track of tagged nodes in the previous wave, to know when the algorithm can be completed.
    /// </summary>
    private bool[] _taggedNodes = [];

    public void Update(long currentTick)
    {
        if (_messageQueue.Count + _processingQueue.Count == 0)
        {
            Graph.Nodes.ForEach(node => node.DisinhibitNode(currentTick));

            // Decrease search level when the given number of waves have completed on the current level
            _wavesLeftForLevel = Math.Max(0, --_wavesLeftForLevel);
            if (_wavesLeftForLevel == 0)
            {
                _wavesLeftForLevel = _wavesPerLevel;
                CurrentSearchLevel = Math.Max(0, --CurrentSearchLevel);

                Graph.Nodes.ForEach(node =>
                    node.SearchLevel = Math.Min(node.SearchLevel, CurrentSearchLevel)
                );
            }

            if (StartNode is { IsTagged: true, SearchLevel: 0 })
            {
                _isFinishedWaveCountBuffer--;
                IsFinished = _isFinishedWaveCountBuffer <= 0;
            }

            if (!IsFinished)
                BeginNewWave(currentTick, currentTick + 5);
        }

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
                    }
                );

            // Begin refraction if sender exists (It does not exist if the message is a "start" message)
            message.Source?.BeginRefraction(currentTick);
        }

        while (_messageQueue.Count > 0 && _messageQueue.Peek().ReceiveAt <= currentTick)
        {
            var message = _messageQueue.Dequeue();
            if (message.SourceEdge != null)
                PostEvent(
                    new EdgeEvent
                    {
                        SourceId = message.SourceEdge.GetOtherNodeId(message.Receiver.NodeId),
                        TargetId = message.Receiver.NodeId,
                        Level = message.SourceEdge.Level,
                        EventType = EdgeEventType.Neutral,
                        Tick = message.ReceiveAt,
                        ReceiveAt = message.ReceiveAt,
                    }
                );
            ExecuteNodeMessage(message);
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
        var edgeLevel = nodeMessage.SourceEdge?.Level ?? 0;

        IEnumerable<StratiumProcessingMessage> newMessages = nodeMessage.Type switch
        {
            StratiumNodeMessage.MessageType.Excitatory => receiver.Excite(currentTick, edgeLevel),
            StratiumNodeMessage.MessageType.Inhibitory => receiver.Inhibit(currentTick),
            StratiumNodeMessage.MessageType.InitiateWave => receiver.InitiateWave(currentTick),
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
        // Check if the algorithm is finished (e.g. no new nodes were tagged since last wave)
        var currentlyTaggedNodes = Graph.Nodes.Select(n => n.IsTagged).ToArray();
        if (currentlyTaggedNodes.Equals(_taggedNodes))
        {
            IsFinished = true;
            return;
        }

        _taggedNodes = currentlyTaggedNodes;

        // Create a new message for all nodes marked as wave initiators
        var waveInitiatorMessages = Graph
            .Nodes.Where(n => n.WaveInitiator)
            .Select(n => new StratiumProcessingMessage()
            {
                SentAt = currentTick,
                ReceiveAt = atTick,
                SendMessage = new StratiumNodeMessage()
                {
                    Type = StratiumNodeMessage.MessageType.InitiateWave,
                    SourceEdge = null,
                    ReceiveAt = atTick,
                    SentAt = atTick,
                    Receiver = n,
                },
            });

        // Queue the messages to execute the wave
        QueueMessages(waveInitiatorMessages);
    }

    public AlgorithmEventHandler? EventHandler { get; set; }

    public void PostEvent(AlgorithmEvent algEvent)
    {
        EventHandler?.PostEvent(algEvent);
    }
}
