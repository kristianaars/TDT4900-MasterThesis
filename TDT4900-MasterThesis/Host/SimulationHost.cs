using Microsoft.Extensions.Hosting;
using TDT4900_MasterThesis.Engine;

namespace TDT4900_MasterThesis.Host;

public class SimulationHost : BackgroundService
{
    private SimulationEngine _engine;

    public SimulationHost(SimulationEngine engine)
    {
        _engine = engine;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //await _engine.RunSimulation(stoppingToken);
    }
}
