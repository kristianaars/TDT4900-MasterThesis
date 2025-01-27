using MIConvexHull;
using TDT4900_MasterThesis.helpers;
using TDT4900_MasterThesis.model.graph;

namespace TDT4900_MasterThesis.factory;

public class RandomGraphFactory
{
    private int _vertexCount;
    private int _edgeCount;

    public double WeightBiasRange { get; set; } = 0;

    public RandomGraphFactory(int vertexCount, int edgeCount)
    {
        _vertexCount = vertexCount;
        _edgeCount = edgeCount;
    }

    private ICollection<Node> GetVertices()
    {
        var mapW = (int)Math.Sqrt(_vertexCount * 1000) * 3;
        var mapH = mapW;

        var rnd = new Random();
        var vertices = new List<Node>();

        var minDistance = 35;

        while (vertices.Count < _vertexCount)
        {
            var x = rnd.Next(20, mapW + 20);
            var y = rnd.Next(20, mapH + 20);

            if (
                vertices.Count == 0
                || vertices.Min(u => PointHelper.DistanceBetween(x, y, u.X, u.Y) > minDistance)
            )
            {
                vertices.Add(new Node(vertices.Count) { X = x, Y = y });
            }
        }

        return vertices;
    }

    public Graph GetGraph()
    {
        var rnd = new Random();

        // Generate vertices
        var vertices = GetVertices().ToList();

        // Setup edge list and edge matrix. The edge matrix contains info about which vertices are already connected
        var edges = new List<Edge>();
        var edgeConnectionMatrix = new int[_vertexCount, _vertexCount];

        // Perform Delaunay Triangulation
        var delaunay = Triangulation.CreateDelaunay(vertices);

        foreach (var cell in delaunay.Cells)
        {
            var v1 = cell.Vertices[0];
            var v2 = cell.Vertices[1];
            var v3 = cell.Vertices[2];

            if (edgeConnectionMatrix[Math.Min(v1.Id, v2.Id), Math.Max(v1.Id, v2.Id)] < 1)
            {
                edges.Add(GenerateEdge(v1, v2));
                edgeConnectionMatrix[Math.Min(v1.Id, v2.Id), Math.Max(v1.Id, v2.Id)] = 1;
            }

            if (edgeConnectionMatrix[Math.Min(v2.Id, v3.Id), Math.Max(v2.Id, v3.Id)] < 1)
            {
                edges.Add(GenerateEdge(v2, v3));
                edgeConnectionMatrix[Math.Min(v2.Id, v3.Id), Math.Max(v2.Id, v3.Id)] = 1;
            }

            if (edgeConnectionMatrix[Math.Min(v3.Id, v1.Id), Math.Max(v3.Id, v1.Id)] < 1)
            {
                edges.Add(GenerateEdge(v3, v1));
                edgeConnectionMatrix[Math.Min(v3.Id, v1.Id), Math.Max(v3.Id, v1.Id)] = 1;
            }
        }

        edges.Sort((e1, e2) => e2.Weight - e1.Weight);
        edges.RemoveRange(0, edges.Count - _edgeCount);

        return new Graph(vertices.ToArray(), edges.ToArray());
    }

    public Edge GenerateEdge(Node source, Node target)
    {
        var random = new Random();
        var dist = PointHelper.DistanceBetween(source.X, source.Y, target.X, target.Y);

        return new Edge(
            source,
            target,
            (int)((dist * (1 + (random.NextDouble() - 0.5) * 2 * WeightBiasRange)))
        );
    }
}
