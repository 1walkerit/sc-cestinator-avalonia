using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using ScCestinator.Services;

namespace ScCestinator.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly PathService _pathService = new();
    private readonly LocalizationService _localizationService = new();
    private readonly GitHubService _gitHubService = new();
    private readonly IFolderPickerService _folderPickerService;

    public ICommand InstallCommand { get; }
    public ICommand BrowseFolderCommand { get; }

    public MainWindowViewModel(IFolderPickerService folderPickerService)
    {
        _folderPickerService = folderPickerService;

        InstallCommand = new RelayCommand(
            execute: async () => await InstallAsync(),
            canExecute: () => IsUpdateAvailable && !IsBusy
        );

        BrowseFolderCommand = new AsyncRelayCommand(
            execute: BrowseFolderAsync,
            canExecute: () => !IsBusy
        );
    }

    private string? _inputPath;
    public string? InputPath
    {
        get => _inputPath;
        set
        {
            _inputPath = value;
            OnPropertyChanged();
            ValidatePath();
        }
    }

    private string _status = "Zadej cestu ke Star Citizen";
    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    private string _localVersion = "-";
    public string LocalVersion
    {
        get => _localVersion;
        set
        {
            _localVersion = value;
            OnPropertyChanged();
        }
    }

    private string _onlineVersion = "-";
    public string OnlineVersion
    {
        get => _onlineVersion;
        set
        {
            _onlineVersion = value;
            OnPropertyChanged();
        }
    }

    private bool _isUpdateAvailable;
    public bool IsUpdateAvailable
    {
        get => _isUpdateAvailable;
        set
        {
            _isUpdateAvailable = value;
            OnPropertyChanged();
            (InstallCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged();
            (InstallCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (BrowseFolderCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    private async Task BrowseFolderAsync()
    {
        try
        {
            Status = "Otevírám dialog...";

            var selectedPath = await _folderPickerService.PickFolderAsync("Vyberte složku Star Citizen");

            if (selectedPath != null)
            {
                InputPath = selectedPath;
            }
            else
            {
                Status = "Výběr složky zrušen";
            }
        }
        catch (Exception ex)
        {
            Status = $"Chyba: {ex.Message}";
        }
    }

    private async void ValidatePath()
    {
        var result = _pathService.ValidateStarCitizenPath(InputPath);

        LocalVersion = "-";
        OnlineVersion = "-";
        IsUpdateAvailable = false;

        if (result.LivePath == null)
        {
            Status = "Neplatná cesta";
            return;
        }

        if (!result.LiveExists)
        {
            Status = $"LIVE složka neexistuje: {result.LivePath}";
            return;
        }

        if (!result.DataExists)
        {
            Status = "Chybí složka data";
            return;
        }

        if (result.GlobalIniExists)
        {
            var local = await _localizationService.ReadLocalVersionAsync(result.GlobalIniPath);
            LocalVersion = local ?? "neznámá";
        }
        else
        {
            LocalVersion = "nenainstalováno";
        }

        var online = await _gitHubService.GetOnlineVersionAsync();
        OnlineVersion = online ?? "neznámá";

        if (LocalVersion == "nenainstalováno")
        {
            Status = "Lokalizace není nainstalována";
            IsUpdateAvailable = true;
            return;
        }

        if (LocalVersion != OnlineVersion)
        {
            Status = $"Je dostupná aktualizace ({LocalVersion} → {OnlineVersion})";
            IsUpdateAvailable = true;
        }
        else
        {
            Status = "Máte aktuální verzi ✔";
            IsUpdateAvailable = false;
        }
    }

    public async Task InstallAsync()
    {
        if (string.IsNullOrWhiteSpace(InputPath))
            return;

        Status = "Kontroluji cestu ke Star Citizenu...";
        var result = _pathService.ValidateStarCitizenPath(InputPath);

        if (result.LivePath == null)
            return;

        IsBusy = true;

        try
        {
            await _localizationService.InstallOrUpdateAsync(
                result.LivePath,
                true,
                progress => Status = progress
            );
            
            Status = "Hotovo ✔";
            ValidatePath();
        }
        catch (HttpRequestException ex)
        {
            Status = $"Chyba sítě: {ex.Message}";
        }
        catch (TaskCanceledException)
        {
            Status = "Chyba: Vypršel časový limit připojení";
        }
        catch (UnauthorizedAccessException)
        {
            Status = "Chyba: Přístup odepřen - zkontrolujte oprávnění";
        }
        catch (IOException ex)
        {
            Status = $"Chyba I/O: {ex.Message}";
        }
        catch (Exception ex)
        {
            Status = $"Chyba při instalaci: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}