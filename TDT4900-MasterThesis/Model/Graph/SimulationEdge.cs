namespace TDT4900_MasterThesis.Model.Graph;

public class SimulationEdge(SimulationNode source, SimulationNode target, int weight)
{
    public SimulationNode Source { get; } = source;
    public SimulationNode Target { get; } = target;
    public int Weight { get; } = weight;

    protected bool Equals(SimulationEdge other)
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
        return Equals((SimulationEdge)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Source, Target);
    }
}
