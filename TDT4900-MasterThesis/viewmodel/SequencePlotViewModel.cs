using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TDT4900_MasterThesis.Engine;
using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Message;
using TDT4900_MasterThesis.Model;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Model.Graph;
using TDT4900_MasterThesis.view.plot;

namespace TDT4900_MasterThesis.viewmodel;

public partial class SequencePlotViewModel : ObservableObject, IEventConsumer
{
    public SequencePlotView SequencePlotView;

    [ObservableProperty]
    private bool _enableAutoScroll = true;

    [ObservableProperty]
    private bool _enableDataUpdate = true;

    public SequencePlotViewModel(SequencePlotView sequencePlotView)
    {
        SequencePlotView = sequencePlotView;
    }

    [Obsolete("Use ConsumeEvent instead")]
    public void AppendNodeStateUpdate(NodeStateUpdate update) { }

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
