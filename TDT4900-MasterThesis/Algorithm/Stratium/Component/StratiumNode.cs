using AutoMapper;
using TDT4900_MasterThesis.Algorithm.Component;
using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Stratium.Component;

[AutoMap(typeof(Node))]
public class StratiumNode : AlgorithmNode, IAlgorithmEventProducer
{
    private static bool Directed => true;

    public List<StratiumNode> Neighbours { get; set; } = new();
    public List<StratiumNode> AllNodes { get; set; } = new();
    public List<StratiumEdge> NeighbouringEdges { get; set; } = new();

    public int SearchLevel { get; set; } = 1;

    public int MaxSearchLevel { get; set; } = 4;

    /// <summary>
    /// A node that initiates a wave will excite its neighbors when a new wave is initiated.
    /// </summary>
    public bool WaveInitiator { get; set; }

    /// <summary>
    /// Tau is the processing time, e.g. how long a message takes to process before sending it to the next node.
    /// </summary>
    public int Tau { get; set; }

    /// <summary>
    /// Time it takes to send an excitatory message to other nodes
    /// </summary>
    public int DeltaExcitatory { get; set; }

    /// <summary>
    /// Time it takes to send an inhibitory message to other nodes
    /// </summary>
    public int DeltaInhibitory { get; set; }

    /// <summary>
    /// The default tau-value (processing time) for all nodes. This is the initial value of <see cref="Tau"/> before
    /// the node is tagged.
    /// </summary>
    public int TauZero { get; set; }

    /// <summary>
    /// Tau-value (processing time) for tagged nodes. This is the value of <see cref="Tau"/> after the node is tagged.
    /// </summary>
    public int TauPlus { get; set; }

    /// <summary>
    /// Refractory period of the node after it has been excited
    /// </summary>
    public int RefractoryPeriod { get; set; }

    private int _refractoryCounter;
    private int _taggedExcitationWindow;
    private int _taggedInhibitionWindow;

    /// <summary>
    /// Update the state of the node
    /// </summary>
    /// <param name="currentTick"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Update(long currentTick)
    {
        if (_taggedExcitationWindow > 0)
        {
            _taggedExcitationWindow--;
        }

        if (_taggedInhibitionWindow > 0)
        {
            _taggedInhibitionWindow--;
        }

        if (_refractoryCounter > 0)
        {
            _refractoryCounter--;
            if (_refractoryCounter <= 0)
            {
                if (this is not { State: NodeState.Refractory })
                    throw new InvalidOperationException(
                        $"Cannot disable refractory state when node is in state {State}. Refractory counter: {_refractoryCounter}. Node: {this}"
                    );
                State = NodeState.Neutral;
                PostEvent(NodeEventType.Neutral, currentTick);
            }
        }
    }

    /// <summary>
    /// Enters the refractory state for <see cref="RefractoryPeriod"/> ticks.
    /// The node must be in processing or refractory state for refractory period to kick in.
    /// </summary>
    public void BeginRefraction(long currentTick)
    {
        if (this is { State: NodeState.Processing or NodeState.Refractory })
        {
            _refractoryCounter = RefractoryPeriod;
            State = NodeState.Refractory;
            PostEvent(NodeEventType.Refractory, currentTick);
        }
    }

