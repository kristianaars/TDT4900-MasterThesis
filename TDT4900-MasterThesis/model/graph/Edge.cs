namespace TDT4900_MasterThesis.model.graph;

public class Edge(Node source, Node target, int weight)
{
    public Node Source { get; } = source;
    public Node Target { get; } = target;
    public int Weight { get; } = weight;

    protected bool Equals(Edge other)
    {
        return Source.Equals(other.Source) && Target.Equals(other.Target);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((Edge)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Source, Target);
    }
}
