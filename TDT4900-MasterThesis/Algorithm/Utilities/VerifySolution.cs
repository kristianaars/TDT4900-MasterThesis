using Serilog;
using TDT4900_MasterThesis.Algorithm.Component;

namespace TDT4900_MasterThesis.Algorithm.Utilities;

public class VerifySolution
{
    public required AlgorithmGraph<AlgorithmNode, AlgorithmEdge> GraphTagged { get; set; }

    public required AlgorithmNode StartNode { get; set; }
    public required AlgorithmNode TargetNode { get; set; }

    private List<AlgorithmNode> _wavefront = [];
    private readonly List<AlgorithmNode> _visitedNodes = [];
    private int _currentDistance;

    /// <summary>
    /// Returns the longest distance of the solution using a propogating wavefront from the startnode
    /// </summary>
    /// <returns></returns>
    public int GetSolutionDistance()
    {
        // Initialize the wavefront with the startnode
        _wavefront.Clear();
        _visitedNodes.Clear();
        _currentDistance = 0;
        _wavefront.Add(StartNode);

        var maxDistance = GraphTagged.Nodes.Count;

        if (!GraphTagged.Nodes.Contains(TargetNode))
            Log.Information("Debug stop");

        if (!GraphTagged.Nodes.Contains(StartNode))
            Log.Information("Debug stop");

        while (_wavefront.Count > 0)
        {
            _currentDistance++;
            _visitedNodes.AddRange(_wavefront);
            _wavefront = _wavefront
                .SelectMany(n =>
                {
                    var neighbours = GraphTagged
                        .GetOutNeighbours(n)
                        .Distinct() // Distinct is used to avoid adding the same node multiple times
                        .Where(neighbour => !_visitedNodes.Contains(neighbour))
                        .ToList();
                    _visitedNodes.AddRange(neighbours);
                    return neighbours;
                })
                .ToList();

            if (_currentDistance > maxDistance)
                throw new Exception(
                    $"No solution was found after {_currentDistance} wave propogations"
                );
        }

        if (!_visitedNodes.Contains(TargetNode))
        {
            throw new Exception("The target node was not reached by the wavefront");
        }

        return _currentDistance;
    }
}
