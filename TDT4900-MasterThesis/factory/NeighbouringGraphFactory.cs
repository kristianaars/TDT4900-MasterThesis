using TDT4900_MasterThesis.model.graph;

namespace TDT4900_MasterThesis.factory;

/// <summary>
/// Produces a <see cref="Graph"/> with evenly spaced nodes, where each node is connected to its neighbouring nodes
/// within a certain radius.
/// </summary>
public class NeighbouringGraphFactory
{
    private readonly int _nodeCount;
    private readonly double _nodeSpacing;
    private readonly double _edgeRadius;
    private readonly int _noise;

    /// <summary>
    /// Initializes a new instance of the <see cref="NeighbouringGraphFactory"/> class.
    /// </summary>
    /// <param name="nodeCount">The number of nodes in the graph</param>
    /// <param name="nodeSpacing">The spacing between nodes</param>
    /// <param name="edgeRadius">The radius within which nodes are considered neighbours</param>
    public NeighbouringGraphFactory(int nodeCount, double nodeSpacing, double edgeRadius, int noise)
    {
        _nodeCount = nodeCount;
        _nodeSpacing = nodeSpacing;
        _edgeRadius = edgeRadius;
        _noise = noise;
    }

    /// <summary>
    /// Produces a <see cref="Graph"/> with evenly spaced nodes, where each node is connected to its neighbouring nodes
    /// within a certain radius.
    /// </summary>
    /// <returns>A new instance of <see cref="Graph"/></returns>
    public Graph GetGraph()
    {
        var random = new Random();
        var nodes = new List<Node>();
        var edges = new List<Edge>();

        // Number of nodes in horizontal and vertical direction
        var h = (int)Math.Ceiling(Math.Sqrt(_nodeCount));
        var v = h;

        for (var i = 0; i < h; i++)
        {
            for (var j = 0; j < v; j++)
            {
                if (nodes.Count < _nodeCount)
                {
                    nodes.Add(
                        new Node(nodes.Count)
                        {
                            X = (int)(i * _nodeSpacing) + random.Next(-_noise, _noise),
                            Y = (int)(j * _nodeSpacing) + random.Next(-_noise, _noise),
                        }
                    );
                }
            }
        }

        for (int i = 0; i < _nodeCount - 1; i++)
        {
            for (int j = i + 1; j < _nodeCount; j++)
            {
                double distance = Distance(nodes[i], nodes[j]);
                if (distance <= _edgeRadius)
                {
                    edges.Add(new Edge(nodes[i], nodes[j], 1));
                }
            }
        }

        var graph = new Graph(nodes.ToArray(), edges.ToArray());
        graph.ConvertToBidirectional();

        graph.Nodes.ForEach(n =>
        {
            n.Neighbours = graph.GetOutEdges(n).Select(e => e.Target).ToArray();
            n.AllNodes = graph.Nodes.ToArray();
        });

        return graph;
    }

    private double Distance(Node a, Node b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return System.Math.Sqrt(dx * dx + dy * dy);
    }
}
