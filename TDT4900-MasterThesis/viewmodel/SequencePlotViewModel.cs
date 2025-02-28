using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TDT4900_MasterThesis.engine;
using TDT4900_MasterThesis.message;
using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.view.plot;

namespace TDT4900_MasterThesis.viewmodel;

public partial class SequencePlotViewModel : ObservableRecipient, IRecipient<NewGraphMessage>
{
    public SequencePlotView SequencePlotView;

    /// <summary>
    /// Contains the complete history of node state updates
    /// </summary>
    public List<NodeStateUpdate> NodeStateUpdateHistory;

    [ObservableProperty]
    private bool _enableAutoScroll = true;

    [ObservableProperty]
    private bool _enableDataUpdate = true;

    public SequencePlotViewModel(SequencePlotView sequencePlotView)
    {
        SequencePlotView = sequencePlotView;
        IsActive = true;

        NodeStateUpdateHistory = [];
    }

    public void AppendNodeStateUpdate(NodeStateUpdate update)
    {
        if (NodeStateUpdateHistory.Count > 0 && update.Tick < NodeStateUpdateHistory[^1].Tick)
            throw new Exception(
                $"Node state update is not in order. Last tick: {NodeStateUpdateHistory[^1].Tick}, current tick: {update.Tick}"
            );

        NodeStateUpdateHistory.Add(update);
        SequencePlotView.AppendStateUpdate(update);
    }

    public void Receive(NewGraphMessage message)
    {
        lock (SimulationEngine.UpdateLock)
        {
            var graph = message.Value;
            SequencePlotView.InitGraph(graph);
        }
    }

    partial void OnEnableAutoScrollChanged(bool value)
    {
        SequencePlotView.EnableAutoScroll = value;
    }

    partial void OnEnableDataUpdateChanged(bool value)
    {
        SequencePlotView.EnableDataUpdate = value;
    }

    public void Reset()
    {
        NodeStateUpdateHistory.Clear();
        SequencePlotView.ResetView();
    }
}
