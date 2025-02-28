using TDT4900_MasterThesis.helpers;
using TDT4900_MasterThesis.model.simulation;

namespace TDT4900_MasterThesis.model.graph;

public class Graph(Node[] nodes, Edge[] edges)
{
    /// <summary>
    /// Index by [source, target] to get the weight of the edge between source and target.
    /// A weight of -1 means there is no edge between the nodes.
    /// </summary>
    private readonly int[,] _adjacencyMatrix = BuildAdjacencyMatrix(nodes, edges);
    private readonly Node[] _nodes = nodes;

    public List<Node> Nodes => _nodes.ToList();
    public List<Edge> Edges => GetAllEdges();

    private static int[,] BuildAdjacencyMatrix(Node[] nodes, Edge[] edges)
    {
        int[,] adjacencyMatrix = new int[nodes.Length, nodes.Length];
        ArrayHelper.FillArray(adjacencyMatrix, -1);

        foreach (var e in edges)
            adjacencyMatrix[e.Source.Id, e.Target.Id] = e.Weight;

        return adjacencyMatrix;
    }

    /// <summary>
    /// Get all edges in graph where node is the source of the edge.
    /// </summary>
    /// <param name="node">Node to receive outward edges from</param>
    /// <returns>Array of outward edges for node</returns>
    public Edge[] GetOutEdges(Node node)
    {
        var outEdges = new List<Edge>();

        for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
        {
            var w = _adjacencyMatrix[node.Id, i];
            if (w > 0)
                outEdges.Add(new Edge(node, _nodes[i], w));
        }

        return outEdges.ToArray();
    }

    /// <summary>
    /// Get all edges in graph where node is the target of the edge.
    /// </summary>
    /// <param name="node">Node to receive inward edges from</param>
    /// <returns>Array of inward edges for node</returns>
    public Edge[] GetInEdges(Node node)
    {
        var inEdges = new List<Edge>();

        for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
        {
            var w = _adjacencyMatrix[i, node.Id];
            if (w > 0)
                inEdges.Add(new Edge(_nodes[i], node, w));
        }

        return inEdges.ToArray();
    }

    /// <summary>
    /// Sets the weight for an edge in the graph. This method will only update the adjacency matrix, and a new edge
    /// object will be returned.
    /// </summary>
    /// <param name="edge">Edge to edit weight-value for</param>
    /// <param name="weight">The new weight</param>
    /// <returns><see cref="Edge"/> with updated weight.</returns>
    public Edge SetEdgeWeight(Edge edge, int weight)
    {
        _adjacencyMatrix[edge.Source.Id, edge.Target.Id] = weight;

        return new Edge(edge.Source, edge.Target, weight);
    }

    public List<Edge> GetAllEdges()
    {
        var allEdges = new List<Edge>();

        for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < _adjacencyMatrix.GetLength(1); j++)
            {
                if (_adjacencyMatrix[i, j] >= 0)
                    allEdges.Add(new Edge(_nodes[i], _nodes[j], _adjacencyMatrix[i, j]));
            }
        }

        return allEdges;
    }

    public void ConvertToBidirectional()
    {
        for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < _adjacencyMatrix.GetLength(1); j++)
            {
                if (_adjacencyMatrix[i, j] >= 0)
                    _adjacencyMatrix[j, i] = _adjacencyMatrix[i, j];
            }
        }
    }
}
