using CommunityToolkit.Mvvm.ComponentModel;
using TDT4900_MasterThesis.Algorithm;

namespace TDT4900_MasterThesis.ViewModel;

public partial class SimulationStatsViewModel(SequencePlotViewModel sequencePlotViewModel)
    : ObservableObject
{
    [ObservableProperty]
    private int _simulationBatchId;

    [ObservableProperty]
    private string _simulationState;

    [ObservableProperty]
    private AlgorithmType _algorithmType;

    [ObservableProperty]
    private int _completedSimulations;

    [ObservableProperty]
    private int _simulationBatchSize;

    [ObservableProperty]
    private int _tps;

    [ObservableProperty]
    private int _fps;

    [ObservableProperty]
    private long _currentTick;

    [ObservableProperty]
    private string _graphType;
}
