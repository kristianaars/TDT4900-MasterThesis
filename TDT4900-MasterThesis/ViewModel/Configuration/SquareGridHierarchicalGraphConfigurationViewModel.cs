using CommunityToolkit.Mvvm.ComponentModel;

namespace TDT4900_MasterThesis.ViewModel.Configuration;

/// <summary>
/// View model for configuration of a square grid hierarchical graph.
/// Target type: <see cref="Model.Db.SquareGridHierarchicalGraphSpec"/>
/// </summary>
public partial class SquareGridHierarchicalGraphConfigurationViewModel : ObservableObject
{
    [ObservableProperty]
    private int _nodeCount = 20;

    [ObservableProperty]
    private int _distance = 50;

    [ObservableProperty]
    private int _noise = 25;

    [ObservableProperty]
    private int _baseGridSize = 50;

    [ObservableProperty]
    private int _hierarchicalLevels = 2;
}
