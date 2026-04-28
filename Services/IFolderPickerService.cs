using System.Threading.Tasks;

namespace ScCestinator.Services;

public interface IFolderPickerService
{
    Task<string?> PickFolderAsync(string title);
}
