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
        NodeEngine nodeEngine,
        SequencePlotViewModel sequencePlotViewModel,
        GraphPlotViewModel graphPlotViewModel,
        SimulationService simulationService,
        AlphaAlgorithmConfigurationViewModel alphaAlgorithmConfigurationViewModel
    )
    {
        _appSettings = appSettings;
        _nodeEngine = nodeEngine;
        _graphPlotViewModel = graphPlotViewModel;
        _sequencePlotViewModel = sequencePlotViewModel;
        _simulationService = simulationService;
        _alphaAlgorithmConfigurationViewModel = alphaAlgorithmConfigurationViewModel;

        _graphSettingsNodeCount = _appSettings.Simulation.GraphNodeCount;
        _graphSettingsEdgeCount = _appSettings.Simulation.GraphEdgeCount;

        _sourceNodeId = 0;
        _targetNodeId = 0;

        _targetTps = _appSettings.Simulation.TargetTps;
        _targetFps = _appSettings.Simulation.TargetFps;

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
        _simulationService.SetTargetFps(TargetFps);
        _simulationService.SetTargetTps(TargetTps);
    }

    [RelayCommand]
    private async Task PlaySimulation() =>
        await _simulationService.RunSimulationBatchAsync(
            200,
            new NeighboringGraphSpec()
            {
                NodeCount = 1000,
                Distance = 70,
                Radius = 100,
                Noise = 20,
            },
            new AlphaAlgorithmSpec()
            {
                AlgorithmType = AlgorithmType.Alpha,
                DeltaTExcitatory = AlphaAlgorithmConfigurationViewModel.DeltaExcitatory,
                DeltaTInhibitory = AlphaAlgorithmConfigurationViewModel.DeltaInhibitory,
                RefractoryPeriod = AlphaAlgorithmConfigurationViewModel.RefractoryPeriod,
                TauPlus = AlphaAlgorithmConfigurationViewModel.TauPlus,
                TauZero = AlphaAlgorithmConfigurationViewModel.TauZero,
            },
            CancellationToken.None
        );

    [RelayCommand]
    private void PauseSimulation()
    {
        _simulationService.PauseSimulation();
    }

    [RelayCommand]
    private void ResumeSimulation()
    {
        _simulationService.ResumeSimulation();
    }
}
