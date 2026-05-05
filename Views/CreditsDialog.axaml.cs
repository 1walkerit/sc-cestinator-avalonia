using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ScCestinator.Views;

public partial class CreditsDialog : Window
{
    public CreditsDialog()
    {
        InitializeComponent();
    }

    private void Close_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OpenCestinatorGithub_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/JarredSC/Star-Citizen-CZ-lokalizace/",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Credits] Nelze otevřít odkaz: {ex.Message}");
        }
    }
}
