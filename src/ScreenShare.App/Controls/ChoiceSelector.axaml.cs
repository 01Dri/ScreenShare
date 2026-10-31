using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace ScreenShare.App.Controls;

/// <summary>
/// Seletor de duas opções (ícone + rótulo), com uma legenda para cada opção.
/// A escolha é exposta por <see cref="IsFirstSelected"/>, pronto para binding bidirecional.
/// </summary>
public partial class ChoiceSelector : UserControl
{
    public static readonly StyledProperty<bool> IsFirstSelectedProperty =
        AvaloniaProperty.Register<ChoiceSelector, bool>(
            nameof(IsFirstSelected),
            defaultValue: true,
            defaultBindingMode: BindingMode.TwoWay);
    
    public static readonly StyledProperty<string?> FirstLabelProperty =
        AvaloniaProperty.Register<ChoiceSelector, string?>(nameof(FirstLabel));

    public static readonly StyledProperty<Geometry?> FirstIconProperty =
        AvaloniaProperty.Register<ChoiceSelector, Geometry?>(nameof(FirstIcon));

    public static readonly StyledProperty<string?> FirstHintProperty =
        AvaloniaProperty.Register<ChoiceSelector, string?>(nameof(FirstHint));

    public static readonly StyledProperty<string?> SecondLabelProperty =
        AvaloniaProperty.Register<ChoiceSelector, string?>(nameof(SecondLabel));

    public static readonly StyledProperty<Geometry?> SecondIconProperty =
        AvaloniaProperty.Register<ChoiceSelector, Geometry?>(nameof(SecondIcon));

    public static readonly StyledProperty<string?> SecondHintProperty =
        AvaloniaProperty.Register<ChoiceSelector, string?>(nameof(SecondHint));

    /// <summary><c>true</c> quando a primeira opção está selecionada.</summary>
    public bool IsFirstSelected
    {
        get => GetValue(IsFirstSelectedProperty);
        set => SetValue(IsFirstSelectedProperty, value);
    }

    public string? FirstLabel
    {
        get => GetValue(FirstLabelProperty);
        set => SetValue(FirstLabelProperty, value);
    }

    public Geometry? FirstIcon
    {
        get => GetValue(FirstIconProperty);
        set => SetValue(FirstIconProperty, value);
    }

    public string? FirstHint
    {
        get => GetValue(FirstHintProperty);
        set => SetValue(FirstHintProperty, value);
    }

    public string? SecondLabel
    {
        get => GetValue(SecondLabelProperty);
        set => SetValue(SecondLabelProperty, value);
    }

    public Geometry? SecondIcon
    {
        get => GetValue(SecondIconProperty);
        set => SetValue(SecondIconProperty, value);
    }

    public string? SecondHint
    {
        get => GetValue(SecondHintProperty);
        set => SetValue(SecondHintProperty, value);
    }

    public ChoiceSelector()
    {
        InitializeComponent();
        UpdateSelection();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsFirstSelectedProperty)
        {
            UpdateSelection();
        }
    }

    private void UpdateSelection()
    {
        FirstOption.IsChecked = IsFirstSelected;
        SecondOption.IsChecked = !IsFirstSelected;
    }

    private void OnFirstOptionIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (FirstOption.IsChecked == true)
        {
            SetCurrentValue(IsFirstSelectedProperty, true);
        }
    }

    private void OnSecondOptionIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (SecondOption.IsChecked == true)
        {
            SetCurrentValue(IsFirstSelectedProperty, false);
        }
    }
}
