using System.ComponentModel.DataAnnotations;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TDT4900_MasterThesis.Algorithm;
using TDT4900_MasterThesis.Engine;
using TDT4900_MasterThesis.Factory;
using TDT4900_MasterThesis.Host;
using TDT4900_MasterThesis.Message;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Model.Graph;
using TDT4900_MasterThesis.service;
using TDT4900_MasterThesis.viewmodel.Component;

namespace TDT4900_MasterThesis.viewmodel;

public partial class MainWindowViewModel : ObservableObject
{
    private NodeEngine _nodeEngine;

    private SimulationEngine _simulationEngine;
    private SequencePlotViewModel _sequencePlotViewModel;
    private GraphPlotViewModel _graphPlotViewModel;
    private SimulationBatchEngine _simulationBatchEngine;

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

    [ObservableProperty]
    private IEnumerable<ComboBoxItemModel<AlgorithmType>> _algorithmOptions;

    [ObservableProperty]
    private ComboBoxItemModel<AlgorithmType> _selectedAlgorithmOption;

    public MainWindowViewModel(
        AppSettings appSettings,
        SimulationEngine simulationEngine,
        NodeEngine nodeEngine,
        SequencePlotViewModel sequencePlotViewModel,
        GraphPlotViewModel graphPlotViewModel,
        SimulationBatchEngine simulationBatchEngine
    )
    {
        _appSettings = appSettings;
        _simulationEngine = simulationEngine;
        _nodeEngine = nodeEngine;
        _graphPlotViewModel = graphPlotViewModel;
        _sequencePlotViewModel = sequencePlotViewModel;
        _simulationBatchEngine = simulationBatchEngine;

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

        AlgorithmOptions = Enum.GetValues<AlgorithmType>()
            .Select(e => new ComboBoxItemModel<AlgorithmType>()
            {
                Value = e,
                DisplayName = e.ToString(),
            });
        SelectedAlgorithmOption = AlgorithmOptions.First();
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
        /*
        var nodeCount = GraphSettingsNodeCount;
        var edgeCount = GraphSettingsEdgeCount;

        var f = new RandomGraphFactory(nodeCount, edgeCount);

        Graph graph = f.GetGraph();
        */

        var nodeCount = GraphSettingsNodeCount;
        var nodeSpacing = 70;
        var nodeRadius = 100;
        var noise = 20;

        var f = new NeighbouringGraphFactory(nodeCount, nodeSpacing, nodeRadius, noise);
        var graph = f.GetGraph();

        if (TargetNodeId >= nodeCount || TargetNodeId == 0)
        {
            TargetNodeId = new Random().Next(1, graph.Nodes.Count);
        }

        _nodeEngine.TargetNodeId = TargetNodeId;
        //WeakReferenceMessenger.Default.Send(new NewGraphMessage(graph));

        //new PersistenceService(new GraphSerializerService()).SaveGraph(graph, "");
    }

    [RelayCommand]
    private async Task PlaySimulation()
    {
        var f = new NeighbouringGraphFactory(45, 70, 100, 20);
        var graph = f.GetGraph();

        var algSpec = new AlphaAlgorithmSpec()
        {
            AlgorithmType = AlgorithmType.Alpha,
            DeltaTExcitatory = DeltaExcitatory,
            DeltaTInhibitory = DeltaInhibitory,
            RefractoryPeriod = RefractoryPeriod,
            TauPlus = TauPlus,
            TauZero = TauZero,
        };

        var simulationBatch = new SimulationBatch()
        {
            Simulations =
            [
                new()
                {
                    Graph = graph,
                    AlgorithmSpec = algSpec,
                    StartNode = graph.Nodes[0],
                    TargetNode = graph.Nodes[5],
                },
                new()
                {
                    Graph = graph,
                    AlgorithmSpec = algSpec,
                    StartNode = graph.Nodes[0],
                    TargetNode = graph.Nodes[5],
                },
                new()
                {
                    Graph = graph,
                    AlgorithmSpec = algSpec,
                    StartNode = graph.Nodes[0],
                    TargetNode = graph.Nodes[5],
                },
            ],
        };

        await _simulationBatchEngine.RunSimulationBatchAsync(
            simulationBatch,
            CancellationToken.None
        );
    }

    [RelayCommand]
    private void PauseSimulation()
    {
        _simulationEngine.Pause();
    }

    [RelayCommand]
    private void ResetSimulation()
    {
        lock (SimulationEngine.UpdateLock)
        {
            _simulationEngine.Reset();
        }
    }
}
