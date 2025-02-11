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

            AddEdgeIfNotExists(v1, v2, edges, edgeConnectionMatrix);
            AddEdgeIfNotExists(v2, v3, edges, edgeConnectionMatrix);
            AddEdgeIfNotExists(v3, v1, edges, edgeConnectionMatrix);
        }

        // Step 1: Ensure Connectivity with a Minimum Spanning Tree (MST)
        var mstEdges = GetMinimumSpanningTree(vertices, edges);

        // Step 2: Add Random Edges to Meet Desired Edge Count
        var remainingEdges = edges
            .Except(mstEdges)
            .OrderBy(_ => rnd.Next()) // Randomize order
            .Take(_edgeCount - mstEdges.Count)
            .ToList();

        var finalEdges = mstEdges.Concat(remainingEdges).ToList();

        var g = new Graph(vertices.ToArray(), finalEdges.ToArray());

        g.ConvertToBidirectional();

        g.Nodes.ForEach(n => n.Neighbours = g.GetOutEdges(n).Select(e => e.Target).ToArray());

        return g;
    }

    private void AddEdgeIfNotExists(Node v1, Node v2, List<Edge> edges, int[,] edgeConnectionMatrix)
    {
        if (edgeConnectionMatrix[Math.Min(v1.Id, v2.Id), Math.Max(v1.Id, v2.Id)] < 1)
        {
            edges.Add(GenerateEdge(v1, v2));
            edgeConnectionMatrix[Math.Min(v1.Id, v2.Id), Math.Max(v1.Id, v2.Id)] = 1;
        }
    }

    private List<Edge> GetMinimumSpanningTree(List<Node> vertices, List<Edge> edges)
    {
        // Kruskal's algorithm for MST
        var mstEdges = new List<Edge>();
        var parent = Enumerable.Range(0, vertices.Count).ToArray();

        int Find(int v)
        {
            if (parent[v] != v)
                parent[v] = Find(parent[v]);
            return parent[v];
        }

        void Union(int v1, int v2)
        {
            var root1 = Find(v1);
            var root2 = Find(v2);
            if (root1 != root2)
                parent[root1] = root2;
        }

        // Sort edges by weight
        var sortedEdges = edges.OrderBy(e => e.Weight).ToList();

        foreach (var edge in sortedEdges)
        {
            if (Find(edge.Source.Id) != Find(edge.Target.Id))
            {
                mstEdges.Add(edge);
                Union(edge.Source.Id, edge.Target.Id);

                // Stop if we've added enough edges for an MST
                if (mstEdges.Count == vertices.Count - 1)
                    break;
            }
        }

        return mstEdges;
    }

    private ICollection<Node> GetVertices()
    {
        var mapW = (int)Math.Sqrt(_vertexCount * 1000) * 3;
        var mapH = mapW;

        var rnd = new Random();
        var vertices = new List<Node>();

        var minDistance = 10;

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
