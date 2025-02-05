namespace TDT4900_MasterThesis.model.graph;

public class Node(int id) : MIConvexHull.IVertex
{
    public int Id { get; } = id;

    public int X { get; set; }
    public int Y { get; set; }
    public double[] Position => [X, Y];

    public NodeState State { get; set; } = NodeState.Neutral;

    public bool Equals(Node other)
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
        return Equals((Node)obj);
    }

    public override int GetHashCode()
    {
        return Id;
    }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}";
    }
}

public enum NodeState
{
    Neutral,
    Cooldown,
    Processing,
    Inhibited,
    Tagged,
}