    /// <summary>
    /// Excite the node, and return a list of messages to send to other nodes as a result of the excitation
    /// </summary>
    /// <param name="currentTick">Tick of the excitation</param>
    /// <param name="charge">The charge-level of the exitation</param>
    /// <returns></returns>
    public StratiumProcessingMessage[] Excite(long currentTick, int charge)
    {
        StratiumProcessingMessage[] messages;

        switch (State)
        {
            case NodeState.Neutral:
                _taggedExcitationWindow = DeltaExcitatory * 2 + TauZero * 2;
                _taggedInhibitionWindow = DeltaExcitatory + DeltaInhibitory + TauZero * 2;

                SearchLevel = Math.Min(charge / 6, MaxSearchLevel);

                if (IsTagged)
                    messages = NeighbourExcitatoryMessageBurst(currentTick, charge + 1, SearchLevel)
                        .Concat(GlobalInhibitoryMessageBurst(currentTick, charge))
                        .Concat(
                            NeighbourExcitatoryMessageBurst(currentTick, charge, SearchLevel - 1)
                        )
                        .ToArray();
                else
                {
                    messages = NeighbourExcitatoryMessageBurst(currentTick, charge + 1, SearchLevel)
                        .ToArray();
                }

                break;
            case NodeState.Refractory:
                messages = NoAction();
                break;
            case NodeState.Processing:
                messages = NoAction();
                break;
            case NodeState.Inhibited:
                // Tag node if it was excited before expected
                if (_taggedExcitationWindow < TauZero && _taggedExcitationWindow > 0)
                {
                    //if (SearchLevel == 0)
                    TagNode(currentTick);
                    //SearchLevel = 0;
                    //SearchLevel = Math.Max(0, SearchLevel - 1);
                }

                messages = NoAction();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (messages.Length > 0)
        {
            State = NodeState.Processing;
            PostEvent(NodeEventType.Processing, currentTick);
        }

        return messages;
    }

    /// <summary>
    /// Excite the node, and return a list of messages to send to other nodes as a result of the excitation
    /// </summary>
    /// <param name="currentTick"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public StratiumProcessingMessage[] Inhibit(long currentTick)
    {
        if (IsTagged)
            return NoAction();

        if (_taggedInhibitionWindow == 0)
        {
            _taggedExcitationWindow = 0;
        }

        switch (State)
        {
            case NodeState.Neutral:
                State = NodeState.Inhibited;
                PostEvent(NodeEventType.Inhibited, currentTick);
                return NoAction();
            case NodeState.Refractory:
                // Disable the refractory period
                _refractoryCounter = 0;
                State = NodeState.Inhibited;
                PostEvent(NodeEventType.Inhibited, currentTick);
                return NoAction();
            case NodeState.Processing:
                State = NodeState.Inhibited;
                PostEvent(NodeEventType.Inhibited, currentTick);
                return NoAction();
            case NodeState.Inhibited:
                return NoAction();
            default:
                return NoAction();
        }
    }

    public StratiumProcessingMessage[] InitiateWave(long currentTick)
    {
        switch (State)
        {
            case NodeState.Neutral:
                _taggedExcitationWindow = DeltaExcitatory * 2 + TauZero * 2;
                _taggedInhibitionWindow = DeltaExcitatory + DeltaInhibitory + TauZero * 2;

                State = NodeState.Processing;
                PostEvent(NodeEventType.Processing, currentTick);

                return NeighbourExcitatoryMessageBurst(currentTick, 0, 0).ToArray();
            case NodeState.Refractory:
                return NoAction();
            case NodeState.Processing:
                return NoAction();
            case NodeState.Inhibited:
                return NoAction();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Node will enter a tagged state with reduced <see cref="Tau"/> (processing time)
    /// </summary>
    public void TagNode(long currentTick)
    {
        IsTagged = true;
        Tau = TauPlus;
        PostEvent(NodeEventType.Tagged, currentTick);
    }

    /// <summary>
    /// Sends excitatory messages to all neighbours for a given level
    /// </summary>
    /// <param name="currentTick"></param>
    /// <param name="maxLevel">Maximum edge level to send the message(s) on</param>
    public IEnumerable<StratiumProcessingMessage> NeighbourExcitatoryMessageBurst(
        long currentTick,
        int charge,
        int level
    )
    {
        var processingTime = Tau;
        var executionTime = DeltaExcitatory;

        var processAt = currentTick + processingTime;
        var executeAt = processAt + executionTime;

        var edges = NeighbouringEdges
            .Where(e => e.Level == level)
            .Select(e => new StratiumProcessingMessage()
            {
                ReceiveAt = processAt,
                SentAt = currentTick,
                SendMessage = new StratiumNodeMessage
                {
                    Type = StratiumNodeMessage.MessageType.Excitatory,
                    SourceEdge = e,
                    Receiver = e.GetOtherNode(this),
                    SentAt = processAt,
                    ReceiveAt = executeAt,
                    Charge = charge,
                },
            });

        return edges;
    }

    /// <summary>
    /// Generates inhibitory messages to be sent to all neighbors
    /// </summary>
    /// <returns>List of process messages containing exicitatory messages</returns>
    public IEnumerable<StratiumProcessingMessage> GlobalInhibitoryMessageBurst(
        long currentTick,
        int charge = 0
    )
    {
        var processingTime = Tau;
        var executionTime = DeltaInhibitory;

        var processAt = currentTick + processingTime;
        var executeAt = processAt + executionTime;

        return AllNodes.Select(n => new StratiumProcessingMessage()
        {
            ReceiveAt = processAt,
            SentAt = currentTick,
            SendMessage = new StratiumNodeMessage
            {
                Type = StratiumNodeMessage.MessageType.Inhibitory,
                SourceEdge = null,
                Receiver = n,
                SentAt = processAt,
                ReceiveAt = executeAt,
                Charge = charge,
            },
        });
    }

    /// <summary>
    /// Generates inhibitory messages to be sent to all neighbors
    /// </summary>
    /// <returns>List of process messages containing exicitatory messages</returns>
    private IEnumerable<StratiumProcessingMessage> NeighbourInhibitoryMessageBurst(
        long currentTick,
        int charge = 0
    )
    {
        var processingTime = Tau;
        var executionTime = DeltaInhibitory;

        var processAt = currentTick + processingTime;
        var executeAt = processAt + executionTime;

        return NeighbouringEdges.Select(e => new StratiumProcessingMessage()
        {
            ReceiveAt = processAt,
            SentAt = currentTick,
            SendMessage = new StratiumNodeMessage
            {
                Type = StratiumNodeMessage.MessageType.Inhibitory,
                SourceEdge = e,
                Receiver = e.GetOtherNode(this),
                SentAt = processAt,
                ReceiveAt = executeAt,
                Charge = charge,
            },
        });
    }

    /// <summary>
    /// Generates inhibitory messages to be sent to all neighbors
    /// </summary>
    /// <returns>List of process messages containing exicitatory messages</returns>
    private IEnumerable<StratiumProcessingMessage> NeighbourInhibitoryMessageBurst(
        long currentTick,
        int level,
        int charge
    )
    {
        var processingTime = Tau;
        var executionTime = DeltaInhibitory;

        var processAt = currentTick + processingTime;
        var executeAt = processAt + executionTime;

        return NeighbouringEdges
            .Where(e => e.Level == level)
            .Select(e => new StratiumProcessingMessage()
            {
                ReceiveAt = processAt,
                SentAt = currentTick,
                SendMessage = new StratiumNodeMessage
                {
                    Type = StratiumNodeMessage.MessageType.Inhibitory,
                    SourceEdge = e,
                    Receiver = e.GetOtherNode(this),
                    SentAt = processAt,
                    ReceiveAt = executeAt,
                    Charge = charge,
                },
            });
    }

    /// <summary>
    /// Return empty message array, to be used when no action is needed
    /// </summary>
    /// <returns></returns>
    private StratiumProcessingMessage[] NoAction() => [];

    /// <summary>
    /// Disinhibit a inhibitied node. Function will be ignored if node is not inhibited.
    /// </summary>
    public void DisinhibitNode(long currentTick)
    {
        if (State == NodeState.Inhibited)
        {
            State = NodeState.Neutral;
            PostEvent(NodeEventType.Neutral, currentTick);
        }
    }

    protected bool Equals(StratiumNode other)
    {
        return NodeId == other.NodeId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((StratiumNode)obj);
    }

    public override int GetHashCode()
    {
        return NodeId;
    }

    public override string ToString()
    {
        return $"[{nameof(NodeId)}: {NodeId}, {nameof(Tau)}: {Tau}, {nameof(IsTagged)}: {IsTagged}, {nameof(State)}: {State}]";
    }

    /// <summary>
    /// Post event update for the node to the current <see cref="AlgorithmEventHandler"/>
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="atTick"></param>
    private void PostEvent(NodeEventType eventType, long atTick)
    {
        PostEvent(
            new NodeEvent
            {
                EventType = eventType,
                NodeId = NodeId,
                Tick = atTick,
            }
        );
    }

    public AlgorithmEventHandler? EventHandler { get; set; }

    public void PostEvent(AlgorithmEvent algEvent)
    {
        EventHandler?.PostEvent(algEvent);
    }
}
