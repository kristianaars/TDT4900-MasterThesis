using CommunityToolkit.Mvvm.ComponentModel;

namespace TDT4900_MasterThesis.ViewModel.Configuration;

public partial class StratiumAlgorithmConfigurationViewModel(AppSettings appSettings)
    : ObservableObject
{
    [ObservableProperty]
    private int _deltaExcitatory = 7;

    [ObservableProperty]
    private int _deltaInhibitory = 5;

    [ObservableProperty]
    private int _tauPlus = 2;

    [ObservableProperty]
    private int _tauZero = 4;

    [ObservableProperty]
    private int _refractoryPeriod = 22;
}
