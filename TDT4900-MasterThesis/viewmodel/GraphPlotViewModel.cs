using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TDT4900_MasterThesis.engine;
using TDT4900_MasterThesis.message;
using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.view.plot;

namespace TDT4900_MasterThesis.viewmodel;

public class GraphPlotViewModel : ObservableRecipient, IRecipient<NewGraphMessage>
{
    public GraphPlotView GraphPlotView;

    public GraphPlotViewModel(GraphPlotView graphPlotView)
    {
        GraphPlotView = graphPlotView;
        IsActive = true;
    }

    public void AppendStateUpdate(NodeStateUpdate nodeStateUpdate)
    {
        GraphPlotView.AppendStateHistory(nodeStateUpdate);
    }

    /// <summary>
    /// Subscription function for receiving new graph for the simulation
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Receive(NewGraphMessage message)
    {
        lock (SimulationEngine.UpdateLock)
        {
            var graph = message.Value;
            GraphPlotView.Init(graph);
        }
    }

    public void Reset()
    {
        //GraphPlotView.ResetView();
    }
}
