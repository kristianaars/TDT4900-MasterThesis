using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TDT4900_MasterThesis.View.Controls;

public partial class LabeledTextField : UserControl
{
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<
        LabeledTextField,
        string
    >(nameof(Label));

    public static readonly StyledProperty<string> DescriptionProperty = AvaloniaProperty.Register<
        LabeledTextField,
        string
    >(nameof(Description));

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<
        LabeledTextField,
        string
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

    public LabeledTextField()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
