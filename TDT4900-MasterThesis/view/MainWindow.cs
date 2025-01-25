using Gtk;

namespace TDT4900_MasterThesis.view;

public class MainWindow : Window
{
    private readonly MainCanvas _canvasView;

    public MainWindow(AppSettings appSettings, MainCanvas canvasView)
        : base(appSettings.WindowTitle)
    {
        SetDefaultSize(appSettings.DefaultWindowWidth, appSettings.DefaultWindowHeight);
        _canvasView = canvasView;

        Add(_canvasView);
    }
}
