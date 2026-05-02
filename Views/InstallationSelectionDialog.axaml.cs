using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ScCestinator.Views;

public partial class InstallationSelectionDialog : Window
{
    public string? SelectedInstallation { get; private set; }

    public InstallationSelectionDialog()
    {
        InitializeComponent();
    }

    public InstallationSelectionDialog(IEnumerable<string> installations)
        : this()
    {
        InstallationsListBox.ItemsSource = installations;
        InstallationsListBox.SelectedIndex = 0;
        this.Opened += (_, _) => InstallationsListBox.Focus();
        InstallationsListBox.DoubleTapped += (_, _) =>
{
    SelectedInstallation = InstallationsListBox.SelectedItem as string;
    Close(SelectedInstallation);
};
    }

    private void Ok_Click(object? sender, RoutedEventArgs e)
    {
        SelectedInstallation = InstallationsListBox.SelectedItem as string;
        Close(SelectedInstallation);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}
