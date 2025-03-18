using AutoMapper;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Component;

[AutoMap(typeof(Node))]
public class AlgorithmNode : IEquatable<AlgorithmNode>
{
    public int NodeId { get; set; }

    /// <summary>
    /// Marks if the node is tagged or not. A node is tagged if it is on a shortest path.
    /// </summary>
    public bool IsTagged { get; set; }

    public bool Equals(AlgorithmNode? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
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
        return Equals((AlgorithmNode)obj);
    }

    public override int GetHashCode()
    {
        return NodeId;
    }
}
