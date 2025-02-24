using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using TDT4900_MasterThesis.message;
using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;
using TDT4900_MasterThesis.view.plot;

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

    private readonly SequencePlotView? _sequencePlotView;

    private Graph? _graph;

    public NodeMessageEngine(SequencePlotView? sequencePlotView)
    {
        _sequencePlotView = sequencePlotView;

        WeakReferenceMessenger.Default.Register<NewGraphMessage>(
            this,
            (r, m) => ReceiveNewGraphMessage(m)
        );
    }

    public void Update(long currentTick)
    {
        if (_graph == null)
            return;

        if (_messageQueue.Count + _processingQueue.Count == 0)
        {
            BeginNewWave(currentTick);
        }

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

    public void ResetComponent()
    {
        _messageQueue.Clear();
        _processingQueue.Clear();
    }

    public void BeginNewWave(long atTick)
    {
        var target = _graph!.Nodes[0];
        _graph.Nodes.ForEach(node => node.State = NodeState.Neutral);

        // Solution is found, no need to perform a new wave
        if (target.IsTagged)
            return;

        QueueProcessMessage(
            new ProcessMessage(
                atTick,
                atTick,
                new NodeMessage(atTick, atTick, null, target, NodeMessage.MessageType.Excitatory)
            )
        );
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
                if (receiver.State != NodeState.Refractory)
                    _sequencePlotView?.AppendNodeMessage(nodeMessage);

                newMessages = receiver.Excite(currentTick);
                break;
            case NodeMessage.MessageType.Inhibitory:
                newMessages = receiver.Inhibit(currentTick);
                _sequencePlotView?.AppendNodeMessage(nodeMessage);
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
        //Log.Information("Queuing message {message}", message);

        _processingQueue.Enqueue(message, message.ReceiveAt);
    }

    public void QueueProcessMessages(ProcessMessage[] messages)
    {
        foreach (var processMessage in messages)
        {
            QueueProcessMessage(processMessage);
        }
    }

    private void ReceiveNewGraphMessage(NewGraphMessage message)
    {
        _graph = message.Value;
        ResetComponent();
    }
}
