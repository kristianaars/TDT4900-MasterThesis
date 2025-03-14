using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TDT4900_MasterThesis.Algorithm;
using TDT4900_MasterThesis.Engine;
using TDT4900_MasterThesis.Factory;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Service;
using TDT4900_MasterThesis.ViewModel.Component;

namespace TDT4900_MasterThesis.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{
    private NodeEngine _nodeEngine;

    private SequencePlotViewModel _sequencePlotViewModel;
    private GraphPlotViewModel _graphPlotViewModel;
    private SimulationService _simulationService;

    [ObservableProperty]
    private AlphaAlgorithmConfigurationViewModel _alphaAlgorithmConfigurationViewModel;

    [ObservableProperty]
    private NeighbourGraphConfigurationViewModel _neighbourGraphConfigurationViewModel;

    private AppSettings _appSettings;

    [ObservableProperty]
    private int _targetTps;

    [ObservableProperty]
    private int _targetFps;

    [ObservableProperty]
    private int _simulationBatchSize;

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
        NodeEngine nodeEngine,
        SequencePlotViewModel sequencePlotViewModel,
        GraphPlotViewModel graphPlotViewModel,
        SimulationService simulationService,
        AlphaAlgorithmConfigurationViewModel alphaAlgorithmConfigurationViewModel,
        NeighbourGraphConfigurationViewModel neighbourGraphConfigurationViewModel
    )
    {
        _appSettings = appSettings;
        _nodeEngine = nodeEngine;
        _graphPlotViewModel = graphPlotViewModel;
        _sequencePlotViewModel = sequencePlotViewModel;
        _simulationService = simulationService;
        _alphaAlgorithmConfigurationViewModel = alphaAlgorithmConfigurationViewModel;
        _neighbourGraphConfigurationViewModel = neighbourGraphConfigurationViewModel;

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
        var graph = new GraphFactory().CreateGraph(
            new NeighboringGraphSpec()
            {
                NodeCount = NeighbourGraphConfigurationViewModel.NodeCount,
                Distance = NeighbourGraphConfigurationViewModel.Distance,
                Radius = NeighbourGraphConfigurationViewModel.Radius,
                Noise = NeighbourGraphConfigurationViewModel.Noise,
            }
        );
        _graphPlotViewModel.InitializeGraph(graph);
    }

    [RelayCommand]
    private void ApplySimulationConfiguration()
    {
        _simulationService.SetTargetFps(TargetFps);
        _simulationService.SetTargetTps(TargetTps);
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task RunSimulationBatchAsync(CancellationToken cancellationToken) =>
        await _simulationService.RunSimulationBatchAsync(
            SimulationBatchSize,
            PersistSimulationBatch,
            new NeighboringGraphSpec()
            {
                NodeCount = NeighbourGraphConfigurationViewModel.NodeCount,
                Distance = NeighbourGraphConfigurationViewModel.Distance,
                Radius = NeighbourGraphConfigurationViewModel.Radius,
                Noise = NeighbourGraphConfigurationViewModel.Noise,
            },
            BuildAlgorithmSpec(),
            cancellationToken
        );

    private AlgorithmSpec BuildAlgorithmSpec() =>
        SelectedAlgorithmOption.Value switch
        {
            AlgorithmType.Alpha => new AlphaAlgorithmSpec()
            {
                AlgorithmType = AlgorithmType.Alpha,
                DeltaTExcitatory = AlphaAlgorithmConfigurationViewModel.DeltaExcitatory,
                DeltaTInhibitory = AlphaAlgorithmConfigurationViewModel.DeltaInhibitory,
                RefractoryPeriod = AlphaAlgorithmConfigurationViewModel.RefractoryPeriod,
                TauPlus = AlphaAlgorithmConfigurationViewModel.TauPlus,
                TauZero = AlphaAlgorithmConfigurationViewModel.TauZero,
            },
            _ => new AlgorithmSpec() { AlgorithmType = SelectedAlgorithmOption.Value },
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
