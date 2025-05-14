using CommunityToolkit.Mvvm.ComponentModel;
using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.View.Plot;

namespace TDT4900_MasterThesis.ViewModel;

public partial class GraphPlotViewModel : ObservableObject, IAlgorithmEventConsumer
{
    public GraphPlotView GraphPlotView;

    private Graph? _graph;

    public Graph Graph
    {
        get => _graph;
        set => OnGraphChanged(value);
    }

    private Node _startNode;

    public Node StartNode
    {
        get => _startNode;
        set => GraphPlotView.SetStartNode(value);
    }

    private Node _targetNode;

    public Node TargetNode
    {
        get => _targetNode;
        set => GraphPlotView.SetTargetNode(value);
    }

    [ObservableProperty]
    private bool _enableDataUpdate;

    [ObservableProperty]
    private int _layerCount;

    [ObservableProperty]
    private int _maxVisibleLayer;

    public GraphPlotViewModel(GraphPlotView graphPlotView)
    {
        GraphPlotView = graphPlotView;
        //EnableDataUpdate = graphPlotView.EnableDataUpdate;
    }

    partial void OnEnableDataUpdateChanged(bool value)
    {
        //GraphPlotView.EnableDataUpdate = value;
    }

    partial void OnMaxVisibleLayerChanged(int value)
    {
        GraphPlotView.MaxVisibleLayer = value;
        GraphPlotView.Draw();
    }

    private void OnGraphChanged(Graph graph)
    {
        _graph = graph;
        GraphPlotView.InitializeGraph(graph);
        LayerCount = graph.Edges.Max(e => e.Level) + 1;

        MaxVisibleLayer = Math.Min(MaxVisibleLayer, LayerCount);
    }

    public void ConsumeEvent(AlgorithmEvent algorithmEvent)
    {
        GraphPlotView.AppendAlgorithmEvent(algorithmEvent);
    }
}
