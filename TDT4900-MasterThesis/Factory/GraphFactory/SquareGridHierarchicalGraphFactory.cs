using TDT4900_MasterThesis.Helper;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Factory.GraphFactory;

public class SquareGridHierarchicalGraphFactory
{
    public required int NodeCount { get; set; }
    public required int Distance { get; set; }
    public required int Noise { get; set; }

    public required int BaseGridSize { get; set; }
    public required int HierarchicalLevels { get; set; }

    public Graph GetGraph()
    {
        var edges = new List<Edge>();

        var nodes = GraphFactoryHelper.ScatterNodes(NodeCount, Distance, Noise);

        //var nodes = GraphFactoryHelper.LineOfNodes(NodeCount, Distance);

        var dimensions = GraphFactoryHelper.GetDimensions(nodes);
        var width = dimensions.X;
        var height = dimensions.Y;

        var level = 0;

        while (level < HierarchicalLevels)
        {
            var gridSize = BaseGridSize * Math.Pow(2, level);

            var vGrids = (int)Math.Ceiling(height / gridSize) + 1;
            var hGrids = (int)Math.Ceiling(width / gridSize) + 1;
            var gridBlocks = new List<Node>[hGrids, vGrids];

            // Initialize each element with a new List<Node>
            for (int i = 0; i < hGrids; i++)
            for (int j = 0; j < vGrids; j++)
                gridBlocks[i, j] = new List<Node>();

            // Fill grid blocks with relevant nodes
            foreach (var node in nodes)
            {
                var x = (int)Math.Max(0, Math.Ceiling(node.X / gridSize));
                var y = (int)Math.Max(0, Math.Ceiling(node.Y / gridSize));
                gridBlocks[x, y].Add(node);
            }

            var edgeCounter = 0;
            // Connect nodes with nodes in neighbouring grid blocks (left, right, above, below)
            // In principle all direct neighbours are connected, but we only add right and bottom neighbours to avoid
            // duplicate edges. The final graph is undirected, and will solve this problem
            for (var i = 0; i < hGrids; i++)
            {
                for (var j = 0; j < vGrids; j++)
                {
                    var currentBlock = gridBlocks[i, j];
                    var neighbours = new List<Node>();

                    // Add from bottom neighbouring grid block
                    if (j + 1 < vGrids)
                        neighbours.AddRange(gridBlocks[i, j + 1]);

                    // Add from right neighbouring grid block
                    if (i + 1 < hGrids)
                        neighbours.AddRange(gridBlocks[i + 1, j]);

                    // Create edges between nodes in current block and neighbouring blocks
                    edges.AddRange(
                        currentBlock.SelectMany(n =>
                            neighbours.Select(m => new Edge
                            {
                                EdgeId = edgeCounter++,
                                SourceNodeId = n.NodeId,
                                TargetNodeId = m.NodeId,
                                Level = level,
                            })
                        )
                    );
                }
            }

            level++;
        }

        return new Graph { Edges = edges, Nodes = nodes };
    }
}
