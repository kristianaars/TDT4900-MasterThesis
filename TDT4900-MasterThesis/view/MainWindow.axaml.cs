using Avalonia.Controls;
using TDT4900_MasterThesis.view.plot;

namespace TDT4900_MasterThesis.view;

public partial class MainWindow : Window
{
    private readonly GraphPlotView _graphPlotView;

    public MainWindow(GraphPlotView graphPlotView, SequencePlotView sequencePlotView)
    {
        InitializeComponent();

        GraphPlotContainer.Children.Add(graphPlotView);
        NodeSequencePlotContainer.Children.Add(sequencePlotView);
    }
}
