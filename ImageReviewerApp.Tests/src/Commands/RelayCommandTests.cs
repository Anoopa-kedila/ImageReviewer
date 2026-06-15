using ImageReviewerApp.Commands;

namespace ImageReviewerApp.Tests.Commands;

public class RelayCommandTests
{
    [Fact]
    public void Execute_CallsDelegate()
    {
        bool executed = false;
        var command = new RelayCommand(() => executed = true);

        command.Execute(null);

        Assert.True(executed);
    }

    [Fact]
    public void CanExecute_NullDelegate_ReturnsTrue()
    {
        var command = new RelayCommand(() => { });

        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void CanExecute_WithDelegate_ReturnsCorrectValue()
    {
        bool canExecuteValue = false;
        var command = new RelayCommand(() => { }, () => canExecuteValue);

        Assert.False(command.CanExecute(null));

        canExecuteValue = true;

        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void Execute_CanExecuteFalse_StillExecutesCommand()
    {
        bool executed = false;
        var command = new RelayCommand(
            () => executed = true,
            () => false);

        command.Execute(null);

        Assert.True(executed);
    }

    [Fact]
    public void RaiseCanExecuteChanged_RaisesEvent()
    {
        bool eventRaised = false;
        var command = new RelayCommand(() => { });

        command.CanExecuteChanged += (s, e) => eventRaised = true;

        command.RaiseCanExecuteChanged();

        Assert.True(eventRaised);
    }

    [Fact]
    public void Constructor_NullExecute_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RelayCommand(null!));
    }
}
