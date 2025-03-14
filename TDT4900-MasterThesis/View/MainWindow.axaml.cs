using Avalonia.Controls;
using TDT4900_MasterThesis.ViewModel;

namespace TDT4900_MasterThesis.View;

public partial class MainWindow : Window
{
    public MainWindow(
        AppSettings appSettings,
        SequencePlotViewModel sequencePlotViewModel,
        GraphPlotViewModel graphPlotViewModel,
        MainWindowViewModel mainWindowViewModel,
        SimulationStatsViewModel simulationStatsViewModel
    )
    {
        InitializeComponent();

        Title = appSettings.WindowTitle;
        Width = appSettings.DefaultWindowWidth;
        Height = appSettings.DefaultWindowHeight;

        DataContext = mainWindowViewModel;

        GraphPlotContainer.Child = graphPlotViewModel.GraphPlotView;
        GraphPlotContainer.Parent!.DataContext = graphPlotViewModel;

        NodeSequencePlotContainer.Child = sequencePlotViewModel.SequencePlotView;
        NodeSequencePlotContainer.Parent!.DataContext = sequencePlotViewModel;

        SimulationBatchStatsContainer.DataContext = simulationStatsViewModel;
    }
}
