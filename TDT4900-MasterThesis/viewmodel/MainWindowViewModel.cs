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
    private SequencePlotViewModel _sequencePlotViewModel;
    private GraphPlotViewModel _graphPlotViewModel;

    private SimulationEngine _simulationEngine;

    private AppSettings _appSettings;

    [ObservableProperty]
    private int _graphSettingsNodeCount;

    [ObservableProperty]
    private int _graphSettingsEdgeCount;

    [ObservableProperty]
    private int _sourceNodeId;

    [ObservableProperty]
    private int _targetNodeId;

    [ObservableProperty]
    private int _targetTps;

    [ObservableProperty]
    private int _targetFps;

    [ObservableProperty]
    private long _currentTick;

    [ObservableProperty]
    private int _fps;

    [ObservableProperty]
    private int _tps;

    [ObservableProperty]
    private string _simulationState = "Stopped";

    public MainWindowViewModel(
        SequencePlotViewModel sequencePlotViewModel,
        GraphPlotViewModel graphPlotViewModel,
        AppSettings appSettings,
        SimulationEngine simulationEngine
    )
    {
        _sequencePlotViewModel = sequencePlotViewModel;
        _graphPlotViewModel = graphPlotViewModel;
        _appSettings = appSettings;
        _simulationEngine = simulationEngine;

        _graphSettingsNodeCount = _appSettings.Simulation.GraphNodeCount;
        _graphSettingsEdgeCount = _appSettings.Simulation.GraphEdgeCount;

        _sourceNodeId = 0;
        _targetNodeId = 0;

        _targetTps = _appSettings.Simulation.TargetTps;
        _targetFps = _appSettings.Simulation.TargetFps;

        _simulationEngine.MainWindowViewModel = this;
    }

    [RelayCommand]
    private void ApplySimulationConfiguration()
    {
        _simulationEngine.TargetTps = TargetTps;
        _simulationEngine.TargetFps = TargetFps;
    }

    [RelayCommand]
    private void ResetSimulation()
    {
        var nodeCount = GraphSettingsNodeCount;
        var edgeCount = GraphSettingsEdgeCount;

        var f = new RandomGraphFactory(nodeCount, edgeCount);

        Graph graph = f.GetGraph();

        graph.Nodes.ForEach(n => n.SimulationSettings = _appSettings.Simulation);

        if (TargetNodeId >= nodeCount || TargetNodeId == 0)
        {
            TargetNodeId = new Random().Next(1, graph.Nodes.Count);
        }

        graph.Nodes[TargetNodeId].IsTagged = true;

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
}
