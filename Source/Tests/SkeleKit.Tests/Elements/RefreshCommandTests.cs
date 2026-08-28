using System.Windows.Input;
using Xunit;

namespace SkeleKit.Tests.Elements;

public class RefreshCommandTests
{
	sealed class TestCommand(
		bool canExecute) : ICommand
	{
		public event EventHandler? CanExecuteChanged
		{
			add { }
			remove { }
		}

		public int Executions { get; private set; }
		public object? CanExecuteParameter { get; private set; }
		public object? ExecuteParameter { get; private set; }

		public bool CanExecute(
			object? parameter)
		{
			CanExecuteParameter = parameter;
			return canExecute;
		}

		public void Execute(
			object? parameter)
		{
			ExecuteParameter = parameter;
			Executions++;
		}
	}


	[Fact]
	public void ScrollView_RejectedRefreshDoesNotStart()
	{
		TestCommand command = new(false);
		ScrollView view = new() { RefreshCommand = command };

		view.OnRefreshTriggered();

		Assert.False(view.IsRefreshing.Value);
		Assert.Equal(0, command.Executions);
	}

	[Fact]
	public void ScrollView_AllowedRefreshStartsAndExecutes()
	{
		TestCommand command = new(true);
		object parameter = new();
		ScrollView view = new()
		{
			RefreshCommand = command,
			RefreshCommandParameter = parameter
		};

		view.OnRefreshTriggered();

		Assert.True(view.IsRefreshing.Value);
		Assert.Equal(1, command.Executions);
		Assert.Same(parameter, command.CanExecuteParameter);
		Assert.Same(parameter, command.ExecuteParameter);
	}

}
