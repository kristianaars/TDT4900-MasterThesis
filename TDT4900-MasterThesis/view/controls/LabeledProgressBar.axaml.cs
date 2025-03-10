using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TDT4900_MasterThesis.view.controls;

public partial class LabeledProgressBar : UserControl
{
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<
        LabeledProgressBar,
        string
    >(nameof(Label));

    public static readonly StyledProperty<string> DescriptionProperty = AvaloniaProperty.Register<
        LabeledProgressBar,
        string
    >(nameof(Description));

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<
        LabeledProgressBar,
        string
    >(nameof(Text), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<int> PercentProperty = AvaloniaProperty.Register<
        LabeledProgressBar,
        int
    >(nameof(Text), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public int Percent
    {
        get => GetValue(PercentProperty);
        set => SetValue(PercentProperty, value);
    }

    public LabeledProgressBar()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
