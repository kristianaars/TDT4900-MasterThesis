namespace TDT4900_MasterThesis.model.graph;

public class Node(int id)
{
    public int Id { get; } = id;

    protected bool Equals(Node other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Node)obj);
    }

    public override int GetHashCode()
    {
        return Id;
    }
}