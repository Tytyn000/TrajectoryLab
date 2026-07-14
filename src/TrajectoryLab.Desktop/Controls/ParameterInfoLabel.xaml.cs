using System.Windows;
using System.Windows.Controls;

namespace TrajectoryLab.Desktop.Controls;

public partial class ParameterInfoLabel :
    UserControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(
            nameof(Label),
            typeof(string),
            typeof(ParameterInfoLabel),
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty HelpTextProperty =
        DependencyProperty.Register(
            nameof(HelpText),
            typeof(string),
            typeof(ParameterInfoLabel),
            new PropertyMetadata(string.Empty));

    public ParameterInfoLabel()
    {
        InitializeComponent();
    }

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string HelpText
    {
        get => (string)GetValue(HelpTextProperty);
        set => SetValue(HelpTextProperty, value);
    }
}