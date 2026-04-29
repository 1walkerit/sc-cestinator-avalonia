using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media;
using ScCestinator.Services;

namespace ScCestinator.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly PathService _pathService = new();
    private readonly LocalizationService _localizationService = new();
    private readonly GitHubService _gitHubService = new();
    private readonly IFolderPickerService _folderPickerService;
    private readonly IConfirmationDialogService _confirmationDialogService;
    private readonly SettingsService _settingsService = new();

    public AsyncRelayCommand InstallCommand { get; }
    public AsyncRelayCommand UninstallCommand { get; }
    public AsyncRelayCommand BrowseFolderCommand { get; }
    public ICommand OpenUrlCommand { get; }
    public ICommand DownloadLatestVersionCommand { get; }
    public ICommand OpenDownloadFolderCommand { get; }

    public ICommand OpenGameFolderCommand { get; }
    public ICommand ClearLogsCommand { get; }
    public ICommand ClearShadersCommand { get; }


    public string AppVersion { get; }

    private string _appUpdateStatus = "";
    public string AppUpdateStatus
    {
        get => _appUpdateStatus;
        set
        {
            _appUpdateStatus = value;
            OnPropertyChanged();
        }
    }


    public MainWindowViewModel(
        IFolderPickerService folderPickerService,
        IConfirmationDialogService confirmationDialogService)
    {
        _folderPickerService = folderPickerService;
        _confirmationDialogService = confirmationDialogService;

        // Get app version from assembly
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        AppVersion = $"Verze: {version?.Major}.{version?.Minor}.{version?.Build ?? 0}";
  

        InstallCommand = new AsyncRelayCommand(
            execute: InstallAsync,
            canExecute: () => IsUpdateAvailable && !IsBusy
        );

        UninstallCommand = new AsyncRelayCommand(
            execute: UninstallAsync,
            canExecute: () => LocalVersion != "-" && LocalVersion != "nenainstalováno" && !IsBusy
        );

        BrowseFolderCommand = new AsyncRelayCommand(
            execute: BrowseFolderAsync,
            canExecute: () => !IsBusy
        );

        OpenUrlCommand = new RelayCommand(param =>
        {
            try
            {
                if (param is not string url || string.IsNullOrWhiteSpace(url))
                    return;

                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch
            {
                Status = "Nepodařilo se otevřít odkaz.";
            }
        });
DownloadLatestVersionCommand = new AsyncRelayCommand(DownloadLatestVersionAsync);
        OpenDownloadFolderCommand = new RelayCommand(OpenDownloadFolder);

        OpenGameFolderCommand = new RelayCommand(OpenGameFolder);
        ClearLogsCommand = new RelayCommand(ClearLogs);
        ClearShadersCommand = new RelayCommand(ClearShaders);


        // Load last used path
        var settings = _settingsService.LoadSettings();
        if (!string.IsNullOrWhiteSpace(settings.LastUsedPath))
        {
            InputPath = settings.LastUsedPath;
        }

        // Check for app update automatically on startup
        _ = CheckAppUpdateAsync();
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
            UpdateStatusBrush();
        }
    }

    private IBrush _statusBrush = Brushes.Black;
    public IBrush StatusBrush
    {
        get => _statusBrush;
        private set
        {
            _statusBrush = value;
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
            UninstallCommand?.RaiseCanExecuteChanged();
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


    private bool _isAppUpdateAvailable;
    
    private double _downloadProgress;
    public double DownloadProgress
    {
        get => _downloadProgress;
        set
        {
            _downloadProgress = value;
            OnPropertyChanged();
        }
    }

    private bool _isDownloading;
    public bool IsDownloading
    {
        get => _isDownloading;
        set
        {
            _isDownloading = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowDownloadButton));
        }
    }



    private string _lastDownloadFolder = "";
    public string LastDownloadFolder
    {
        get => _lastDownloadFolder;
        set
        {
            _lastDownloadFolder = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanOpenDownloadFolder));
        }
    }

    public bool CanOpenDownloadFolder => !string.IsNullOrWhiteSpace(LastDownloadFolder) && Directory.Exists(LastDownloadFolder);

    public bool ShowDownloadButton => IsAppUpdateAvailable && !IsDownloading;

    public bool IsAppUpdateAvailable
    {
        get => _isAppUpdateAvailable;
        set
        {
            _isAppUpdateAvailable = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowDownloadButton));
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
            UninstallCommand?.RaiseCanExecuteChanged();
            BrowseFolderCommand?.RaiseCanExecuteChanged();
        }
    }




    
    private void OpenGameFolder()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(InputPath) || !Directory.Exists(InputPath))
            {
                Status = "Neplatná cesta ke hře.";
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = InputPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Status = $"Nepodařilo se otevřít složku: {ex.Message}";
        }
    }

    private void ClearLogs()
    {
        Status = "Vyčištění logů zatím není implementováno.";
    }


    private void ClearShaders()
    {
        try
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            var paths = new[]
            {
                Path.Combine(home, ".cache", "mesa_shader_cache"),
                Path.Combine(home, ".cache", "nvidia")
            };

            int totalDeleted = 0;
            int foldersProcessed = 0;

            foreach (var dir in paths)
            {
                if (!Directory.Exists(dir))
                {
                    Console.WriteLine($"[Shaders] Nenalezeno: {dir}");
                    continue;
                }

                foldersProcessed++;

                var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file);
                        totalDeleted++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Shaders] Chyba mazání {file}: {ex.Message}");
                    }
                }

                var subDirs = Directory.GetDirectories(dir, "*", SearchOption.AllDirectories)
                    .OrderByDescending(d => d.Length);

                foreach (var subDir in subDirs)
                {
                    try
                    {
                        if (!Directory.EnumerateFileSystemEntries(subDir).Any())
                        {
                            Directory.Delete(subDir);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Shaders] Chyba mazání složky {subDir}: {ex.Message}");
                    }
                }
            }

            Status = $"Shadery vyčištěny ({foldersProcessed} složky)";
            Console.WriteLine($"[Shaders] Smazáno souborů: {totalDeleted}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Status = "Chyba při mazání shaderů";
        }
    }


    private void OpenDownloadFolder()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(LastDownloadFolder) || !Directory.Exists(LastDownloadFolder))
            {
                Status = "Složka se staženým souborem neexistuje.";
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = LastDownloadFolder,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Status = $"Nepodařilo se otevřít složku: {ex.Message}";
        }
    }

    public async Task DownloadLatestVersionAsync()
    {
        try
        {
            Status = "Stahuji novou verzi...";

            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("ScCestinator");

            var json = await client.GetStringAsync(Constants.GitHubLatestReleaseApi);
            using var doc = System.Text.Json.JsonDocument.Parse(json);

            var assets = doc.RootElement.GetProperty("assets");
            foreach (var asset in assets.EnumerateArray())
            {
                var name = asset.GetProperty("name").GetString();
                if (name != null && name.EndsWith(".AppImage"))
                {
                    var url = asset.GetProperty("browser_download_url").GetString();

                    var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    var downloadsDir = GetDownloadFolder();
                    Directory.CreateDirectory(downloadsDir);

                    var downloadPath = Path.Combine(downloadsDir, name);

IsDownloading = true;
DownloadProgress = 0;

using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
response.EnsureSuccessStatusCode();

var total = response.Content.Headers.ContentLength ?? -1L;
var canReport = total != -1;

using var stream = await response.Content.ReadAsStreamAsync();
using var fs = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None);

var buffer = new byte[8192];
long totalRead = 0;
int read;

while ((read = await stream.ReadAsync(buffer)) > 0)
{
    await fs.WriteAsync(buffer.AsMemory(0, read));
    totalRead += read;

    if (canReport)
    {
        DownloadProgress = (double)totalRead / total * 100;
    }
}

IsDownloading = false;

                    LastDownloadFolder = downloadsDir;
                    Status = $"Staženo: {downloadPath}";
                    return;
                }
            }

            Status = "AppImage nebyl nalezen.";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Status = $"Chyba při stahování: {ex.Message}";
        }
    }

    
    private string GetDownloadFolder()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "xdg-user-dir",
                Arguments = "DOWNLOAD",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            using var process = Process.Start(psi);
            var result = process?.StandardOutput.ReadToEnd().Trim();

            if (!string.IsNullOrWhiteSpace(result) && Directory.Exists(result))
                return result;
        }
        catch
        {
        }

        // fallback
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var fallback = Path.Combine(home, "SC-Cestinator-downloads");
        Directory.CreateDirectory(fallback);
        return fallback;
    }

    private async Task CheckAppUpdateAsync()
    {
        try
        {
            var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var currentVersion = $"{appVersion?.Major}.{appVersion?.Minor}.{appVersion?.Build ?? 0}";

            var latestAppVersion = await _gitHubService.GetLatestAppVersionAsync();

            if (!string.IsNullOrWhiteSpace(latestAppVersion) && latestAppVersion != currentVersion)
            {
                AppUpdateStatus = $"⚠ Nová verze aplikace: {currentVersion} → {latestAppVersion}";
                IsAppUpdateAvailable = true;
            }
            else
            {
                AppUpdateStatus = "";
                IsAppUpdateAvailable = false;
            }
        }
        catch
        {
            // Nechceme rozbíjet start aplikace kvůli kontrole aktualizace
            AppUpdateStatus = "";
            IsAppUpdateAvailable = false;
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
            AppUpdateStatus = "";
            IsAppUpdateAvailable = false;
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
            var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var currentVersion = $"{appVersion?.Major}.{appVersion?.Minor}.{appVersion?.Build ?? 0}";           


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
            var latestAppVersion = await _gitHubService.GetLatestAppVersionAsync();

            if (!string.IsNullOrWhiteSpace(latestAppVersion) && latestAppVersion != currentVersion)
            {
                AppUpdateStatus = $"⚠ Nová verze aplikace: {currentVersion} → {latestAppVersion}";
                IsAppUpdateAvailable = true;
            }
            else
            {
                AppUpdateStatus = "";
                IsAppUpdateAvailable = false;
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

    public async Task UninstallAsync()
    {
        if (string.IsNullOrWhiteSpace(InputPath))
            return;

        var confirmed = await _confirmationDialogService.ConfirmUninstallAsync();
        if (!confirmed)
        {
            Status = "Odinstalace zrušena";
            return;
        }

        Status = "Kontroluji cestu ke Star Citizenu...";
        var result = _pathService.ValidateStarCitizenPath(InputPath);

        if (result.LivePath == null)
            return;

        IsBusy = true;

        try
        {
            await _localizationService.UninstallAsync(
                result.LivePath,
                progress => Status = progress
            );

            Status = "Odinstalace dokončena ✔";
            await ValidatePathAsync();
        }
        catch (FileNotFoundException ex)
        {
            Status = ex.Message;
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
            Status = $"Chyba při odinstalaci: {ex.Message}";
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

    private void UpdateStatusBrush()
    {
        if (string.IsNullOrWhiteSpace(Status))
        {
            StatusBrush = Brushes.Black;
            return;
        }

        var s = Status;

        if (s.Contains("✔") || s.Contains("aktuální", StringComparison.OrdinalIgnoreCase) ||
            s.Contains("dokončena", StringComparison.OrdinalIgnoreCase) ||
            s.Contains("hotovo", StringComparison.OrdinalIgnoreCase))
        {
            StatusBrush = Brushes.ForestGreen;
            return;
        }

        if (s.Contains("chyba", StringComparison.OrdinalIgnoreCase) ||
            s.Contains("neexistuje", StringComparison.OrdinalIgnoreCase) ||
            s.Contains("neplatná", StringComparison.OrdinalIgnoreCase) ||
            s.Contains("chybí", StringComparison.OrdinalIgnoreCase) ||
            s.Contains("není nainstalována", StringComparison.OrdinalIgnoreCase) ||
            s.Contains("nepodařilo", StringComparison.OrdinalIgnoreCase))
        {
            StatusBrush = Brushes.Firebrick;
            return;
        }

        StatusBrush = Brushes.Black;
    }
}
