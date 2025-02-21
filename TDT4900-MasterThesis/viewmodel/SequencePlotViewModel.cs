using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TDT4900_MasterThesis.message;
using TDT4900_MasterThesis.view.plot;

namespace TDT4900_MasterThesis.viewmodel;

public partial class SequencePlotViewModel : ObservableRecipient, IRecipient<NewGraphMessage>
{
    public SequencePlotView SequencePlotView;

    [ObservableProperty]
    private bool _enableAutoScroll = true;

    public SequencePlotViewModel(SequencePlotView sequencePlotView)
    {
        SequencePlotView = sequencePlotView;
        IsActive = true;
    }

    public void Receive(NewGraphMessage message)
    {
        SequencePlotView.Graph = message.Value;
    }

    partial void OnEnableAutoScrollChanged(bool value)
    {
        SequencePlotView.EnableAutoScroll = value;
    }
}
