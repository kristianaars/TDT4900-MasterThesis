using System.ComponentModel.DataAnnotations;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TDT4900_MasterThesis.engine;
using TDT4900_MasterThesis.factory;
using TDT4900_MasterThesis.host;
using TDT4900_MasterThesis.message;
using TDT4900_MasterThesis.model.graph;

namespace TDT4900_MasterThesis.viewmodel;

public partial class MainWindowViewModel : ObservableObject
{
    private NodeEngine _nodeEngine;

    private SimulationEngine _simulationEngine;

    private AppSettings _appSettings;

    #region Graph Settings Properties
    [ObservableProperty]
    private int _graphSettingsNodeCount;

    [ObservableProperty]
    private int _graphSettingsEdgeCount;
    #endregion

    #region Simulation Configuration Properties
    [ObservableProperty]
    private int _sourceNodeId;

    [ObservableProperty]
    private int _targetNodeId;

    [ObservableProperty]
    private int _targetTps;

    [ObservableProperty]
    private int _targetFps;
    #endregion

    #region Node Configuration Properties
    [ObservableProperty]
    private int _deltaExcitatory;

    [ObservableProperty]
    private int _deltaInhibitory;

    [ObservableProperty]
    private int _tauPlus;

    [ObservableProperty]
    private int _tauZero;

    [ObservableProperty]
    private int _refractoryPeriod;

    #endregion

    # region Simulation Control Properties
    [ObservableProperty]
    private long _currentTick;

    [ObservableProperty]
    private int _fps;

    [ObservableProperty]
    private int _tps;

    [ObservableProperty]
    private string _simulationState = "Stopped";
    #endregion

    public MainWindowViewModel(
        AppSettings appSettings,
        SimulationEngine simulationEngine,
        NodeEngine nodeEngine
    )
    {
        _appSettings = appSettings;
        _simulationEngine = simulationEngine;
        _nodeEngine = nodeEngine;

        _graphSettingsNodeCount = _appSettings.Simulation.GraphNodeCount;
        _graphSettingsEdgeCount = _appSettings.Simulation.GraphEdgeCount;

        _sourceNodeId = 0;
        _targetNodeId = 0;

        _targetTps = _appSettings.Simulation.TargetTps;
        _targetFps = _appSettings.Simulation.TargetFps;

        _deltaExcitatory = _nodeEngine.DeltaExcitatory;
        _deltaInhibitory = _nodeEngine.DeltaInhibitory;
        _tauPlus = _nodeEngine.TauPlus;
        _tauZero = _nodeEngine.TauZero;
        _refractoryPeriod = _nodeEngine.RefractoryPeriod;

        _simulationEngine.MainWindowViewModel = this;
    }

    [RelayCommand]
    private void ApplySimulationConfiguration()
    {
        _simulationEngine.TargetTps = TargetTps;
        _simulationEngine.TargetFps = TargetFps;
    }

    [RelayCommand]
    private void ApplyGraphConfiguration()
    {
        _nodeEngine.DeltaExcitatory = DeltaExcitatory;
        _nodeEngine.DeltaInhibitory = DeltaInhibitory;
        _nodeEngine.TauZero = TauZero;
        _nodeEngine.TauPlus = TauPlus;
        _nodeEngine.RefractoryPeriod = RefractoryPeriod;
    }

    [RelayCommand]
    private void GenerateNewGraph()
    {
        var nodeCount = GraphSettingsNodeCount;
        var edgeCount = GraphSettingsEdgeCount;

        var f = new RandomGraphFactory(nodeCount, edgeCount);

        Graph graph = f.GetGraph();

        if (TargetNodeId >= nodeCount || TargetNodeId == 0)
        {
            TargetNodeId = new Random().Next(1, graph.Nodes.Count);
        }

        _nodeEngine.TargetNodeId = TargetNodeId;
        WeakReferenceMessenger.Default.Send(new NewGraphMessage(graph));
    }

    [RelayCommand]
    private void PlaySimulation()
    {
        _simulationEngine.Play();
    }

    [RelayCommand]
    private void PauseSimulation()
    {
        _simulationEngine.Pause();
    }

    [RelayCommand]
    private void ResetSimulation()
    {
        _simulationEngine.Reset();
    }
}
