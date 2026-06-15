using ImageReviewerApp.Contracts;
using Microsoft.Win32;
using System.Windows;

namespace ImageReviewerApp.Services;

public class DialogService : IDialogService
{
    public string? ShowOpenFileDialog(string filter, string title)
    {
        var dialog = new OpenFileDialog
        {
            Filter = filter,
            Title = title
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? ShowSaveFileDialog(string filter, string title, string defaultExt)
    {
        var dialog = new SaveFileDialog
        {
            Filter = filter,
            Title = title,
            DefaultExt = defaultExt
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public void ShowError(string message, string title = "Error")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void ShowInformation(string message, string title = "Information")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowWarning(string message, string title = "Warning")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
