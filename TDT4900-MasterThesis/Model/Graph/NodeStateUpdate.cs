namespace TDT4900_MasterThesis.Model.Graph;

/// <summary>
/// Represents the update of the state of a node at the given tick
/// </summary>
public class NodeStateUpdate
{
    public int NodeId { get; set; }
    public bool IsTagged { get; set; }
    public NodeState State { get; set; }
    public long Tick { get; set; }

    public NodeStateUpdate(int nodeId, NodeState state, bool isTagged, long tick)
    {
        NodeId = nodeId;
        State = state;
        IsTagged = isTagged;
        Tick = tick;
    }
}
