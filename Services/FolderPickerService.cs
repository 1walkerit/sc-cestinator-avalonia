using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace ScCestinator.Services;

public class FolderPickerService : IFolderPickerService
{
    private Window? _window;

    public void SetWindow(Window window)
    {
        _window = window;
    }

    public async Task<string?> PickFolderAsync(string title)
    {
        if (_window == null)
            return null;

        var storageProvider = _window.StorageProvider;

        if (!storageProvider.CanOpen)
            return null;

        var result = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });

        if (result.Count > 0)
        {
            var folder = result.First();
            return folder.Path.LocalPath;
        }

        return null;
    }
}
