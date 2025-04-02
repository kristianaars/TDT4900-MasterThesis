using CommunityToolkit.Mvvm.ComponentModel;
using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.View.Plot;

namespace TDT4900_MasterThesis.ViewModel;

public partial class GraphPlotViewModel : ObservableObject, IEventConsumer
{
    public GraphPlotView GraphPlotView;

    [ObservableProperty]
    private Graph? _graph;

    [ObservableProperty]
    private Node _startNode;

    [ObservableProperty]
    private Node _targetNode;

    [ObservableProperty]
    private bool _enableDataUpdate;

    public GraphPlotViewModel(GraphPlotView graphPlotView)
    {
        GraphPlotView = graphPlotView;
        //EnableDataUpdate = graphPlotView.EnableDataUpdate;
    }

    partial void OnEnableDataUpdateChanged(bool value)
    {
        //GraphPlotView.EnableDataUpdate = value;
    }

    public void ConsumeEvent(NodeEvent nodeEvent)
    {
        GraphPlotView.AppendNodeEvent(nodeEvent);
    }

    partial void OnGraphChanged(Graph? value)
    {
        if (value != null)
            GraphPlotView.InitializeGraph(Graph!);
    }

    partial void OnStartNodeChanged(Node value)
    {
        GraphPlotView.SetStartNode(value);
    }

    partial void OnTargetNodeChanged(Node value)
    {
        GraphPlotView.SetTargetNode(value);
    }
}
