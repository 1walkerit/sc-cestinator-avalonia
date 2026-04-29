using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;

namespace ScCestinator.Services;

public class ConfirmationDialogService : IConfirmationDialogService
{
    private Window? _window;

    public void SetWindow(Window window)
    {
        _window = window;
    }


    public async Task<bool> ConfirmAsync(string title, string message, string yesText = "Ano", string noText = "Ne")
    {
        if (_window is null)
            return false;

        var dialog = new Window
        {
            Title = title,
            Width = 420,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Background = new SolidColorBrush(Color.Parse("#BB101820"))
        };

        var result = false;

        var yesButton = new Button
        {
            Content = yesText,
            Width = 120,
            Height = 40,
            Background = new SolidColorBrush(Color.Parse("#AA4444")),
            Foreground = Brushes.White,
            FontWeight = FontWeight.SemiBold,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Padding = new Thickness(12, 6)
        };
        var yesNormalBackground = new SolidColorBrush(Color.Parse("#AA4444"));
        var yesHoverBackground = new SolidColorBrush(Color.Parse("#963D3D"));
        yesButton.Background = yesNormalBackground;
        yesButton.PointerEntered += (_, _) => yesButton.Background = yesHoverBackground;
        yesButton.PointerExited += (_, _) => yesButton.Background = yesNormalBackground;

        var noButton = new Button
        {
            Content = noText,
            Width = 120,
            Height = 40,
            Background = new SolidColorBrush(Color.Parse("#6A6A6A")),
            Foreground = Brushes.White,
            FontWeight = FontWeight.SemiBold,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Padding = new Thickness(12, 6)
        };
        var noNormalBackground = new SolidColorBrush(Color.Parse("#6A6A6A"));
        var noHoverBackground = new SolidColorBrush(Color.Parse("#5E5E5E"));
        noButton.Background = noNormalBackground;
        noButton.PointerEntered += (_, _) => noButton.Background = noHoverBackground;
        noButton.PointerExited += (_, _) => noButton.Background = noNormalBackground;

        yesButton.Click += (_, _) =>
        {
            result = true;
            dialog.Close();
        };

        noButton.Click += (_, _) => dialog.Close();

        dialog.Content = new Border
        {
            Margin = new Thickness(16),
            Padding = new Thickness(18),
            CornerRadius = new CornerRadius(10),
            Background = new SolidColorBrush(Color.Parse("#E6FFFFFF")),
            Child = new StackPanel
            {
                Spacing = 16,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        FontSize = 17,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = new SolidColorBrush(Color.Parse("#1C1C1C")),
                        TextWrapping = TextWrapping.Wrap,
                        TextAlignment = TextAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Spacing = 12,
                        Children = { yesButton, noButton }
                    }
                }
            }
        };

        await dialog.ShowDialog(_window);
        return result;
    }

    public async Task<bool> ConfirmUninstallAsync()
    {
        if (_window is null)
            return false;

        var dialog = new Window
        {
            Title = "Potvrzení odinstalace",
            Width = 420,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Background = new SolidColorBrush(Color.Parse("#BB101820"))
        };

        var result = false;

        var yesButton = new Button
        {
            Content = "Ano",
            Width = 120,
            Height = 40,
            Background = new SolidColorBrush(Color.Parse("#AA4444")),
            Foreground = Brushes.White,
            FontWeight = FontWeight.SemiBold,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Padding = new Thickness(12, 6)
        };
        var yesNormalBackground = new SolidColorBrush(Color.Parse("#AA4444"));
        var yesHoverBackground = new SolidColorBrush(Color.Parse("#963D3D"));
        yesButton.Background = yesNormalBackground;
        yesButton.PointerEntered += (_, _) => yesButton.Background = yesHoverBackground;
        yesButton.PointerExited += (_, _) => yesButton.Background = yesNormalBackground;

        var noButton = new Button
        {
            Content = "Ne",
            Width = 120,
            Height = 40,
            Background = new SolidColorBrush(Color.Parse("#6A6A6A")),
            Foreground = Brushes.White,
            FontWeight = FontWeight.SemiBold,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Padding = new Thickness(12, 6)
        };
        var noNormalBackground = new SolidColorBrush(Color.Parse("#6A6A6A"));
        var noHoverBackground = new SolidColorBrush(Color.Parse("#5E5E5E"));
        noButton.Background = noNormalBackground;
        noButton.PointerEntered += (_, _) => noButton.Background = noHoverBackground;
        noButton.PointerExited += (_, _) => noButton.Background = noNormalBackground;

        yesButton.Click += (_, _) =>
        {
            result = true;
            dialog.Close();
        };

        noButton.Click += (_, _) => dialog.Close();

        dialog.Content = new Border
        {
            Margin = new Thickness(16),
            Padding = new Thickness(18),
            CornerRadius = new CornerRadius(10),
            Background = new SolidColorBrush(Color.Parse("#E6FFFFFF")),
            Child = new StackPanel
            {
                Spacing = 16,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Opravdu chcete odinstalovat češtinu?",
                        FontSize = 17,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = new SolidColorBrush(Color.Parse("#1C1C1C")),
                        TextWrapping = TextWrapping.Wrap,
                        TextAlignment = TextAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Spacing = 12,
                        Children = { yesButton, noButton }
                    }
                }
            }
        };

        await dialog.ShowDialog(_window);
        return result;
    }
}
