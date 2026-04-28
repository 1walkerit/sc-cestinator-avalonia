using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Reflection;
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
    private readonly SettingsService _settingsService = new();

    public AsyncRelayCommand InstallCommand { get; }
    public AsyncRelayCommand BrowseFolderCommand { get; }

    public string AppVersion { get; }

    public MainWindowViewModel(IFolderPickerService folderPickerService)
    {
        _folderPickerService = folderPickerService;

        // Get app version from assembly
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        AppVersion = $"Verze: {version?.Major}.{version?.Minor}.{version?.Build ?? 0}";

        InstallCommand = new AsyncRelayCommand(
            execute: InstallAsync,
            canExecute: () => IsUpdateAvailable && !IsBusy
        );

        BrowseFolderCommand = new AsyncRelayCommand(
            execute: BrowseFolderAsync,
            canExecute: () => !IsBusy
        );

        // Load last used path
        var settings = _settingsService.LoadSettings();
        if (!string.IsNullOrWhiteSpace(settings.LastUsedPath))
        {
            InputPath = settings.LastUsedPath;
        }
    }

    private string? _inputPath;
    public string? InputPath
    {
        get => _inputPath;
        set
        {
            _inputPath = value;
            OnPropertyChanged();
            
            // Fire and forget with proper error handling
            _ = ValidatePathAsync();
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
            InstallCommand?.RaiseCanExecuteChanged();
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
            InstallCommand?.RaiseCanExecuteChanged();
            BrowseFolderCommand?.RaiseCanExecuteChanged();
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
                
                // Save path when selected via folder picker
                _settingsService.SaveSettings(new AppSettings { LastUsedPath = selectedPath });
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

    private async Task ValidatePathAsync()
    {
        try
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
            }
            else if (LocalVersion != OnlineVersion)
            {
                Status = $"Je dostupná aktualizace ({LocalVersion} → {OnlineVersion})";
                IsUpdateAvailable = true;
            }
            else
            {
                Status = "Máte aktuální verzi ✔";
                IsUpdateAvailable = false;
            }
            
            // Save path when validation succeeds (path is valid and has data folder)
            if (!string.IsNullOrWhiteSpace(InputPath))
            {
                _settingsService.SaveSettings(new AppSettings { LastUsedPath = InputPath });
            }
        }
        catch
        {
            // Silently handle validation errors to avoid crashing on property change
            Status = "Chyba při ověřování cesty";
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
            await ValidatePathAsync();
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