using System.Threading.Tasks;

namespace ScCestinator.Services;

public interface IConfirmationDialogService
{
    Task<bool> ConfirmUninstallAsync();

    Task<bool> ConfirmAsync(
        string title,
        string message,
        string yesText = "Ano",
        string noText = "Ne");

    Task ShowInfoAsync(string title, string message);
}
