using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;

namespace TDT4900_MasterThesis.engine;

public class MessageWaveEngine : IUpdatable
{
    private readonly NodeMessageEngine _messageEngine;
    private readonly Graph _graph;

    private readonly int _waveInterval;
    private long _lastWave;

    public MessageWaveEngine(NodeMessageEngine messageEngine, Graph graph, AppSettings appSettings)
    {
        _messageEngine = messageEngine;
        _graph = graph;

        _waveInterval = appSettings.Simulation.WaveInterval;
    }

    public void Update(long currentTick)
    {
        if (_lastWave + _waveInterval <= currentTick)
        {
            BeginNewWave(currentTick);
        }
    }

    public void BeginNewWave(long atTick)
    {
        _lastWave = atTick;
        _messageEngine.SendMessage(new Message(atTick, _graph.Nodes[0], _graph.Nodes[0]));
    }
}
