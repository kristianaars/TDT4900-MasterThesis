using CommunityToolkit.Mvvm.ComponentModel;

namespace TDT4900_MasterThesis.ViewModel.Configuration;

public partial class RadiusNeighbourGraphConfigurationViewModel : ObservableObject
{
    [ObservableProperty]
    private int _nodeCount = 25;

    [ObservableProperty]
    private int _distance = 50;

    [ObservableProperty]
    private int _radius = 50;

    [ObservableProperty]
    private int _noise = 25;
}
