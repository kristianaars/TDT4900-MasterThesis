using CommunityToolkit.Mvvm.ComponentModel;

namespace TDT4900_MasterThesis.ViewModel;

public partial class NeighbourGraphConfigurationViewModel : ObservableObject
{
    [ObservableProperty]
    private int _nodeCount = 100;

    [ObservableProperty]
    private int _distance = 50;

    [ObservableProperty]
    private int _radius = 150;

    [ObservableProperty]
    private int _noise = 25;
}
