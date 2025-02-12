using Avalonia.Controls;
using Avalonia.Interactivity;
using TDT4900_MasterThesis.engine;
using TDT4900_MasterThesis.view.plot;

namespace TDT4900_MasterThesis.view;

public partial class MainWindow : Window
{
    private readonly GraphPlotView _graphPlotView;

    private readonly SimulationEngine _engine;

    public MainWindow(
        GraphPlotView graphPlotView,
        SequencePlotView sequencePlotView,
        SimulationEngine engine,
        AppSettings appSettings
    )
    {
        InitializeComponent();

        _engine = engine;

        Title = appSettings.WindowTitle;
        Width = appSettings.DefaultWindowWidth;
        Height = appSettings.DefaultWindowHeight;

        GraphPlotContainer.Child = graphPlotView;
        NodeSequencePlotContainer.Child = sequencePlotView;
    }

    private void Pause_OnClick(object? sender, RoutedEventArgs e)
    {
        _engine.Pause();
    }

    private void Play_OnClick(object? sender, RoutedEventArgs e)
    {
        _engine.Play();
    }

    private void Restart_OnClick(object? sender, RoutedEventArgs e)
    {
        _engine.Restart();
    }
}
