using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TDT4900_MasterThesis.View.Controls;

public partial class LabeledTextBox : UserControl
{
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<
        LabeledTextBox,
        string
    >(nameof(Label));

    public static readonly StyledProperty<string> DescriptionProperty = AvaloniaProperty.Register<
        LabeledTextBox,
        string
    >(nameof(Description));

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<
        LabeledTextBox,
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

    public LabeledTextBox()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
