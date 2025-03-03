using System.Text.Json;
using TDT4900_MasterThesis.model.graph;

namespace TDT4900_MasterThesis.service;

public class GraphSerializerService
{
    public string SerializeGraph(Graph graph)
    {
        var graphDto = new GraphDto()
        {
            Nodes = graph.Nodes.ToArray(),
            Edges = graph
                .Edges.Select(e => new EdgeDto
                {
                    Target = e.Target.Id,
                    Source = e.Source.Id,
                    Weight = e.Weight,
                })
                .ToArray(),
        };

        return JsonSerializer.Serialize(graphDto);
    }

    public Graph DeserializeGraph(string json)
    {
        var graphDto = JsonSerializer.Deserialize<GraphDto>(json);
        var nodes = graphDto!.Nodes;
        var edges = graphDto
            .Edges.Select(e => new Edge(nodes[e.Source], nodes[e.Target], e.Weight))
            .ToArray();
        return new Graph(nodes, edges);
    }

    class GraphDto
    {
        public Node[] Nodes { get; set; }
        public EdgeDto[] Edges { get; set; }
    }

    class EdgeDto
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public int Weight { get; set; }
    }
}
