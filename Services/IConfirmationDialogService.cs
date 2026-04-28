using System.Threading.Tasks;

namespace ScCestinator.Services;

public interface IConfirmationDialogService
{
    Task<bool> ConfirmUninstallAsync();
}
