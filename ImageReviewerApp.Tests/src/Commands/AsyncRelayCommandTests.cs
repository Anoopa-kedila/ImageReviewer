using ImageReviewerApp.Commands;

namespace ImageReviewerApp.Tests.Commands;

public class AsyncRelayCommandTests
{
    [Fact]
    public async Task Execute_CallsDelegate()
    {
        bool executed = false;
        var command = new AsyncRelayCommand(async () =>
        {
            await Task.Delay(10);
            executed = true;
        });

        command.Execute(null);
        await Task.Delay(50);

        Assert.True(executed);
    }

    [Fact]
    public void Execute_WhileExecuting_CanExecuteReturnsFalse()
    {
        var tcs = new TaskCompletionSource<bool>();
        var command = new AsyncRelayCommand(async () => await tcs.Task);

        Assert.True(command.CanExecute(null));

        command.Execute(null);

        Assert.False(command.CanExecute(null));

        tcs.SetResult(true);
    }

    [Fact]
    public async Task Execute_AfterCompletion_CanExecuteReturnsTrue()
    {
        var command = new AsyncRelayCommand(async () => await Task.Delay(10));

        command.Execute(null);
        await Task.Delay(50);

        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void Execute_WithCanExecuteDelegate_RespectsCondition()
    {
        bool canExecuteValue = false;
        var command = new AsyncRelayCommand(
            async () => await Task.CompletedTask,
            () => canExecuteValue);

        Assert.False(command.CanExecute(null));

        canExecuteValue = true;

        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void RaiseCanExecuteChanged_RaisesEvent()
    {
        bool eventRaised = false;
        var command = new AsyncRelayCommand(async () => await Task.CompletedTask);

        command.CanExecuteChanged += (s, e) => eventRaised = true;

        command.RaiseCanExecuteChanged();

        Assert.True(eventRaised);
    }

    [Fact]
    public void Constructor_NullExecute_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new AsyncRelayCommand(null!));
    }

    [Fact]
    public async Task Execute_OperationCancelled_DoesNotPropagate()
    {
        var command = new AsyncRelayCommand(async () =>
        {
            await Task.Delay(10);
            throw new OperationCanceledException();
        });

        command.Execute(null);
        await Task.Delay(50);
    }

    [Fact]
    public void CanExecute_NullCanExecuteDelegate_ReturnsTrue()
    {
        var command = new AsyncRelayCommand(async () => await Task.CompletedTask);

        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public async Task Execute_MultipleCommands_OnlyOneExecutesAtTime()
    {
        int executionCount = 0;
        var tcs = new TaskCompletionSource<bool>();
        var command = new AsyncRelayCommand(async () =>
        {
            executionCount++;
            await tcs.Task;
        });

        command.Execute(null);
        command.Execute(null);
        command.Execute(null);

        Assert.Equal(1, executionCount);

        tcs.SetResult(true);
    }

    [Fact]
    public async Task Execute_CompletesSuccessfully()
    {
        bool executed = false;
        var command = new AsyncRelayCommand(async () =>
        {
            await Task.Delay(10);
            executed = true;
        });

        command.Execute(null);
        await Task.Delay(50);

        Assert.True(executed);
    }
}
