using Foundation;
using Microsoft.Extensions.DependencyInjection;
using UIKit;

namespace BareUI;

enum ShellKind
{
	None,
	SinglePage,
	Stack,
	Tabs
}

/// <summary>
/// The app entry point: registers services, maps ViewModels to pages, builds the shell, and runs.
/// </summary>
public sealed class BareApp
{
	readonly ServiceCollection services = [];
	readonly ViewRegistry registry = new();

	ShellKind shell = ShellKind.None;
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
	/// Registers the pages the app can show. Every navigable page goes here, once.
	/// </summary>
	public BareApp UsePages(
		Action<PagesBuilder> configure)
	{
		configure(new(registry));

		return this;
	}

	/// <summary>
	/// A single page with no navigation bar.
	/// </summary>
	public BareApp SinglePage<TView>()
		where TView : ContentView =>
		Root<TView>(ShellKind.SinglePage);

	/// <summary>
	/// One navigation stack rooted at <typeparamref name="TView"/>.
	/// </summary>
	public BareApp Stack<TView>()
		where TView : ContentView =>
		Root<TView>(ShellKind.Stack);

	/// <summary>
	/// A tab bar; each tab gets its own navigation stack.
	/// </summary>
	public BareApp Tabs(
		Action<TabsBuilder> configure)
	{
		tabs = new(registry);
		configure(tabs);

		shell = ShellKind.Tabs;

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

	BareApp Root<TView>(
		ShellKind kind)
		where TView : ContentView
	{
		rootViewModel = registry.ViewModelOf<TView>();
		shell = kind;

		return this;
	}

	// the shell is rooted by the window; navigator + hosts are rooted by this static app
	internal UIViewController BuildShell() =>
		shell switch
		{
			ShellKind.Tabs => BuildTabs(tabs!),
			ShellKind.Stack => BuildStack(RootPage(), largeTitles: false),
			ShellKind.SinglePage => new PageHost(RootPage()),
			_ => throw new InvalidOperationException("Call Tabs(...), Stack<TView>() or SinglePage<TView>() before Run().")
		};

	ContentView RootPage() =>
		registry.CreatePage(registry.CreateViewModel(rootViewModel!, Services));

	UIViewController BuildTabs(
		TabsBuilder definition)
	{
		UITabBarController controller = new();

		List<UIViewController> stacks = [];
		foreach (TabDefinition tab in definition.Definitions)
		{
			ContentView page = registry.CreatePage(registry.CreateViewModel(tab.ViewModel, Services));
			UINavigationController stack = BuildStack(page, definition.UseLargeTitles);

			// a nav controller takes its tab item from its root page, so the page Title would win
			stack.ViewControllers![0].TabBarItem = new(tab.Title, UIImage.GetSystemImage(tab.Icon), null);

			stacks.Add(stack);
		}

		controller.ViewControllers = [.. stacks];

		return controller;
	}

	static UINavigationController BuildStack(
		ContentView page,
		bool largeTitles)
	{
		UINavigationController stack = new(new PageHost(page));
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
