using TDT4900_MasterThesis.Helper;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Model.Graph;

namespace TDT4900_MasterThesis.Factory.GraphFactory;

/// <summary>
/// Produces a <see cref="SimulationGraph"/> with evenly spaced nodes, where each node is connected to its neighbouring nodes
/// within a certain radius.
/// </summary>
public class RadiusNeighbouringGraphFactory
{
    private readonly int _nodeCount;
    private readonly int _nodeSpacing;
    private readonly double _edgeRadius;
    private readonly int _noise;

    /// <summary>
    /// Initializes a new instance of the <see cref="RadiusNeighbouringGraphFactory"/> class.
    /// </summary>
    /// <param name="nodeCount">The number of nodes in the graph</param>
    /// <param name="nodeSpacing">The spacing between nodes</param>
    /// <param name="edgeRadius">The radius within which nodes are considered neighbours</param>
    /// <param name="noise">The noise of the node positions</param>
    public RadiusNeighbouringGraphFactory(
        int nodeCount,
        int nodeSpacing,
        double edgeRadius,
        int noise
    )
    {
        _nodeCount = nodeCount;
        _nodeSpacing = nodeSpacing;
        _edgeRadius = edgeRadius;
        _noise = noise;
    }

    /// <summary>
    /// Produces a <see cref="SimulationGraph"/> with evenly spaced nodes, where each node is connected to its neighbouring nodes
    /// within a certain radius.
    /// </summary>
    /// <returns>A new instance of <see cref="SimulationGraph"/></returns>
    public Graph GetGraph()
    {
        var edges = new List<Edge>();
        var nodes = GraphFactoryHelper.ScatterNodes(_nodeCount, _nodeSpacing, _noise);

        // Connect nodes within a certain radius
        var edgeCounter = 0;
        for (int i = 0; i < _nodeCount - 1; i++)
        {
            for (int j = i + 1; j < _nodeCount; j++)
            {
                double distance = GraphFactoryHelper.Distance(nodes[i], nodes[j]);
                if (distance <= _edgeRadius)
                {
                    edges.Add(
                        new Edge()
                        {
                            EdgeId = edgeCounter++,
                            SourceNodeId = nodes[i].NodeId,
                            TargetNodeId = nodes[j].NodeId,
                        }
                    );
                }
            }
        }

        var graph = new Graph()
        {
            Nodes = nodes,
            Edges = edges,
            IsDirected = false,
        };

        return graph;
    }
}
