using System.Text.Json.Serialization;
using TDT4900_MasterThesis.Helper;

namespace TDT4900_MasterThesis.Model.Graph;

public class SimulationGraph(SimulationNode[] nodes, SimulationEdge[] edges)
{
    /// <summary>
    /// Index by [source, target] to get the weight of the edge between source and target.
    /// A weight of -1 means there is no edge between the nodes.
    /// </summary>
    private readonly int[,] _adjacencyMatrix = BuildAdjacencyMatrix(nodes, edges);
    private readonly SimulationNode[] _nodes = nodes;

    public List<SimulationNode> Nodes => _nodes.ToList();

    public List<SimulationEdge> Edges => GetAllEdges();

    private static int[,] BuildAdjacencyMatrix(SimulationNode[] nodes, SimulationEdge[] edges)
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
    /// <param name="simulationNode">Node to receive outward edges from</param>
    /// <returns>Array of outward edges for node</returns>
    public SimulationEdge[] GetOutEdges(SimulationNode simulationNode)
    {
        var outEdges = new List<SimulationEdge>();

        for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
        {
            var w = _adjacencyMatrix[simulationNode.Id, i];
            if (w > 0)
                outEdges.Add(new SimulationEdge(simulationNode, _nodes[i], w));
        }

        return outEdges.ToArray();
    }

    /// <summary>
    /// Get all edges in graph where node is the target of the edge.
    /// </summary>
    /// <param name="simulationNode">Node to receive inward edges from</param>
    /// <returns>Array of inward edges for node</returns>
    public SimulationEdge[] GetInEdges(SimulationNode simulationNode)
    {
        var inEdges = new List<SimulationEdge>();

        for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
        {
            var w = _adjacencyMatrix[i, simulationNode.Id];
            if (w > 0)
                inEdges.Add(new SimulationEdge(_nodes[i], simulationNode, w));
        }

        return inEdges.ToArray();
    }

    /// <summary>
    /// Sets the weight for an edge in the graph. This method will only update the adjacency matrix, and a new edge
    /// object will be returned.
    /// </summary>
    /// <param name="simulationEdge">Edge to edit weight-value for</param>
    /// <param name="weight">The new weight</param>
    /// <returns><see cref="SimulationEdge"/> with updated weight.</returns>
    public SimulationEdge SetEdgeWeight(SimulationEdge simulationEdge, int weight)
    {
        _adjacencyMatrix[simulationEdge.Source.Id, simulationEdge.Target.Id] = weight;

        return new SimulationEdge(simulationEdge.Source, simulationEdge.Target, weight);
    }

    public List<SimulationEdge> GetAllEdges()
    {
        var allEdges = new List<SimulationEdge>();

        for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < _adjacencyMatrix.GetLength(1); j++)
            {
                if (_adjacencyMatrix[i, j] >= 0)
                    allEdges.Add(new SimulationEdge(_nodes[i], _nodes[j], _adjacencyMatrix[i, j]));
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
