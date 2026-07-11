using Foundation;
using Microsoft.Extensions.DependencyInjection;
using UIKit;

namespace BareUI;

/// <summary>
/// The app entry point: registers services, maps ViewModels to pages, builds the shell, and runs.
/// </summary>
public sealed class BareApp
{
	readonly ServiceCollection services = [];
	readonly ViewRegistry registry = new();

	TabsBuilder? tabs;
	Type? rootViewModel;

	IServiceProvider? provider;
	Navigator? navigator;

	BareApp()
	{ }

	internal static BareApp? Current { get; private set; }

	/// <summary>
	/// Starts configuring the app.
	/// </summary>
	public static BareApp Create() =>
		Current = new();

	/// <summary>
	/// Registers services. Use explicit factories to stay trim-safe.
	/// </summary>
	public BareApp UseServices(
		Action<IServiceCollection> configure)
	{
		configure(services);

		return this;
	}

	/// <summary>
	/// Maps a ViewModel to the page that renders it.
	/// </summary>
	public BareApp Map<TViewModel, TView>()
		where TViewModel : class
		where TView : ContentView<TViewModel>, new()
	{
		registry.Map<TViewModel, TView>();

		return this;
	}

	/// <summary>
	/// Builds a tab bar shell.
	/// </summary>
	public BareApp Tabs(
		Action<TabsBuilder> configure)
	{
		tabs = new();
		configure(tabs);

		return this;
	}

	/// <summary>
	/// Uses a single navigation stack rooted at <typeparamref name="TViewModel"/>.
	/// </summary>
	public BareApp Root<TViewModel>()
		where TViewModel : class
	{
		rootViewModel = typeof(TViewModel);

		return this;
	}

	/// <summary>
	/// Runs the app. Replaces Main/AppDelegate/scene wiring.
	/// </summary>
	public void Run(
		string[] args)
	{
		services.AddSingleton<INavigator>(_ => Navigator);

		UIApplication.Main(args, null, typeof(BareAppDelegate));
	}


	internal IServiceProvider Services =>
		provider ??= services.BuildServiceProvider();

	internal Navigator Navigator =>
		navigator ??= new(registry, Services, CurrentStack);

	// the shell is rooted by the window; navigator + hosts are rooted by this static app
	internal UIViewController BuildShell()
	{
		if (tabs is { } definition)
			return BuildTabs(definition);

		if (rootViewModel is null)
			throw new InvalidOperationException("Call Tabs(...) or Root<TViewModel>() before Run().");

		return BuildStack(rootViewModel);
	}

	UIViewController BuildTabs(
		TabsBuilder definition)
	{
		UITabBarController controller = new();

		List<UIViewController> stacks = [];
		foreach (TabDefinition tab in definition.Definitions)
		{
			UINavigationController stack = BuildStack(tab.ViewModel, definition.UseLargeTitles);

			// a nav controller takes its tab item from its root page, so the page Title would win
			stack.ViewControllers![0].TabBarItem = new(tab.Title, UIImage.GetSystemImage(tab.Icon), null);

			stacks.Add(stack);
		}

		controller.ViewControllers = [.. stacks];

		return controller;
	}

	UINavigationController BuildStack(
		Type viewModel,
		bool largeTitles = false)
	{
		object instance = Services.GetRequiredService(viewModel);

		UINavigationController stack = new(new PageHost(registry.CreatePage(instance)));
		stack.NavigationBar.PrefersLargeTitles = largeTitles;

		return stack;
	}

	static UINavigationController? CurrentStack()
	{
		UIViewController? root = UIApplication.SharedApplication
			.ConnectedScenes
			.OfType<UIWindowScene>()
			.SelectMany(scene => scene.Windows)
			.FirstOrDefault(window => window.IsKeyWindow)?
			.RootViewController;

		return root switch
		{
			UITabBarController tabs => tabs.SelectedViewController as UINavigationController,
			UINavigationController stack => stack,
			_ => null
		};
	}
}

[Register(nameof(BareAppDelegate))]
public class BareAppDelegate : UIApplicationDelegate
{
	public override UIWindow? Window { get; set; }
}

[Register(nameof(BareSceneDelegate))]
public class BareSceneDelegate : UIWindowSceneDelegate
{
	public override UIWindow? Window { get; set; }

	public override void WillConnect(
		UIScene scene,
		UISceneSession session,
		UISceneConnectionOptions connectionOptions)
	{
		if (scene is not UIWindowScene windowScene || BareApp.Current is not { } app)
			return;

		Window = new(windowScene)
		{
			RootViewController = app.BuildShell()
		};
		Window.MakeKeyAndVisible();
	}
}
