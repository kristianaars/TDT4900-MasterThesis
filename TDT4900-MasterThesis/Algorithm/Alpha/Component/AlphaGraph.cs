using AutoMapper;
using TDT4900_MasterThesis.Helper;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Alpha.Component;

[AutoMap(typeof(Graph))]
public class AlphaGraph
{
    public List<AlphaNode> Nodes { get; set; }
    public List<AlphaEdge> Edges { get; set; }

    private AlphaEdge?[,]? _adjecencyMatrix;

    /// <summary>
    /// Configure fields of Nodes and Edges as well as the backing adjecency matrix.
    /// </summary>
    public void Initialize(AlphaAlgorithmSpec algorithmSpec)
    {
        // Fix edges after mapping. Caused by a degree of over-engineering...
        Edges.ForEach(e =>
        {
            e.Target = Nodes.Find(n => Equals(n, e.Target))!;
            e.Source = Nodes.Find(n => Equals(n, e.Source))!;
        });

        _adjecencyMatrix = BuildAdjecencyMatrix();

        // Initialize all nodes
        Nodes.ForEach(n =>
        {
            n.Neighbours = GetOutEdges(n).Select(e => e.Target).ToList()!;
            n.AllNodes = Nodes;
            n.RefractoryPeriod = algorithmSpec.RefractoryPeriod;
            n.DeltaExcitatory = algorithmSpec.DeltaTExcitatory;
            n.DeltaInhibitory = algorithmSpec.DeltaTInhibitory;
            n.TauZero = algorithmSpec.TauZero;
        });
    }

    private AlphaEdge?[,] BuildAdjecencyMatrix()
    {
        AlphaEdge?[,] matrix = new AlphaEdge[Nodes.Count, Nodes.Count];
        ArrayHelper.FillArray(matrix, null); //Fill all fields with null

        Edges.ForEach(e =>
        {
            matrix[e.Source.NodeId, e.Target.NodeId] = e;
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
