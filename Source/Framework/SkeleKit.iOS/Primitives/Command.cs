using System.Windows.Input;

namespace SkeleKit;

/// <summary>
/// Creates commands from plain delegates, for handlers that live in the view rather than a ViewModel.
/// </summary>
public static class Command
{
	/// <summary>
	/// A command that runs <paramref name="action"/>, always executable.
	/// </summary>
	/// <param name="action">The handler to run.</param>
	/// <returns>The wrapping command.</returns>
	public static ICommand From(
		Action action) =>
		new DelegateCommand(_ => action());

	/// <summary>
	/// A command that runs <paramref name="action"/> with the command parameter, always executable.
	/// </summary>
	/// <typeparam name="T">The parameter type the handler expects.</typeparam>
	/// <param name="action">The handler to run.</param>
	/// <returns>The wrapping command.</returns>
	public static ICommand From<T>(
		Action<T?> action) =>
		new DelegateCommand(parameter => action(parameter is T value ? value : default));
}

internal sealed class DelegateCommand(
	Action<object?> execute) : ICommand
{
	public event EventHandler? CanExecuteChanged
	{
		add { }
		remove { }
	}


	public bool CanExecute(
		object? parameter) => true;

	public void Execute(
		object? parameter) => execute(parameter);
}
