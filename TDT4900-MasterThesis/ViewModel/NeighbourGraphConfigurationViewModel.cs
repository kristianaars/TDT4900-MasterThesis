using CommunityToolkit.Mvvm.ComponentModel;

namespace TDT4900_MasterThesis.ViewModel;

public partial class NeighbourGraphConfigurationViewModel : ObservableObject
{
    [ObservableProperty]
    private int _nodeCount;

    [ObservableProperty]
    private int _distance;

    [ObservableProperty]
    private int _radius;

    [ObservableProperty]
    private int _noise;
}
