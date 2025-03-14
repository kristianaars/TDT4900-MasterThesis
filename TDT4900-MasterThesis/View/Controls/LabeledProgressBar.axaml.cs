using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TDT4900_MasterThesis.View.Controls;

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

    public static readonly StyledProperty<int> ValueProperty = AvaloniaProperty.Register<
        LabeledProgressBar,
        int
    >(nameof(Value), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<int> MaximumProperty = AvaloniaProperty.Register<
        LabeledProgressBar,
        int
    >(nameof(Maximum), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<string> ProgressTextFormatProperty =
        AvaloniaProperty.Register<LabeledProgressBar, string>(
            nameof(ProgressTextFormat),
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay
        );
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

    public int Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public object Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public string ProgressTextFormat
    {
        get => GetValue(ProgressTextFormatProperty);
        set => SetValue(ProgressTextFormatProperty, value);
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
