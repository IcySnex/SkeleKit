namespace SkeleKit;

/// <summary>
/// Manages application navigation, modal presentations, and native dialogs from a view model.
/// </summary>
public interface INavigator
{
	/// <summary>
	/// Selects the tab with the given title, as declared on <c>Tab(title, ...)</c>.
	/// </summary>
	/// <param name="title">The tab's title.</param>
	/// <returns>A task representing the async operation.</returns>
	Task SelectTabAsync(
		string title);

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
	/// Pushes a registered view, resolving any associated ViewModel from the service container.
	/// </summary>
	/// <typeparam name="TView">The registered view type.</typeparam>
	/// <returns>A task representing the async operation.</returns>
	Task PushViewAsync<TView>() where TView : ContentView;

	/// <summary>
	/// Pushes a registered view by type, resolving any associated ViewModel from the service container.
	/// </summary>
	/// <param name="view">The registered view type.</param>
	/// <returns>A task representing the async operation.</returns>
	Task PushViewAsync(
		Type view);

	/// <summary>
	/// Pushes an existing page instance directly.
	/// </summary>
	/// <remarks>
	/// Create a new instance per navigation.
	/// </remarks>
	/// <param name="page">The page to push.</param>
	/// <returns>A task representing the async operation.</returns>
	Task PushViewAsync(
		ContentView page);


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
	/// Presents a registered view, resolving any associated ViewModel from the service container.
	/// </summary>
	/// <typeparam name="TView">The registered view type.</typeparam>
	/// <param name="style">The modal style and presentation configuration.</param>
	/// <returns>A task representing the async operation.</returns>
	Task PresentViewAsync<TView>(
		ModalStyle style) where TView : ContentView;

	/// <summary>
	/// Presents a registered view by type, resolving any associated ViewModel from the service container.
	/// </summary>
	/// <param name="view">The registered view type.</param>
	/// <param name="style">The modal style and presentation configuration.</param>
	/// <returns>A task representing the async operation.</returns>
	Task PresentViewAsync(
		Type view,
		ModalStyle style);

	/// <summary>
	/// Presents an existing page instance directly.
	/// </summary>
	/// <remarks>
	/// Create a new instance per navigation.
	/// </remarks>
	/// <param name="page">The page to present.</param>
	/// <param name="style">The modal style and presentation configuration.</param>
	/// <returns>A task representing the async operation.</returns>
	Task PresentViewAsync(
		ContentView page,
		ModalStyle style);


	/// <summary>
	/// Dismisses the top-most active modal presentation layer.
	/// </summary>
	/// <returns>A task representing the async operation.</returns>
	Task DismissAsync();


	/// <summary>
	/// Opens a web address in an in-app Safari browser, with the system reader, share and done chrome.
	/// </summary>
	/// <param name="url">The <c>http</c> or <c>https</c> address to open.</param>
	/// <returns>A task that completes once the browser is presented.</returns>
	Task OpenUrlAsync(
		string url);

	/// <summary>
	/// Opens a web address in an in-app Safari browser with custom presentation and browser options.
	/// </summary>
	/// <param name="url">The <c>http</c> or <c>https</c> address to open.</param>
	/// <param name="style">The modal style and sheet detents used to present the browser.</param>
	/// <param name="entersReaderIfAvailable">Whether Safari should enter Reader mode when it is available.</param>
	/// <param name="barCollapsingEnabled">Whether Safari's bars may collapse while browsing.</param>
	/// <param name="dismissButtonStyle">The style of the browser's dismiss button.</param>
	/// <returns>A task that completes once the browser is presented.</returns>
	Task OpenUrlAsync(
		string url,
		ModalStyle style,
		bool entersReaderIfAvailable = false,
		bool barCollapsingEnabled = true,
		SafariDismissButtonStyle dismissButtonStyle = SafariDismissButtonStyle.Close);


	/// <summary>
	/// Displays an alert dialog with a single button to dismiss it.
	/// </summary>
	/// <param name="title">The title text of the alert.</param>
	/// <param name="message">The message body.</param>
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
	/// <param name="message">The message body.</param>
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
	/// <param name="message">The message body.</param>
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
	/// <param name="title">The action sheet's title.</param>
	/// <param name="cancel">The cancel button's text.</param>
	/// <param name="options">The options to choose from.</param>
	/// <returns>A task containing the chosen string option, or null if the action sheet was canceled.</returns>
	Task<string?> SelectAsync(
		string title,
		string cancel = "Cancel",
		params string[] options);
}
