using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TDT4900_MasterThesis.message;
using TDT4900_MasterThesis.view.plot;

namespace TDT4900_MasterThesis.viewmodel;

public class SequencePlotViewModel : ObservableRecipient, IRecipient<NewGraphMessage>
{
    public SequencePlotView SequencePlotView;

    public SequencePlotViewModel(SequencePlotView sequencePlotView)
    {
        SequencePlotView = sequencePlotView;
        IsActive = true;
    }

    public void Receive(NewGraphMessage message)
    {
        SequencePlotView.Graph = message.Value;
    }
}
