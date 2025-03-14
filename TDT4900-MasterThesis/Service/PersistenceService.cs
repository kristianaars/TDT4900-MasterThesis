using System.Text.Json;
using TDT4900_MasterThesis.Model.Graph;

namespace TDT4900_MasterThesis.Service;

public class PersistenceService(GraphSerializerService graphSerializerService)
{
    public void SaveGraph(SimulationGraph simulationGraph, string path)
    {
        var json = graphSerializerService.SerializeGraph(simulationGraph);

        var g = graphSerializerService.DeserializeGraph(json);
    }

    public void LoadGraph(SimulationGraph simulationGraph) { }
}
