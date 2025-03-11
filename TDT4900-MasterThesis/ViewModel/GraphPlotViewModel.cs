using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TDT4900_MasterThesis.Engine;
using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Message;
using TDT4900_MasterThesis.Model;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Model.Graph;
using TDT4900_MasterThesis.View.Plot;

namespace TDT4900_MasterThesis.ViewModel;

public partial class GraphPlotViewModel : ObservableObject, IEventConsumer
{
    public GraphPlotView GraphPlotView;

    [ObservableProperty]
    private bool _enableDataUpdate;

    public GraphPlotViewModel(GraphPlotView graphPlotView)
    {
        GraphPlotView = graphPlotView;
        EnableDataUpdate = graphPlotView.EnableDataUpdate;
    }

    [Obsolete("Use ConsumeEvent instead")]
    public void AppendStateUpdate(NodeStateUpdate nodeStateUpdate)
    {
        //GraphPlotView.AppendNodeEvent(nodeStateUpdate);
    }

    partial void OnEnableDataUpdateChanged(bool value)
    {
        GraphPlotView.EnableDataUpdate = value;
    }

    public void ConsumeEvent(NodeEvent nodeEvent)
    {
        GraphPlotView.AppendNodeEvent(nodeEvent);
    }

    public void InitializeGraph(Graph graph)
    {
        GraphPlotView.InitializeGraph(graph);
    }
}
