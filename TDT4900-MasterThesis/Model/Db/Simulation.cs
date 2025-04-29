namespace TDT4900_MasterThesis.Model.Db;

/// <summary>
/// Simulation represents a single simulation of shortest path algorithm on a graph.
/// It contains both specification of the graph and the algorithm to be used in the simulation, as well as the results of the simulation.
/// </summary>
public class Simulation : BaseModel
{
    public int SimulationBatchId { get; set; }

    /// <summary>
    /// Graph used in the simulation. Generated based on <see cref="GraphSpec"/>.
    /// </summary>
    public Graph? Graph { get; set; }

    public Node? StartNode { get; set; }

    public Node? TargetNode { get; set; }

    public List<Node>? AlgorithmTaggedNodes { get; set; }

    public List<Node>? ShortestPathTaggedNodes { get; set; }

    /// <summary>
    /// Length of the shortest path found by the simulation-algorithm. -1 if not yet calculated.
    /// </summary>
    public int AlgorithmShortestPathLength { get; set; } = -1;

    /// <summary>
    /// Length of the shortest path found by Dijkstra's algorithm. -1 if not yet calculated.
    /// </summary>
    public int ShortestPathLength { get; set; } = -1;

    public List<NodeEvent>? EventHistory { get; set; }

    public bool Success { get; set; }
}
