namespace ImageReviewerApp.Contracts;

public interface IDialogService
{
    string? ShowOpenFileDialog(string filter, string title);
    string? ShowSaveFileDialog(string filter, string title, string defaultExt);
    void ShowError(string message, string title = "Error");
    void ShowInformation(string message, string title = "Information");
    void ShowWarning(string message, string title = "Warning");
}
