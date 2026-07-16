namespace BareUI;

/// <summary>
/// Manages application navigation, modal presentations, and native dialogs from a view model.
/// </summary>
public interface INavigator
{
	/// <summary>
	/// Whether the tab accessory registered with <c>Tabs.Accessory</c> is shown. Changes animate. False when none is registered.
	/// </summary>
	bool AccessoryVisible { get; set; }

	/// <summary>
	/// Pushes a new page onto the stack, resolving its view model from the service container.
	/// </summary>
	/// <typeparam name="TViewModel">The type of the view model to resolve.</typeparam>
	/// <returns>A task representing the async operation.</returns>
	Task PushAsync<TViewModel>() where TViewModel : class;

	/// <summary>
	/// Pushes a new page onto the stack, resolving its view model by type from the service container.
	/// </summary>
	/// <param name="viewModel">The type of the view model to resolve.</param>
	/// <returns>A task representing the async operation.</returns>
	Task PushAsync(
		Type viewModel);

	/// <summary>
	/// Pushes a new page onto the stack using an existing view model instance.
	/// </summary>
	/// <param name="viewModel">The view model instance to bind to the page.</param>
	/// <returns>A task representing the async operation.</returns>
	Task PushAsync(
		object viewModel);


	/// <summary>
	/// Pops the top page off the current navigation stack.
	/// </summary>
	/// <returns>A task representing the async operation.</returns>
	Task PopAsync();

	/// <summary>
	/// Pops all pages off the stack except for the root page.
	/// </summary>
	/// <returns>A task representing the async operation.</returns>
	Task PopToRootAsync();


	/// <summary>
	/// Presents a modal page, resolving its view model from the service container.
	/// </summary>
	/// <typeparam name="TViewModel">The type of the view model to resolve.</typeparam>
	/// <param name="style">The modal style and presentation configuration.</param>
	/// <returns>A task representing the async operation.</returns>
	Task PresentAsync<TViewModel>(
		ModalStyle style) where TViewModel : class;

	/// <summary>
	/// Presents a modal page, resolving its view model by type from the service container.
	/// </summary>
	/// <param name="viewModel">The type of the view model to resolve.</param>
	/// <param name="style">The modal style and presentation configuration.</param>
	/// <returns>A task representing the async operation.</returns>
	Task PresentAsync(
		Type viewModel,
		ModalStyle style);

	/// <summary>
	/// Presents a modal page using an existing view model instance.
	/// </summary>
	/// <param name="viewModel">The view model instance to bind to the page.</param>
	/// <param name="style">The modal style and presentation configuration.</param>
	/// <returns>A task representing the async operation.</returns>
	Task PresentAsync(
		object viewModel,
		ModalStyle style);


	/// <summary>
	/// Dismisses the top-most active modal presentation layer.
	/// </summary>
	/// <returns>A task representing the async operation.</returns>
	Task DismissAsync();


	/// <summary>
	/// Displays an alert dialog with a single button to dismiss it.
	/// </summary>
	/// <param name="title">The title text of the alert.</param>
	/// <param name="message">The main message content body.</param>
	/// <param name="dismiss">The text for the dismiss button.</param>
	/// <returns>A task representing the async operation.</returns>
	Task AlertAsync(
		string title,
		string message,
		string dismiss = "OK");

	/// <summary>
	/// Displays a confirmation dialog with accept and cancel actions.
	/// </summary>
	/// <param name="title">The title text of the confirmation box.</param>
	/// <param name="message">The main message content body.</param>
	/// <param name="accept">The text for the confirming button.</param>
	/// <param name="cancel">The text for the canceling button.</param>
	/// <param name="destructive">Whether the confirming button is styled red, for actions that discard something.</param>
	/// <returns>A task containing true if accepted, or false if canceled.</returns>
	Task<bool> ConfirmAsync(
		string title,
		string message,
		string accept = "OK",
		string cancel = "Cancel",
		bool destructive = false);

	/// <summary>
	/// Displays an alert with a single text field, for a name or another short answer.
	/// </summary>
	/// <param name="title">The title text of the alert.</param>
	/// <param name="message">The main message content body.</param>
	/// <param name="placeholder">The text field's placeholder.</param>
	/// <param name="text">The text the field starts with.</param>
	/// <param name="accept">The text for the confirming button.</param>
	/// <param name="cancel">The text for the canceling button.</param>
	/// <returns>A task containing what was typed, or null if the alert was canceled.</returns>
	Task<string?> PromptAsync(
		string title,
		string message,
		string placeholder = "",
		string text = "",
		string accept = "OK",
		string cancel = "Cancel");

	/// <summary>
	/// Displays an action sheet layout with multiple choices.
	/// </summary>
	/// <param name="title">The title text of the confirmation box.</param>
	/// <param name="cancel">The text for the cancel button context.</param>
	/// <param name="options">The array list of selectable string option values.</param>
	/// <returns>A task containing the chosen string option, or null if the action sheet was canceled.</returns>
	Task<string?> ActionSheetAsync(
		string title,
		string cancel = "Cancel",
		params string[] options);
}
