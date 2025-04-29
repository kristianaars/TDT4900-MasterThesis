using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using TDT4900_MasterThesis.Algorithm;
using TDT4900_MasterThesis.Factory.GraphFactory;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Service;
using TDT4900_MasterThesis.ViewModel.Component;
using TDT4900_MasterThesis.ViewModel.Configuration;
using AlphaAlgorithmConfigurationViewModel = TDT4900_MasterThesis.ViewModel.Configuration.AlphaAlgorithmConfigurationViewModel;

namespace TDT4900_MasterThesis.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{
    private SequencePlotViewModel _sequencePlotViewModel;
    private GraphPlotViewModel _graphPlotViewModel;
    private SimulationService _simulationService;
    private SimulationBatchService _simulationBatchService;

    [ObservableProperty]
    private AlphaAlgorithmConfigurationViewModel _alphaAlgorithmConfigurationViewModel;

    [ObservableProperty]
    private StratiumAlgorithmConfigurationViewModel _stratiumAlgorithmConfigurationViewModel;

    [ObservableProperty]
    private RadiusNeighbourGraphConfigurationViewModel _radiusNeighbourGraphConfigurationViewModel;

    [ObservableProperty]
    private SquareGridHierarchicalGraphConfigurationViewModel _squareGridHierarchicalConfigurationViewModel;

    private AppSettings _appSettings;

    [ObservableProperty]
    private int _targetTps;

    [ObservableProperty]
    private int _targetFps;

    [ObservableProperty]
    private int _simulationBatchSize = 1;

    [ObservableProperty]
    private bool _persistSimulationBatch;

    [ObservableProperty]
    private IEnumerable<ComboBoxItemModel<AlgorithmType>> _algorithmOptions;

    [ObservableProperty]
    private ComboBoxItemModel<AlgorithmType> _selectedAlgorithmOption;

    [ObservableProperty]
    private IEnumerable<ComboBoxItemModel<GraphType>> _graphOptions;

    [ObservableProperty]
    private ComboBoxItemModel<GraphType> _selectedGraphOption;

    public MainWindowViewModel(
        AppSettings appSettings,
        SimulationBatchService simulationBatchService,
        SequencePlotViewModel sequencePlotViewModel,
        GraphPlotViewModel graphPlotViewModel,
        SimulationService simulationService,
        AlphaAlgorithmConfigurationViewModel alphaAlgorithmConfigurationViewModel,
        StratiumAlgorithmConfigurationViewModel stratiumAlgorithmConfigurationViewModel,
        RadiusNeighbourGraphConfigurationViewModel radiusNeighbourGraphConfigurationViewModel,
        SquareGridHierarchicalGraphConfigurationViewModel squareGridHierarchicalConfigurationViewModel
    )
    {
        _appSettings = appSettings;
        _graphPlotViewModel = graphPlotViewModel;
        _sequencePlotViewModel = sequencePlotViewModel;
        _simulationService = simulationService;
        _alphaAlgorithmConfigurationViewModel = alphaAlgorithmConfigurationViewModel;
        _stratiumAlgorithmConfigurationViewModel = stratiumAlgorithmConfigurationViewModel;
        _radiusNeighbourGraphConfigurationViewModel = radiusNeighbourGraphConfigurationViewModel;
        _squareGridHierarchicalConfigurationViewModel =
            squareGridHierarchicalConfigurationViewModel;
        _simulationBatchService = simulationBatchService;

        _targetTps = _appSettings.Simulation.TargetTps;
        _targetFps = _appSettings.Simulation.TargetFps;

        // Initialize algorithm options
        AlgorithmOptions = Enum.GetValues<AlgorithmType>()
            .Select(e => new ComboBoxItemModel<AlgorithmType>()
            {
                Value = e,
                DisplayName = e.ToString(),
            });
        SelectedAlgorithmOption = AlgorithmOptions.First();

        // Initialize graph options
        GraphOptions = Enum.GetValues<GraphType>()
            .Select(e => new ComboBoxItemModel<GraphType>()
            {
                Value = e,
                DisplayName = e.ToString(),
            });
        SelectedGraphOption = GraphOptions.First();
    }

