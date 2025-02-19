using Avalonia.Controls;
using Avalonia.Interactivity;
using TDT4900_MasterThesis.engine;
using TDT4900_MasterThesis.view.plot;
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

        GraphPlotContainer.DataContext = graphPlotViewModel;
        GraphPlotContainer.Child = graphPlotViewModel.GraphPlotView;

        NodeSequencePlotContainer.Child = sequencePlotViewModel.SequencePlotView;
        NodeSequencePlotContainer.DataContext = sequencePlotViewModel;
    }
}
