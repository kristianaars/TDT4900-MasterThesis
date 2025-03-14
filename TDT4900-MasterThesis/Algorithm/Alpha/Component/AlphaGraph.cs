using System.Collections;
using AutoMapper;
using TDT4900_MasterThesis.Helper;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Alpha.Component;

[AutoMap(typeof(Graph))]
public class AlphaGraph
{
    public List<AlphaNode> Nodes { get; set; }
    public List<AlphaEdge> Edges { get; set; }
    public bool IsDirected { get; set; }

    private AlphaEdge?[,]? _adjecencyMatrix;

    /// <summary>
    /// Configure fields of Nodes and Edges as well as the backing adjecency matrix.
    /// </summary>
    public void Initialize(AlphaAlgorithmSpec algorithmSpec)
    {
        _adjecencyMatrix = BuildAdjecencyMatrix();

        // Initialize all nodes
        Nodes.ForEach(n =>
        {
            n.Neighbours = GetOutNeighbours(n).ToList();
            n.AllNodes = Nodes;
            n.RefractoryPeriod = algorithmSpec.RefractoryPeriod;
            n.DeltaExcitatory = algorithmSpec.DeltaTExcitatory;
            n.DeltaInhibitory = algorithmSpec.DeltaTInhibitory;
            n.TauZero = algorithmSpec.TauZero;
            n.TauPlus = algorithmSpec.TauPlus;
            n.Tau = n.TauZero;
        });
    }

    private AlphaEdge?[,] BuildAdjecencyMatrix()
    {
        AlphaEdge?[,] matrix = new AlphaEdge[Nodes.Count, Nodes.Count];
        ArrayHelper.FillArray(matrix, null); //Fill all fields with null

        Edges.ForEach(e =>
        {
            matrix[e.SourceId, e.TargetId] = e;
            if (!IsDirected)
            {
                matrix[e.TargetId, e.SourceId] = e;
            }
        });

        return matrix;
    }

    public IEnumerable<AlphaEdge> GetOutEdges(AlphaNode node)
    {
        var outEdges = new List<AlphaEdge>();

        for (var i = 0; i < _adjecencyMatrix!.GetLength(0); i++)
        {
            var e = _adjecencyMatrix[node.NodeId, i];
            if (e != null)
                outEdges.Add(e);
        }

        return outEdges;
    }

    public IEnumerable<AlphaNode> GetOutNeighbours(AlphaNode node)
    {
        return GetOutEdges(node)
            .Select(e => Nodes[e.TargetId == node.NodeId ? e.SourceId : e.TargetId]);
    }

    public IEnumerable<AlphaEdge> GetInEdges(AlphaNode node)
    {
        var inEdges = new List<AlphaEdge>();

        for (var i = 0; i < _adjecencyMatrix!.GetLength(0); i++)
        {
            var e = _adjecencyMatrix[i, node.NodeId];
            if (e != null)
                inEdges.Add(e);
        }

        return inEdges;
    }
}
