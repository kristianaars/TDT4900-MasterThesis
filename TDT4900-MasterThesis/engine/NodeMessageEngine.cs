using Serilog;
using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;
using TDT4900_MasterThesis.view;
using TDT4900_MasterThesis.view.plot;

namespace TDT4900_MasterThesis.engine;

public class NodeMessageEngine : IUpdatable
{
    /// <summary>
    /// Holds the cooldown period of a node in number of remaining ticks. A node in cooldown period cannot be
    /// activated
    /// </summary>
    private readonly int[] _cooldownPeriods;

    private readonly int[] _iEMessagePairWindow;

    /// <summary>
    /// Cooldown for nodes after activation (in ticks)
    /// </summary>
    private readonly int _nodeCooldownPeriod;

    /// <summary>
    /// The time it takes for an excitatory message to reach a node  (in ticks)
    /// </summary>
    private readonly int _deltaTExcitatory;

    /// <summary>
    /// The time it takes for an inhibitory message to reach a node (in ticks)
    /// </summary>
    private readonly int _deltaTInhibitory;

    /// <summary>
    /// Forward latency for tagged nodes (in ticks)
    /// </summary>
    private readonly int _tauPlus;

    /// <summary>
    /// Forward latency for non-tagged nodes (in ticks)
    /// </summary>
    private readonly int _tauZero;

    /// <summary>
    /// Queue which holds all processed messages ready to be executed at a specific tick.
    /// Structure is (message, executionTick) where executionTick is the tick at which the message is to be executed
    /// </summary>
    private readonly PriorityQueue<Message, long> _messageQueue = new();

    /// <summary>
    /// Queue which holds all messages currently being "processed". A processed message should be added to the queue
    /// if the sender is not inhibited in the meantime.
    /// </summary>
    private readonly PriorityQueue<Message, long> _processingQueue = new();

    /// <summary>
    /// Graph to be used for the simulation
    /// </summary>
    private readonly Graph _graph;

    private readonly GraphPlotView? _graphPlotView;

    public NodeMessageEngine(
        Graph graph,
        AppSettings appSettings,
        GraphPlotView? graphPlotView = null
    )
    {
        _graph = graph;
        _graphPlotView = graphPlotView;

        _cooldownPeriods = new int[_graph.Nodes.Count];
        _iEMessagePairWindow = new int[_graph.Nodes.Count];

        _nodeCooldownPeriod = appSettings.Simulation.NodeCooldownPeriod;

        _deltaTExcitatory = appSettings.Simulation.DeltaTExcitatory;
        _deltaTInhibitory = appSettings.Simulation.DeltaTInhibitory;

        _tauZero = appSettings.Simulation.TauZero;
        _tauPlus = appSettings.Simulation.TauPlus;
    }

    public void Update(long currentTick)
    {
        // Decrease time periods for all nodes
        for (var i = 0; i < _graph.Nodes.Count; i++)
        {
            if (_iEMessagePairWindow[i] > 0)
            {
                _iEMessagePairWindow[i]--;
            }

            if (_cooldownPeriods[i] > 0)
            {
                _cooldownPeriods[i]--;
            }
        }

        while (_processingQueue.Count > 0 && _processingQueue.Peek().ReceiveAt <= currentTick)
        {
            var message = _processingQueue.Dequeue();
            if (message.Sender is { State: NodeState.Inhibited })
                continue;
            _messageQueue.Enqueue(message, message.ReceiveAt);
        }

        while (_messageQueue.Count > 0 && _messageQueue.Peek().ReceiveAt <= currentTick)
        {
            ExecuteMessage(_messageQueue.Dequeue());
        }
    }

    /// <summary>
    /// Executes the message
    /// </summary>
    /// <param name="message">Message to execute</param>
    private void ExecuteMessage(Message message)
    {
        var receiver = message.Receiver;

        var currentTick = message.ReceiveAt;

        //Log.Debug("[{tick}] Received message {msg}", currentTick, message);

        switch (message.Type)
        {
            case Message.MessageType.Excitatory:

                _graphPlotView?.ActivateNode(receiver.Id);

                if (receiver is { State: NodeState.Tagged })
                {
                    GlobalInhibitoryMessageBurst(
                        source: receiver,
                        currentTick: currentTick,
                        tau: _tauPlus,
                        deltaT: _deltaTInhibitory
                    );

                    ExcitatoryMessageBurst(
                        source: receiver,
                        currentTick: currentTick,
                        tau: _tauPlus,
                        deltaT: _deltaTExcitatory
                    );
                }
                else if (receiver is { State: NodeState.Inhibited })
                {
                    if (
                        _iEMessagePairWindow[receiver.Id] > 0
                        && _iEMessagePairWindow[receiver.Id] < _tauZero
                    )
                    {
                        receiver.State = NodeState.Tagged;

                        Log.Information(
                            "Excitatory message pair window is active for node {node} with expiration in {window} ticks",
                            receiver,
                            _iEMessagePairWindow[receiver.Id]
                        );
                    }
                }
                else
                {
                    // If the source node is in cooldown, do not burst messages
                    var isInCooldown = _cooldownPeriods[receiver.Id] > 0;
                    if (isInCooldown)
                        return;

                    // Burst to all neighbouring nodes
                    ExcitatoryMessageBurst(
                        source: receiver,
                        currentTick: currentTick,
                        tau: _tauZero,
                        deltaT: _deltaTExcitatory
                    );
                }
                break;
            case Message.MessageType.Inhibitory:
                receiver.State = NodeState.Inhibited;
                break;
        }
    }

