using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TDT4900_MasterThesis.View.Controls;

public partial class LabeledComboBox : UserControl
{
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<
        LabeledComboBox,
        string
    >(nameof(Label));

    public static readonly StyledProperty<string> DescriptionProperty = AvaloniaProperty.Register<
        LabeledComboBox,
        string
    >(nameof(Description));

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<
        LabeledComboBox,
        string
    >(nameof(Text), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<IEnumerable<object>?> ItemsSourceProperty =
        AvaloniaProperty.Register<LabeledComboBox, IEnumerable<object>?>(
            nameof(ItemsSource),
            defaultBindingMode: Avalonia.Data.BindingMode.OneWay
        );

    public static readonly StyledProperty<object> SelectedItemProperty = AvaloniaProperty.Register<
        LabeledComboBox,
        object
    >(nameof(ItemsSource), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

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

    public IEnumerable<object>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public LabeledComboBox()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