    [RelayCommand]
    private void ShowExampleGraph()
    {
        var graph = new GraphFactory().CreateGraph(BuildGraphSpec());
        _graphPlotViewModel.Graph = graph;
    }

    [RelayCommand]
    private void ApplySimulationConfiguration()
    {
        _simulationService.SetTargetFps(TargetFps);
        _simulationService.SetTargetTps(TargetTps);
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task RunSimulationBatchAsync(CancellationToken cancellationToken) =>
        new Thread(async () =>
        {
            try
            {
                await _simulationBatchService.RunSimulationBatchAsync(
                    SimulationBatchSize,
                    PersistSimulationBatch,
                    BuildGraphSpec(),
                    BuildAlgorithmSpec(),
                    cancellationToken
                );
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Fatal error while running simulation batch");
            }
        }).Start();

    private AlgorithmSpec BuildAlgorithmSpec() =>
        SelectedAlgorithmOption.Value switch
        {
            AlgorithmType.Alpha => new AlphaAlgorithmSpec()
            {
                DeltaTExcitatory = AlphaAlgorithmConfigurationViewModel.DeltaExcitatory,
                DeltaTInhibitory = AlphaAlgorithmConfigurationViewModel.DeltaInhibitory,
                RefractoryPeriod = AlphaAlgorithmConfigurationViewModel.RefractoryPeriod,
                TauPlus = AlphaAlgorithmConfigurationViewModel.TauPlus,
                TauZero = AlphaAlgorithmConfigurationViewModel.TauZero,
            },
            AlgorithmType.Stratium => new StratiumAlgorithmSpec()
            {
                DeltaTExcitatory = StratiumAlgorithmConfigurationViewModel.DeltaExcitatory,
                DeltaTInhibitory = StratiumAlgorithmConfigurationViewModel.DeltaInhibitory,
                RefractoryPeriod = StratiumAlgorithmConfigurationViewModel.RefractoryPeriod,
                TauPlus = StratiumAlgorithmConfigurationViewModel.TauPlus,
                TauZero = StratiumAlgorithmConfigurationViewModel.TauZero,
            },
            _ => new AlgorithmSpec() { AlgorithmType = SelectedAlgorithmOption.Value },
        };

    private GraphSpec BuildGraphSpec() =>
        SelectedGraphOption.Value switch
        {
            GraphType.RadiusNeighbourhood => new RadiusNeighboringGraphSpec()
            {
                NodeCount = RadiusNeighbourGraphConfigurationViewModel.NodeCount,
                Distance = RadiusNeighbourGraphConfigurationViewModel.Distance,
                Radius = RadiusNeighbourGraphConfigurationViewModel.Radius,
                Noise = RadiusNeighbourGraphConfigurationViewModel.Noise,
            },
            GraphType.SquareGridHierarchical => new SquareGridHierarchicalGraphSpec()
            {
                NodeCount = SquareGridHierarchicalConfigurationViewModel.NodeCount,
                BaseGridSize = SquareGridHierarchicalConfigurationViewModel.BaseGridSize,
                Distance = SquareGridHierarchicalConfigurationViewModel.Distance,
                HierarchicalLevels =
                    SquareGridHierarchicalConfigurationViewModel.HierarchicalLevels,
                Noise = SquareGridHierarchicalConfigurationViewModel.Noise,
                SingleLineGraph = SquareGridHierarchicalConfigurationViewModel.SingleLineGraph,
            },
            _ => throw new ArgumentException("Unknown graph type"),
        };

    [RelayCommand]
    private void PauseSimulation()
    {
        _simulationService.PauseSimulation();
    }

    [RelayCommand]
    private void PlaySimulation()
    {
        _simulationService.ResumeSimulation();
    }

    [RelayCommand]
    private void StopSimulation()
    {
        throw new NotImplementedException();
    }
}
