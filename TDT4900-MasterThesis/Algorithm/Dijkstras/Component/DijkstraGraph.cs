using AutoMapper;
using TDT4900_MasterThesis.Helper;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Dijkstras.Component;

[AutoMap(typeof(Graph))]
public class DijkstraGraph
{
    public List<DijkstraNode> Nodes { get; set; }
    public List<DijkstraEdge> Edges { get; set; }
    public bool IsDirected { get; set; }

    private DijkstraEdge?[,]? _adjecencyMatrix;

    /// <summary>
    /// Configure fields of Nodes and Edges as well as the backing adjecency matrix.
    /// </summary>
    public void Initialize()
    {
        _adjecencyMatrix = BuildAdjecencyMatrix();

        // Initialize all nodes
        Nodes.ForEach(n => { });
    }

    private DijkstraEdge?[,] BuildAdjecencyMatrix()
    {
        DijkstraEdge?[,] matrix = new DijkstraEdge[Nodes.Count, Nodes.Count];
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

    public IEnumerable<DijkstraEdge> GetOutEdges(DijkstraNode node)
    {
        var outEdges = new List<DijkstraEdge>();

        for (var i = 0; i < _adjecencyMatrix!.GetLength(0); i++)
        {
            var e = _adjecencyMatrix[node.NodeId, i];
            if (e != null)
                outEdges.Add(e);
        }

        return outEdges;
    }

    public IEnumerable<DijkstraNode> GetOutNeighbours(DijkstraNode node)
    {
        return GetOutEdges(node)
            .Select(e => Nodes[e.TargetId == node.NodeId ? e.SourceId : e.TargetId]);
    }
}
