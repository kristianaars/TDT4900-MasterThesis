using System.Numerics;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Helper;

public class GraphFactoryHelper
{
    /// <summary>
    /// Generates a list of nodes with evenly spaced coordinates.
    /// </summary>
    /// <param name="nodeCount">Number of nodes to generate</param>
    /// <param name="spacing">The spacing between each node</param>
    /// <param name="noise">Noise of space between the nodes.</param>
    /// <returns></returns>
    public static List<Node> ScatterNodes(int nodeCount, int spacing, int noise = 0)
    {
        Random random = new Random();

        // Number of nodes in horizontal and vertical direction
        var h = (int)Math.Ceiling(Math.Sqrt(nodeCount));

        var nodes = new List<Node>(nodeCount);

        for (var i = 0; i < h; i++)
        {
            for (var j = 0; j < h; j++)
            {
                if (nodes.Count < nodeCount)
                {
                    nodes.Add(
                        new Node()
                        {
                            NodeId = nodes.Count,
                            X = i * spacing + random.Next(-noise, noise),
                            Y = j * spacing + random.Next(-noise, noise),
                        }
                    );
                }
            }
        }

        return nodes;
    }

    /// <summary>
    /// Generates a list of nodes on a line.
    /// </summary>
    /// <param name="nodeCount">Number of nodes</param>
    /// <param name="spacing">Distance between the nodes</param>
    /// <returns></returns>
    public static List<Node> LineOfNodes(int nodeCount, int spacing, int noise = 0)
    {
        Random random = new Random();

        return Enumerable
            .Range(0, nodeCount)
            .Select(i => new Node()
            {
                NodeId = i,
                X = i * spacing + random.Next(-noise, noise),
                Y = 0,
            })
            .ToList();
    }

    /// <summary>
    /// Calculates the Euclidean distance between two nodes a and b.
    /// </summary>
    /// <param name="a">Node a</param>
    /// <param name="b">Node b</param>
    /// <returns>Distance between nodes a and b</returns>
    public static double Distance(Node a, Node b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    public static Vector2 GetDimensions(List<Node> nodes) =>
        new() { X = nodes.Max(n => n.X), Y = nodes.Max(n => n.Y) };
}
