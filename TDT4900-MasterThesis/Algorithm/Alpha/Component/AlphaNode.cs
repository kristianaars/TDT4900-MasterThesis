using AutoMapper;
using Serilog;
using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Model.Graph;

namespace TDT4900_MasterThesis.Algorithm.Alpha.Component;

[AutoMap(typeof(Node))]
public class AlphaNode : BaseEventProducer
{
    public int NodeId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public List<AlphaNode> Neighbours { get; set; } = new();
    public List<AlphaNode> AllNodes { get; set; } = new();

    /// <summary>
    /// Tau is the processing time, e.g. how long a message takes to process before sending it to the next node.
    /// </summary>
    public int Tau { get; set; }

    /// <summary>
    /// Marks if the node is tagged or not. Tagged nodes have a shorter processing time <see cref="Tau"/>.
    /// </summary>
    public bool IsTagged { get; set; }

    /// <summary>
    /// Current state of the node
    /// </summary>
    public NodeState State { get; set; }

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
                PostEvent(EventType.Neutral, currentTick);
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
            PostEvent(EventType.Refractory, currentTick);
        }
    }

    /// <summary>
    /// Excite the node, and return a list of messages to send to other nodes as a result of the excitation
    /// </summary>
    /// <param name="currentTick"></param>
    /// <returns></returns>
    public AlphaNodeMessage[] Excite(long currentTick)
    {
        switch (State)
        {
            case NodeState.Neutral:
                _taggedExcitationWindow = DeltaExcitatory * 2 + TauZero * 2;
                _taggedInhibitionWindow = DeltaExcitatory + DeltaInhibitory + TauZero * 2;

                State = NodeState.Processing;
                PostEvent(EventType.Processing, currentTick);

                return NeighbourExcitatoryMessageBurst(currentTick)
                    // Add an inhibitory message burst if the node is tagged
                    .Concat(IsTagged ? GlobalInhibitoryMessageBurst(currentTick) : [])
                    .ToArray();
            case NodeState.Refractory:
                return NoAction();
            case NodeState.Processing:
                return NoAction();
            case NodeState.Inhibited:
                // Tag node if it was excited before expected
                if (_taggedExcitationWindow < TauZero && _taggedExcitationWindow > 0)
                {
                    TagNode(currentTick);
                }

                return NoAction();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Excite the node, and return a list of messages to send to other nodes as a result of the excitation
    /// </summary>
    /// <param name="currentTick"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public AlphaNodeMessage[] Inhibit(long currentTick)
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
                PostEvent(EventType.Inhibited, currentTick);
                return NoAction();
            case NodeState.Refractory:
                // Disable the refractory period
                _refractoryCounter = 0;
                State = NodeState.Inhibited;
                PostEvent(EventType.Inhibited, currentTick);
                return NoAction();
            case NodeState.Processing:
                State = NodeState.Inhibited;
                PostEvent(EventType.Inhibited, currentTick);
                return NoAction();
            case NodeState.Inhibited:
                return NoAction();
            default:
                return NoAction();
        }
    }

    /// <summary>
    /// Node will enter a tagged state with reduced <see cref="Tau"/> (processing time)
    /// </summary>
    public void TagNode(long currentTick)
    {
        IsTagged = true;
        Tau = TauPlus;
        PostEvent(EventType.Tagged, currentTick);
    }

    /// <summary>
    /// Generates exitatory messages to be sent to all neighbors
    /// </summary>
    /// <returns>List of process messages containing exicitatory messages</returns>
    private IEnumerable<AlphaNodeMessage> NeighbourExcitatoryMessageBurst(long currentTick)
    {
        var sendAt = currentTick + Tau;

        return Neighbours.Select(n => new AlphaNodeMessage
        {
            Type = AlphaNodeMessage.MessageType.Excitatory,
            Sender = this,
            Receiver = n,
            SentAt = sendAt,
            ReceiveAt = sendAt + DeltaExcitatory,
        });
    }

    /// <summary>
    /// Generates inhibitory messages to be sent to all neighbors
    /// </summary>
    /// <returns>List of process messages containing exicitatory messages</returns>
    public IEnumerable<AlphaNodeMessage> GlobalInhibitoryMessageBurst(long currentTick)
    {
        var sendAt = currentTick + Tau;

        return AllNodes.Select(n => new AlphaNodeMessage()
        {
            Type = AlphaNodeMessage.MessageType.Inhibitory,
            Sender = this,
            Receiver = n,
            SentAt = sendAt,
            ReceiveAt = sendAt + DeltaInhibitory,
        });
    }

    /// <summary>
    /// Generates inhibitory messages to be sent to all neighbors
    /// </summary>
    /// <returns>List of process messages containing exicitatory messages</returns>
    private IEnumerable<AlphaNodeMessage> NeighbourInhibitoryMessageBurst(long currentTick)
    {
        var sendAt = currentTick + Tau;

        return Neighbours.Select(n => new AlphaNodeMessage()
        {
            Type = AlphaNodeMessage.MessageType.Inhibitory,
            Sender = this,
            Receiver = n,
            SentAt = sendAt,
            ReceiveAt = sendAt + DeltaInhibitory,
        });
    }

    /// <summary>
    /// Return empty message array, to be used when no action is needed
    /// </summary>
    /// <returns></returns>
    private AlphaNodeMessage[] NoAction() => [];

    /// <summary>
    /// Disinhibit a inhibitied node. Function will be ignored if node is not inhibited.
    /// </summary>
    public void DisinhibitNode(long currentTick)
    {
        if (State == NodeState.Inhibited)
        {
            State = NodeState.Neutral;
            PostEvent(EventType.Neutral, currentTick);
        }
    }

    protected bool Equals(AlphaNode other)
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
        return Equals((AlphaNode)obj);
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
    /// Post event update for the node to the current <see cref="Handler.EventHandler"/>
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="atTick"></param>
    private void PostEvent(EventType eventType, long atTick)
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
}
