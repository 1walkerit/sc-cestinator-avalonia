using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using ScCestinator.ViewModels;
using ScCestinator.Views;

namespace ScCestinator;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var folderPickerService = new Services.FolderPickerService();
            var confirmationDialogService = new Services.ConfirmationDialogService();
            var viewModel = new MainWindowViewModel(folderPickerService, confirmationDialogService);
            var mainWindow = new MainWindow
            {
                DataContext = viewModel
            };
            
            folderPickerService.SetWindow(mainWindow);
            confirmationDialogService.SetWindow(mainWindow);
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