    /// <summary>
    /// Bursts an excitatory message to all neighbouring nodes of the source node.
    /// </summary>
    /// <param name="source">The source of the excitatory message burst</param>
    /// <param name="currentTick">The current tick in time, will be used together with tau to decide actual message time</param>
    /// <param name="tau">Forward latency for the source node (Processing time)</param>
    /// <param name="deltaT">Time it takes for the excitatory burst being received by targets</param>
    private void ExcitatoryMessageBurst(Node source, long currentTick, int tau, int deltaT)
    {
        if (source is { State: NodeState.Inhibited })
            return;

        // One extra _tauZero since the message pair window is defined before the message is sent
        _iEMessagePairWindow[source.Id] = (2 * _deltaTExcitatory + 2 * _tauZero) - 1;
        Log.Debug(
            "[{tick}] Setting message pair window for node {node} to {exitation} ticks",
            currentTick,
            source,
            _iEMessagePairWindow[source.Id]
        );

        // refractory period
        _cooldownPeriods[source.Id] = _nodeCooldownPeriod;
        Log.Debug(
            "[{tick}] Setting cooldown period for node {node} to {cooldown} ticks",
            currentTick,
            source,
            _cooldownPeriods[source.Id]
        );

        var outEdges = _graph.GetOutEdges(source);
        foreach (var edge in outEdges)
        {
            SendMessage(
                new Message(
                    currentTick + deltaT + tau,
                    source,
                    edge.Target,
                    Message.MessageType.Excitatory
                ),
                tau
            );
        }
    }

    /// <summary>
    /// Bursts an inhibitory message to all neighbouring nodes of the source node.
    /// </summary>
    /// <param name="source">The source of the inhibitory message burst</param>
    /// <param name="currentTick">The current tick in time, will be used together with tau to decide actual message time</param>
    /// <param name="tau">Forward latency for the source node (Processing time)</param>
    /// <param name="deltaT">Time it takes for the inhibitory burst being received by targets</param>
    private void InhibitoryMessageBurst(Node source, long currentTick, int tau, int deltaT)
    {
        if (source is { State: NodeState.Inhibited })
            return;

        var outEdges = _graph.GetOutEdges(source);
        foreach (var edge in outEdges)
        {
            SendMessage(
                new Message(
                    currentTick + deltaT + tau,
                    source,
                    edge.Target,
                    Message.MessageType.Inhibitory
                ),
                tau
            );
        }
    }

    /// <summary>
    /// Sends an inhibitory message burst to all nodes in <see cref="_graph"/>
    /// </summary>
    /// <param name="source">The source of the inhibitory message burst</param>
    /// <param name="currentTick">The tick when the inhibitory burst should be processed</param>
    /// <param name="tau">Forward latency for the source node (Processing time)</param>
    /// <param name="deltaT">Time it takes for the inhibitory burst being received by targets</param>
    private void GlobalInhibitoryMessageBurst(Node source, long currentTick, int tau, int deltaT)
    {
        _graph.Nodes.ForEach(n =>
        {
            if (!Equals(n, source))
                SendMessage(
                    new Message(
                        currentTick + deltaT + tau,
                        source,
                        n,
                        Message.MessageType.Inhibitory
                    ),
                    tau
                );
        });
    }

    /// <summary>
    /// Adds a message to the message queue with the specified execution time
    /// </summary>
    /// <param name="message">Message to be sent</param>
    /// <param name="tau">Processing time for message before being sent</param>
    public void SendMessage(Message message, int tau)
    {
        //Log.Information("Queuing message {msg}", message);
        _processingQueue.Enqueue(message, message.ReceiveAt + tau);
    }
}
