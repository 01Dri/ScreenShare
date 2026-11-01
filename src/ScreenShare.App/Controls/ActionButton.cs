using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace ScreenShare.App.Controls;

/// <summary>
/// Botão de ação com ícone à esquerda. Use a classe <c>primary</c> para a ação principal;
/// sem classe, assume o visual secundário do design.
/// </summary>
public class ActionButton : Button
{
    public static readonly StyledProperty<Geometry?> IconProperty =
        AvaloniaProperty.Register<ActionButton, Geometry?>(nameof(Icon));

    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<ActionButton, double>(nameof(IconSize), defaultValue: 18d);

    public Geometry? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
}
