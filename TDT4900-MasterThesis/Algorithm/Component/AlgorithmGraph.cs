using TDT4900_MasterThesis.Helper;

namespace TDT4900_MasterThesis.Algorithm.Component;

public class AlgorithmGraph<TNode, TEdge>
    where TNode : AlgorithmNode
    where TEdge : AlgorithmEdge
{
    public List<TNode> Nodes { get; set; }
    public List<TEdge> Edges { get; set; }
    public bool IsDirected { get; set; }

    public int Levels => Edges.Max(e => e.Level) + 1;

    private TEdge?[,,]? _adjecencyMatrix;

    public void Initialize()
    {
        _adjecencyMatrix = BuildAdjecencyMatrix();
    }

    private TEdge?[,,] BuildAdjecencyMatrix()
    {
        var matrixSize = Nodes.Max(n => n.NodeId) + 1;

        TEdge?[,,] matrix = new TEdge[matrixSize, matrixSize, Levels];
        ArrayHelper.FillArray(matrix, null); //Fill all fields with null

        Edges.ForEach(e =>
        {
            matrix[e.SourceNodeId, e.TargetNodeId, e.Level] = e;
            if (!IsDirected)
            {
                matrix[e.TargetNodeId, e.SourceNodeId, e.Level] = e;
            }
        });

        return matrix;
    }

    /// <summary>
    /// Returns outward Edges for all levels for a given node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public IEnumerable<TEdge> GetOutEdges(TNode node)
    {
        var outEdges = new List<TEdge>();

        for (var i = 0; i < _adjecencyMatrix!.GetLength(0); i++)
        {
            for (var l = 0; l < _adjecencyMatrix!.GetLength(2); l++)
            {
                var e = _adjecencyMatrix[node.NodeId, i, l];
                if (e != null)
                    outEdges.Add(e);
            }
        }

        return outEdges;
    }

    public IEnumerable<TEdge> GetOutEdges(TNode node, int level)
    {
        var outEdges = new List<TEdge>();

        for (var i = 0; i < _adjecencyMatrix!.GetLength(0); i++)
        {
            var e = _adjecencyMatrix[node.NodeId, i, level];
            if (e != null)
                outEdges.Add(e);
        }

        return outEdges;
    }

    public IEnumerable<TNode> GetOutNeighbours(TNode node)
    {
        return GetOutEdges(node)
            .Select(e =>
                e.TargetNodeId == node.NodeId
                    ? GetNodeByNodeId(e.SourceNodeId)
                    : GetNodeByNodeId(e.TargetNodeId)
            );
    }

    public IEnumerable<TNode> GetOutNeighbours(TNode node, int level)
    {
        return GetOutEdges(node, level)
            .Select(e =>
                e.TargetNodeId == node.NodeId
                    ? GetNodeByNodeId(e.SourceNodeId)
                    : GetNodeByNodeId(e.TargetNodeId)
            );
    }

    private TNode GetNodeByNodeId(int nodeId) => Nodes.Find(n => n.NodeId == nodeId)!;

    public IEnumerable<TEdge> GetInEdges(TNode node)
    {
        var inEdges = new List<TEdge>();

        for (var i = 0; i < _adjecencyMatrix!.GetLength(0); i++)
        {
            for (int l = 0; l < _adjecencyMatrix.GetLength(2); l++)
            {
                var e = _adjecencyMatrix[i, node.NodeId, l];
                if (e != null)
                    inEdges.Add(e);
            }
        }

        return inEdges;
    }

    /// <summary>
    /// Returns the subgraph containing tagged nodes and their edges on the zeroth level.
    /// </summary>
    public AlgorithmGraph<AlgorithmNode, AlgorithmEdge> GetTaggedSubgraph()
    {
        var taggedNodes = Nodes.Where(n => n.IsTagged).ToList();
        var taggedEdges = taggedNodes
            .SelectMany(n =>
                GetOutEdges(n, level: 0)
                    .Where(e => Nodes[e.TargetNodeId].IsTagged && Nodes[e.SourceNodeId].IsTagged)
                    .Cast<AlgorithmEdge>()
            )
            .ToList();

        return new AlgorithmGraph<AlgorithmNode, AlgorithmEdge>()
        {
            Nodes = taggedNodes.Cast<AlgorithmNode>().ToList(),
            Edges = taggedEdges,
            IsDirected = IsDirected,
        };
    }
}
