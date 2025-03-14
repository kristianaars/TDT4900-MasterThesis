using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Factory;

public class GraphFactory
{
    public Graph CreateGraph(GraphSpec spec)
    {
        return spec switch
        {
            NeighboringGraphSpec n => new NeighbouringGraphFactory(
                n.NodeCount,
                n.Distance,
                n.Radius,
                n.Noise
            ).GetGraph(),
            _ => throw new ArgumentException("Unknown graph spec"),
        };
    }
}
