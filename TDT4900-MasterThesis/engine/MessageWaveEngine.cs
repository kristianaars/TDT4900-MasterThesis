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
        _lastWave = -_waveInterval;

        graph.Nodes[3].IsTagged = true;
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
        var target = _graph.Nodes[0];

        _graph.Nodes.ForEach(node => node.State = NodeState.Neutral);

        _lastWave = atTick;
        _messageEngine.QueueProcessMessage(
            new ProcessMessage(
                atTick,
                new NodeMessage(atTick, null, target, NodeMessage.MessageType.Excitatory)
            )
        );
    }
}
