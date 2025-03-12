using CommunityToolkit.Mvvm.ComponentModel;

namespace TDT4900_MasterThesis.ViewModel.Component;

public partial class AlphaAlgorithmConfigurationViewModel(AppSettings appSettings)
    : ObservableObject
{
    [ObservableProperty]
    private int _deltaExcitatory = appSettings.Simulation.DeltaTExcitatory;

    [ObservableProperty]
    private int _deltaInhibitory = appSettings.Simulation.DeltaTInhibitory;

    [ObservableProperty]
    private int _tauPlus = appSettings.Simulation.TauPlus;

    [ObservableProperty]
    private int _tauZero = appSettings.Simulation.TauZero;

    [ObservableProperty]
    private int _refractoryPeriod = appSettings.Simulation.RefractoryPeriod;
}
