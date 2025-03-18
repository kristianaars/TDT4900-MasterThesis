using TDT4900_MasterThesis.Helper;

namespace TDT4900_MasterThesis.Algorithm.Component;

public class AlgorithmGraph<TNode, TEdge>
    where TNode : AlgorithmNode
    where TEdge : AlgorithmEdge
{
    public List<TNode> Nodes { get; set; }
    public List<TEdge> Edges { get; set; }
    public bool IsDirected { get; set; }

    private TEdge?[,]? _adjecencyMatrix;

    public void Initialize()
    {
        _adjecencyMatrix = BuildAdjecencyMatrix();
    }

    private TEdge?[,] BuildAdjecencyMatrix()
    {
        var matrixSize = Nodes.Max(n => n.NodeId) + 1;

        TEdge?[,] matrix = new TEdge[matrixSize, matrixSize];
        ArrayHelper.FillArray(matrix, null); //Fill all fields with null

        Edges.ForEach(e =>
        {
            matrix[e.SourceNodeId, e.TargetNodeId] = e;
            if (!IsDirected)
            {
                matrix[e.TargetNodeId, e.SourceNodeId] = e;
            }
        });

        return matrix;
    }

    public IEnumerable<TEdge> GetOutEdges(TNode node)
    {
        var outEdges = new List<TEdge>();

        for (var i = 0; i < _adjecencyMatrix!.GetLength(0); i++)
        {
            var e = _adjecencyMatrix[node.NodeId, i];
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

    private TNode GetNodeByNodeId(int nodeId) => Nodes.Find(n => n.NodeId == nodeId)!;

    public IEnumerable<TEdge> GetInEdges(TNode node)
    {
        var inEdges = new List<TEdge>();

        for (var i = 0; i < _adjecencyMatrix!.GetLength(0); i++)
        {
            var e = _adjecencyMatrix[i, node.NodeId];
            if (e != null)
                inEdges.Add(e);
        }

        return inEdges;
    }

    /// <summary>
    /// Returns the subgraph containing tagged nodes and their edges
    /// </summary>
    public AlgorithmGraph<AlgorithmNode, AlgorithmEdge> GetTaggedSubgraph()
    {
        var taggedNodes = Nodes.Where(n => n.IsTagged).ToList();
        var taggedEdges = taggedNodes
            .SelectMany(n =>
                GetOutEdges(n)
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
