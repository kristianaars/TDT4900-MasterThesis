using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Factory.GraphFactory;

public class GraphFactory
{
    public Graph CreateGraph(GraphSpec spec)
    {
        return spec switch
        {
            RadiusNeighboringGraphSpec n => new RadiusNeighbouringGraphFactory(
                n.NodeCount,
                n.Distance,
                n.Radius,
                n.Noise
            ).GetGraph(),
            SquareGridHierarchicalGraphSpec n => new SquareGridHierarchicalGraphFactory()
            {
                NodeCount = n.NodeCount,
                Distance = n.Distance,
                Noise = n.Noise,
                BaseGridSize = n.BaseGridSize,
                HierarchicalLevels = n.HierarchicalLevels,
                SingleLineGraph = n.SingleLineGraph,
            }.GetGraph(),
            _ => throw new ArgumentException("Unknown graph spec"),
        };
    }
}
