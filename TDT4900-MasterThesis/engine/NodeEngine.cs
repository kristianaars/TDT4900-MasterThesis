using CommunityToolkit.Mvvm.Messaging;
using TDT4900_MasterThesis.message;
using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;
using TDT4900_MasterThesis.viewmodel;

namespace TDT4900_MasterThesis.engine;

/// <summary>
/// Engine responsible for updating nodes of the graph
/// </summary>
public class NodeEngine : IUpdatable
{
    private readonly SequencePlotViewModel _sequencePlotViewModel;
    private readonly GraphPlotViewModel _graphPlotViewModel;

    private NodeState[] _previousState;
    private bool[] _isTagged;

    private Graph? _graph;

    private int _deltaExcitatory;
    private int _deltaInhibitory;
    private int _tauZero;
    private int _tauPlus;
    private int _refractoryPeriod;

    public int TargetNodeId { get; set; }

    public int DeltaExcitatory
    {
        get => _deltaExcitatory;
        set
        {
            _deltaExcitatory = value;
            SetDeltaExcitatoryForAllNodes(_deltaExcitatory);
        }
    }

    public int DeltaInhibitory
    {
        get => _deltaInhibitory;
        set
        {
            _deltaInhibitory = value;
            SetDeltaInhibitoryForAllNodes(_deltaInhibitory);
        }
    }

    public int TauZero
    {
        get => _tauZero;
        set
        {
            _tauZero = value;
            SetTauZeroForAllNodes(_tauZero);
        }
    }

    public int TauPlus
    {
        get => _tauPlus;
        set
        {
            _tauPlus = value;
            SetTauPlusForAllNodes(_tauPlus);
        }
    }

    public int RefractoryPeriod
    {
        get => _refractoryPeriod;
        set
        {
            _refractoryPeriod = value;
            SetRefractoryPeriodForAllNodes(_refractoryPeriod);
        }
    }

    public NodeEngine(
        AppSettings appSettings,
        SequencePlotViewModel sequencePlotViewModel,
        GraphPlotViewModel graphPlotViewModel
    )
    {
        _sequencePlotViewModel = sequencePlotViewModel;
        _graphPlotViewModel = graphPlotViewModel;
        WeakReferenceMessenger.Default.Register<NewGraphMessage>(
            this,
            (o, m) => ReceiveNewGraphMessage(m)
        );

        _deltaExcitatory = appSettings.Simulation.DeltaTExcitatory;
        _deltaInhibitory = appSettings.Simulation.DeltaTInhibitory;
        _tauZero = appSettings.Simulation.TauZero;
        _tauPlus = appSettings.Simulation.TauPlus;
        _refractoryPeriod = appSettings.Simulation.RefractoryPeriod;

        _previousState = [];
        _isTagged = [];
    }

    public void Update(long currentTick)
    {
        _graph?.Nodes.ForEach(n =>
        {
            n.Update(currentTick);

            if (n.State != _previousState[n.Id] || n.IsTagged != _isTagged[n.Id])
            {
                var update = new NodeStateUpdate(n.Id, n.State, n.IsTagged, currentTick);
                _sequencePlotViewModel.AppendNodeStateUpdate(update);
                _graphPlotViewModel.AppendStateUpdate(update);

                _previousState[n.Id] = n.State;
                _isTagged[n.Id] = n.IsTagged;
            }
        });
    }

    public void ResetComponent()
    {
        _graph!.Nodes.ForEach(n => n.Reset());
        _graph!.Nodes[TargetNodeId].IsTagged = true;

        SetDeltaExcitatoryForAllNodes(_deltaExcitatory);
        SetDeltaInhibitoryForAllNodes(_deltaInhibitory);
        SetTauZeroForAllNodes(_tauZero);
        SetTauPlusForAllNodes(_tauPlus);
        SetRefractoryPeriodForAllNodes(_refractoryPeriod);
    }

    private void SetDeltaExcitatoryForAllNodes(int deltaExcitatory) =>
        _graph?.Nodes.ForEach(n => n.DeltaExcitatory = deltaExcitatory);

    private void SetDeltaInhibitoryForAllNodes(int deltaInhibitory) =>
        _graph?.Nodes.ForEach(n => n.DeltaInhibitory = deltaInhibitory);

    private void SetTauZeroForAllNodes(int tauZero) =>
        _graph?.Nodes.ForEach(n => n.TauZero = tauZero);

    private void SetTauPlusForAllNodes(int tauPlus) =>
        _graph?.Nodes.ForEach(n => n.TauPlus = tauPlus);

    private void SetRefractoryPeriodForAllNodes(int refractoryPeriod) =>
        _graph?.Nodes.ForEach(n => n.RefractoryPeriod = refractoryPeriod);

    /// <summary>
    /// Recipient function for when a new graph is sat in the system
    /// </summary>
    /// <param name="message"></param>
    private void ReceiveNewGraphMessage(NewGraphMessage message)
    {
        _graph = message.Value;

        _previousState = new NodeState[_graph!.Nodes.Count];
        _isTagged = new bool[_graph.Nodes.Count];

        ResetComponent();
    }
}
