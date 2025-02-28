using Avalonia.Controls;
using TDT4900_MasterThesis.viewmodel;

namespace TDT4900_MasterThesis.view;

public partial class MainWindow : Window
{
    public MainWindow(
        AppSettings appSettings,
        SequencePlotViewModel sequencePlotViewModel,
        GraphPlotViewModel graphPlotViewModel,
        MainWindowViewModel mainWindowViewModel
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
    }
}
