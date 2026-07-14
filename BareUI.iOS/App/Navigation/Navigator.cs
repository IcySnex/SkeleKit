namespace BareUI;

internal sealed class Navigator(
	ViewRegistry registry,
	IServiceProvider services,
	Func<UINavigationController?> currentStack) : INavigator
{
	static UIViewController? Top()
	{
		UIViewController? controller = UIApplication.SharedApplication
			.ConnectedScenes
			.OfType<UIWindowScene>()
			.SelectMany(scene => scene.Windows)
			.FirstOrDefault(window => window.IsKeyWindow)?
			.RootViewController;

		while (controller?.PresentedViewController is UIViewController presented)
			controller = presented;

		return controller;
	}

	static void Present<T>(
		UIAlertController alert,
		TaskCompletionSource<T> completion)
	{
		if (Top() is UIViewController top)
			top.PresentViewController(alert, true, null);
		else
			completion.SetResult(default!);
	}

	static void Present(
		UIAlertController alert,
		TaskCompletionSource completion)
	{
		if (Top() is UIViewController top)
			top.PresentViewController(alert, true, null);
		else
			completion.SetResult();
	}


	readonly List<PageHost> hosts = [];


	PageHost Track(
		ContentView page)
	{
		PageHost host = new(page);
		hosts.Add(host);

		return host;
	}

	void Prune()
	{
		UIViewController[] stack = currentStack()?.ViewControllers ?? [];

		hosts.RemoveAll(host => !stack.Contains(host) && host.PresentingViewController is null);
	}


	public Task PushAsync<TViewModel>() where TViewModel : class =>
		PushAsync(registry.CreateViewModel(typeof(TViewModel), services));

	public Task PushAsync(
		Type viewModel) =>
		PushAsync(registry.CreateViewModel(viewModel, services));

	public Task PushAsync(
		object viewModel)
	{
		if (currentStack() is not UINavigationController stack)
			throw new InvalidOperationException("There is no navigation stack to push onto.");

		PageHost host = Track(registry.CreatePage(viewModel));
		stack.PushViewController(host, true);

		return Task.CompletedTask;
	}

	public Task PopAsync()
	{
		currentStack()?.PopViewController(true);
		Prune();

		return Task.CompletedTask;
	}

	public Task PopToRootAsync()
	{
		currentStack()?.PopToRootViewController(true);
		Prune();

		return Task.CompletedTask;
	}


	public Task PresentAsync<TViewModel>(
		ModalStyle style) where TViewModel : class =>
		PresentAsync(registry.CreateViewModel(typeof(TViewModel), services), style);

	public Task PresentAsync(
		Type viewModel,
		ModalStyle style) =>
		PresentAsync(registry.CreateViewModel(viewModel, services), style);

	public Task PresentAsync(
		object viewModel,
		ModalStyle style)
	{
		if (Top() is not { } presenter)
			return Task.CompletedTask;

		PageHost host = Track(registry.CreatePage(viewModel));
		UINavigationController wrapper = new(host);

		wrapper.ModalPresentationStyle = style.Presentation switch
		{
			ModalPresentation.FullScreen => UIModalPresentationStyle.FullScreen,
			ModalPresentation.FormSheet => UIModalPresentationStyle.FormSheet,
			ModalPresentation.CurrentContext => UIModalPresentationStyle.CurrentContext,
			ModalPresentation.OverFullScreen => UIModalPresentationStyle.OverFullScreen,
			ModalPresentation.OverCurrentContext => UIModalPresentationStyle.OverCurrentContext,
			ModalPresentation.Popover => UIModalPresentationStyle.Popover,
			ModalPresentation.PageSheet => UIModalPresentationStyle.PageSheet,
			_ => UIModalPresentationStyle.Automatic
		};

		if (style.Presentation is ModalPresentation.PageSheet && wrapper.SheetPresentationController is UISheetPresentationController sheet)
			sheet.Detents = style.Detent is Detent.Medium
				? [UISheetPresentationControllerDetent.CreateMediumDetent()]
				: [UISheetPresentationControllerDetent.CreateLargeDetent()];

		presenter.PresentViewController(wrapper, true, null);

		return Task.CompletedTask;
	}

	public Task DismissAsync()
	{
		TaskCompletionSource completion = new();

		if (Top() is UIViewController top)
			top.DismissViewController(true, () =>
			{
				Prune();
				completion.SetResult();
			});
		else
			completion.SetResult();

		return completion.Task;
	}


	public Task AlertAsync(
		string title,
		string message,
		string dismiss = "OK")
	{
		TaskCompletionSource completion = new();

		UIAlertController alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
		alert.AddAction(UIAlertAction.Create(dismiss, UIAlertActionStyle.Default, _ => completion.SetResult()));

		Present(alert, completion);

		return completion.Task;
	}

	public Task<bool> ConfirmAsync(
		string title,
		string message,
		string accept = "OK",
		string cancel = "Cancel",
		bool destructive = false)
	{
		TaskCompletionSource<bool> completion = new();

		UIAlertController alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
		alert.AddAction(UIAlertAction.Create(cancel, UIAlertActionStyle.Cancel, _ => completion.SetResult(false)));
		alert.AddAction(UIAlertAction.Create(accept, destructive ? UIAlertActionStyle.Destructive : UIAlertActionStyle.Default, _ => completion.SetResult(true)));

		Present(alert, completion);

		return completion.Task;
	}

	public Task<string?> PromptAsync(
		string title,
		string message,
		string placeholder = "",
		string text = "",
		string accept = "OK",
		string cancel = "Cancel")
	{
		TaskCompletionSource<string?> completion = new();

		UIAlertController alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

		alert.AddTextField(field =>
		{
			field.Placeholder = placeholder;
			field.Text = text;
		});

		alert.AddAction(UIAlertAction.Create(cancel, UIAlertActionStyle.Cancel, _ => completion.SetResult(null)));
		alert.AddAction(UIAlertAction.Create(accept, UIAlertActionStyle.Default, _ =>
			completion.SetResult(alert.TextFields?.FirstOrDefault()?.Text ?? "")));

		Present(alert, completion);

		return completion.Task;
	}

	public Task<string?> ActionSheetAsync(
		string title,
		string cancel = "Cancel",
		params string[] options)
	{
		TaskCompletionSource<string?> completion = new();

		UIAlertController sheet = UIAlertController.Create(title, null, UIAlertControllerStyle.ActionSheet);

		foreach (string option in options)
			sheet.AddAction(UIAlertAction.Create(option, UIAlertActionStyle.Default, _ => completion.SetResult(option)));

		sheet.AddAction(UIAlertAction.Create(cancel, UIAlertActionStyle.Cancel, _ => completion.SetResult(null)));

		Present(sheet, completion);

		return completion.Task;
	}
}
