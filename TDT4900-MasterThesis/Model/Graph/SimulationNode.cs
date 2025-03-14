using System.Text.Json.Serialization;
using Serilog;

namespace TDT4900_MasterThesis.Model.Graph;

using MessageType = NodeMessage.MessageType;

public class SimulationNode : MIConvexHull.IVertex
{
    public int Id { get; init; }

    public int X { get; set; }

    public int Y { get; set; }

    [JsonIgnore]
    public double[] Position => [X, Y];

    [JsonIgnore]
    public SimulationNode[] Neighbours = [];

    [JsonIgnore]
    public SimulationNode[] AllNodes = [];

    private Random _random = new Random();

    /// <summary>
    /// Tau is the processing time, e.g. how long a message takes to process before sending it to the next node.
    /// </summary>
    [JsonIgnore]
    public int Tau => IsTagged ? TauPlus : TauZero;

    /// <summary>
    /// Marks if the node is tagged or not. Tagged nodes have a shorter processing time <see cref="Tau"/>.
    /// </summary>
    [JsonIgnore]
    public bool IsTagged { get; set; }

    /// <summary>
    /// Current state of the node
    /// </summary>
    [JsonIgnore]
    public NodeState State { get; set; }

    private int _refractoryCounter;
    private long _taggedExcitationWindow;
    private long _taggedInhibitionWindow;

    [JsonIgnore]
    public int DeltaExcitatory { get; set; }

    [JsonIgnore]
    public int DeltaInhibitory { get; set; }

    [JsonIgnore]
    public int TauZero { get; set; }

    [JsonIgnore]
    public int TauPlus { get; set; }

    [JsonIgnore]
    public int RefractoryPeriod { get; set; }

    public SimulationNode(int id)
    {
        Id = id;
        Reset();
    }

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
            }
        }
    }

    /// <summary>
    /// Reset the state of the node to its neutral initial state. Used when resetting simulation
    /// </summary>
    public void Reset()
    {
        _refractoryCounter = 0;
        _taggedExcitationWindow = 0;
        _taggedInhibitionWindow = 0;
        IsTagged = false;
        State = NodeState.Neutral;
    }

    public void BeginRefraction()
    {
        if (this is { State: NodeState.Processing or NodeState.Refractory })
        {
            _refractoryCounter = RefractoryPeriod;
            State = NodeState.Refractory;
        }
    }

    public ProcessMessage[] Excite(long currentTick)
    {
        switch (State)
        {
            case NodeState.Neutral:
                _taggedExcitationWindow = DeltaExcitatory * 2 + TauZero * 2;
                _taggedInhibitionWindow = DeltaExcitatory + DeltaInhibitory + TauZero * 2;

                State = NodeState.Processing;
                return NeighbourExcitatoryMessageBurst(currentTick)
                    // Add a inhibitory message burst if the node is tagged
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
                    IsTagged = true;
                }

                return NoAction();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public ProcessMessage[] Inhibit(long currentTick)
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
                return NoAction();
            case NodeState.Refractory:
                // Disable the refractory period
                _refractoryCounter = 0;
                State = NodeState.Inhibited;
                return NoAction();
            case NodeState.Processing:
                State = NodeState.Inhibited;
                return NoAction();
            case NodeState.Inhibited:
                return NoAction();
            default:
                return NoAction();
        }
    }

    /// <summary>
    /// Disinhibit a inhibitied node. Function will be ignored if node is not inhibited.
    /// </summary>
    public void DisinhibitNode()
    {
        if (State == NodeState.Inhibited)
        {
            State = NodeState.Neutral;
        }
    }

    /// <summary>
    /// Generates exitatory messages to be sent to all neighbors
    /// </summary>
    /// <returns>List of process messages containing exicitatory messages</returns>
    private ProcessMessage[] NeighbourExcitatoryMessageBurst(long currentTick)
    {
        var sendAt = currentTick + Tau;
        var receiveAt = sendAt + DeltaExcitatory;

        return Neighbours
            .Select(n => new ProcessMessage(
                currentTick,
                sendAt,
                new NodeMessage(sendAt, receiveAt, this, n, MessageType.Excitatory)
            ))
            .ToArray();
    }

    /// <summary>
    /// Generates inhibitory messages to be sent to all neighbors
    /// </summary>
    /// <returns>List of process messages containing exicitatory messages</returns>
    private ProcessMessage[] NeighbourInhibitoryMessageBurst(long currentTick)
    {
        var sendAt = currentTick + Tau;
        var receiveAt = sendAt + DeltaInhibitory;

        return Neighbours
            .Select(n => new ProcessMessage(
                currentTick,
                sendAt,
                new NodeMessage(sendAt, receiveAt, this, n, MessageType.Inhibitory)
            ))
            .ToArray();
    }

    /// <summary>
    /// Generates inhibitory messages to be sent to all neighbors
    /// </summary>
    /// <returns>List of process messages containing exicitatory messages</returns>
    public ProcessMessage[] GlobalInhibitoryMessageBurst(long currentTick)
    {
        var sendAt = currentTick + Tau;
        var receiveAt = sendAt + DeltaInhibitory;

        return AllNodes
            .Select(n => new ProcessMessage(
                currentTick,
                sendAt,
                new NodeMessage(sendAt, receiveAt, this, n, MessageType.Inhibitory)
            ))
            .ToArray();
    }

    /// <summary>
    /// Return empty message array, to be used when no action is needed
    /// </summary>
    /// <returns></returns>
    private ProcessMessage[] NoAction() => [];

    public bool Equals(SimulationNode other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((SimulationNode)obj);
    }

    public override int GetHashCode()
    {
        return Id;
    }

    public override string ToString()
    {
        return $" {nameof(Id)}: {Id}, {nameof(Neighbours)}: \"{String.Join(", ", Neighbours.Select(n => n.Id).ToArray())}\", {nameof(Tau)}: {Tau}, {nameof(IsTagged)}: {IsTagged}, {nameof(State)}: {State}";
    }
}

public enum NodeState
{
    Neutral,
    Refractory,
    Processing,
    Inhibited,
}
