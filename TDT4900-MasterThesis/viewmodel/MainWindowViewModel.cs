using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TDT4900_MasterThesis.factory;
using TDT4900_MasterThesis.message;
using TDT4900_MasterThesis.model.graph;

namespace TDT4900_MasterThesis.viewmodel;

public partial class MainWindowViewModel : ObservableObject
{
    private SequencePlotViewModel _sequencePlotViewModel;
    private GraphPlotViewModel _graphPlotViewModel;
    private AppSettings _appSettings;

    [ObservableProperty]
    private string _graphSettingsNodeCount;

    [ObservableProperty]
    private string _graphSettingsEdgeCount;

    public MainWindowViewModel(
        SequencePlotViewModel sequencePlotViewModel,
        GraphPlotViewModel graphPlotViewModel,
        AppSettings appSettings
    )
    {
        _sequencePlotViewModel = sequencePlotViewModel;
        _graphPlotViewModel = graphPlotViewModel;
        _appSettings = appSettings;
    }

    [RelayCommand]
    private void ResetSimulation()
    {
        var nodeCount = int.Parse(GraphSettingsNodeCount);
        var edgeCount = int.Parse(GraphSettingsEdgeCount);

        var f = new RandomGraphFactory(nodeCount, edgeCount);

        Graph graph = f.GetGraph();

        graph.Nodes.ForEach(n => n.SimulationSettings = _appSettings.Simulation);

        graph.Nodes[5].IsTagged = true;

        WeakReferenceMessenger.Default.Send(new NewGraphMessage(graph));
    }
}
