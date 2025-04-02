using CommunityToolkit.Mvvm.ComponentModel;
using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Model.Graph;
using TDT4900_MasterThesis.View.Plot;

namespace TDT4900_MasterThesis.ViewModel;

public partial class SequencePlotViewModel : ObservableObject, IEventConsumer
{
    public SequencePlotView SequencePlotView;

    [ObservableProperty]
    private bool _enableAutoScroll = false;

    [ObservableProperty]
    private bool _enableDataUpdate = false;

    public SequencePlotViewModel(SequencePlotView sequencePlotView)
    {
        SequencePlotView = sequencePlotView;
    }

    partial void OnEnableAutoScrollChanged(bool value)
    {
        SequencePlotView.EnableAutoScroll = value;
    }

    partial void OnEnableDataUpdateChanged(bool value)
    {
        SequencePlotView.EnableDataUpdate = value;
    }

    public void InitializeGraph(Graph graph)
    {
        SequencePlotView.InitializeGraph(graph);
    }

    public void ConsumeEvent(NodeEvent nodeEvent)
    {
        SequencePlotView.AppendNodeEvent(nodeEvent);
    }
}
