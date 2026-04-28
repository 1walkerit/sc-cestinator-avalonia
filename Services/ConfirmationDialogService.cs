using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace ScCestinator.Services;

public class ConfirmationDialogService : IConfirmationDialogService
{
    private Window? _window;

    public void SetWindow(Window window)
    {
        _window = window;
    }

    public async Task<bool> ConfirmUninstallAsync()
    {
        if (_window is null)
            return false;

        var dialog = new Window
        {
            Title = "Potvrzení",
            Width = 360,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var result = false;

        var yesButton = new Button { Content = "Ano", Width = 90 };
        var noButton = new Button { Content = "Ne", Width = 90 };

        yesButton.Click += (_, _) =>
        {
            result = true;
            dialog.Close();
        };

        noButton.Click += (_, _) => dialog.Close();

        dialog.Content = new StackPanel
        {
            Margin = new Thickness(16),
            Spacing = 14,
            Children =
            {
                new TextBlock
                {
                    Text = "Opravdu chcete odinstalovat češtinu?",
                    TextWrapping = TextWrapping.Wrap
                },
                new StackPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                    Spacing = 8,
                    Children = { yesButton, noButton }
                }
            }
        };

        await dialog.ShowDialog(_window);
        return result;
    }
}
