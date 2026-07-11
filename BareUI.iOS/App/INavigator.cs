namespace BareUI;

/// <summary>
/// Navigation and dialogs, driven from a ViewModel. Inject it; never touch UIKit.
/// </summary>
public interface INavigator
{
	/// <summary>
	/// Pushes the page mapped to <typeparamref name="TViewModel"/>, resolving the ViewModel from services.
	/// </summary>
	Task PushAsync<TViewModel>()
		where TViewModel : class;

	/// <summary>
	/// Pushes the page mapped to this ViewModel instance.
	/// </summary>
	Task PushAsync(
		object viewModel);

	/// <summary>
	/// Pushes a page directly. For pages with no ViewModel.
	/// </summary>
	Task PushAsync(
		ContentView page);

	/// <summary>
	/// Pops the top page.
	/// </summary>
	Task PopAsync();

	/// <summary>
	/// Pops back to the first page in the stack.
	/// </summary>
	Task PopToRootAsync();

	/// <summary>
	/// Presents the page mapped to <typeparamref name="TViewModel"/> modally.
	/// </summary>
	Task PresentAsync<TViewModel>(
		ModalStyle style)
		where TViewModel : class;

	/// <summary>
	/// Presents the page mapped to this ViewModel instance modally.
	/// </summary>
	Task PresentAsync(
		object viewModel,
		ModalStyle style);

	/// <summary>
	/// Dismisses the current modal.
	/// </summary>
	Task DismissAsync();

	/// <summary>
	/// Shows a message with a single dismiss button.
	/// </summary>
	Task AlertAsync(
		string title,
		string message,
		string dismiss = "OK");

	/// <summary>
	/// Asks a yes/no question; true when the user accepts.
	/// </summary>
	Task<bool> ConfirmAsync(
		string title,
		string message,
		string accept = "OK",
		string cancel = "Cancel");

	/// <summary>
	/// Shows an action sheet; returns the chosen option, or null when cancelled.
	/// </summary>
	Task<string?> ActionSheetAsync(
		string title,
		string cancel,
		params string[] options);
}
