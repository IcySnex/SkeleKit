using Microsoft.Extensions.DependencyInjection;

namespace BareUI;

/// <summary>
/// The core application instance that handles DI, navigation setup, and the app lifecycle.
/// </summary>
public class BareApplication
{
	internal enum ShellKind
	{
		None,
		SinglePage,
		Stack,
		Tabs
	}


	/// <summary>
	/// The currently running application instance.
	/// </summary>
	public static BareApplication? Current { get; private set; }


	static UIViewController? Root() =>
		UIApplication.SharedApplication
			.ConnectedScenes
			.OfType<UIWindowScene>()
			.SelectMany(scene => scene.Windows)
			.FirstOrDefault(window => window.IsKeyWindow)?
			.RootViewController;

	static UINavigationController? CurrentStack() =>
		Root() switch
		{
			UITabBarController tabs => tabs.SelectedViewController as UINavigationController,
			UINavigationController stack => stack,
			_ => null
		};

	static UITabBarController? CurrentTabs() =>
		Root() as UITabBarController;


	readonly ViewRegistry registry;
	readonly ShellKind shell;
	readonly bool preferLargeTitles;
	readonly TabsBuilder? tabsBuilder;
	readonly Type? rootViewModel;

	/// <summary>
	/// The built-in service provider for resolving dependencies.
	/// </summary>
	public IServiceProvider Services { get; }

	// roots the accessory: UIKit's retain alone would let the peers die
	internal UITabAccessory? Accessory { get; private set; }
	View? accessoryContent;
	AccessoryHost? accessoryHost;

	// intent lives on the view itself; a page hiding the tab bar overrides it
	internal bool AccessoryWanted =>
		accessoryContent?.IsVisible.Value is true;

	void SyncAccessory()
	{
		if (Accessory is null
			|| !OperatingSystem.IsIOSVersionAtLeast(26)
			|| CurrentTabs() is not { } tabs)
			return;

		bool barHidden = (CurrentStack()?.TopViewController as PageHost)?.HidesBottomBarWhenPushed is true;

		tabs.SetBottomAccessory(AccessoryWanted && !barHidden ? Accessory : null, animated: true);
	}

	internal Action? Backgrounded { get; set; }
	internal Action? Foregrounded { get; set; }

	internal void NotifyBackground() =>
		Backgrounded?.Invoke();

	internal void NotifyForeground() =>
		Foregrounded?.Invoke();

	internal BareApplication(
		BareApplicationBuilder builder)
	{
		registry = builder.Registry;
		shell = builder.Shell;
		preferLargeTitles = builder.PreferLargeTitles;
		tabsBuilder = builder.TabsBuilder;
		rootViewModel = builder.RootViewModel;

		Backgrounded = builder.LifecycleBackground;
		Foregrounded = builder.LifecycleForeground;

		builder.Services.AddSingleton<INavigator>(provider => new Navigator(registry, provider, CurrentStack));
		Services = builder.Services.BuildServiceProvider();
	}


	internal UIViewController BuildShell()
	{
		PageHost Page(Type? viewModel) =>
			new(registry.CreatePage(registry.CreateViewModel(viewModel!, Services)));

		UINavigationController Stack(Type? viewModel, bool prefersLargeTitles = false)
		{
			UINavigationController stack = new BareStack(Page(viewModel));
			stack.NavigationBar.PrefersLargeTitles = prefersLargeTitles;

			return stack;
		}

		switch (shell)
		{
			case ShellKind.SinglePage:
				return Page(rootViewModel);

			case ShellKind.Stack:
				return Stack(rootViewModel, preferLargeTitles);

			case ShellKind.Tabs:
				UITabBarController controller = new();

				// stacks build eagerly so a badge set during page construction lands on a never-opened tab
				List<UITab> tabs = [];

				foreach (TabsBuilder.Definition definition in tabsBuilder?.Definitions ?? [])
				{
					UINavigationController stack = Stack(definition.ViewModel, tabsBuilder!.UseLargeTitles);
					PageHost root = (PageHost)stack.ViewControllers![0];

					UITab tab = new(
						definition.Title,
						UIImage.GetSystemImage(definition.Icon),
						definition.ViewModel.Name,
						_ => stack);

					root.Tab = tab;
					root.Page?.ApplyTabBadge();

					tabs.Add(tab);
				}

				if (tabsBuilder?.SearchViewModel is { } searchViewModel)
				{
					UINavigationController stack = Stack(searchViewModel, tabsBuilder.UseLargeTitles);

					UISearchTab search = new(_ => stack);
					((PageHost)stack.ViewControllers![0]).Tab = search;

					tabs.Add(search);
				}

				controller.SetTabs([.. tabs], false);

				if (tabsBuilder?.UseSidebar is true)
					controller.Mode = UITabBarControllerMode.TabSidebar;

				if (tabsBuilder?.Minimize is { } minimize and not TabBarMinimize.Never && OperatingSystem.IsIOSVersionAtLeast(26))
					controller.TabBarMinimizeBehavior = minimize is TabBarMinimize.OnScrollUp
						? UITabBarMinimizeBehavior.OnScrollUp
						: UITabBarMinimizeBehavior.OnScrollDown;

				if (tabsBuilder?.AccessoryFactory is { } accessory && OperatingSystem.IsIOSVersionAtLeast(26))
				{
					accessoryContent = accessory();
					accessoryContent.VisibilityChanged = SyncAccessory;
					accessoryHost = new(accessoryContent);
					Accessory = new(accessoryHost);

					if (AccessoryWanted)
						controller.BottomAccessory = Accessory;
				}

				return controller;

			case ShellKind.None:
			default:
				throw new InvalidOperationException("Call Tabs(...), Stack<TView>() or SinglePage<TView>() before Run().");
		}
	}


	/// <summary>
	/// Creates a new builder to configure services and the layout shell.
	/// </summary>
	public static BareApplicationBuilder CreateBuilder() =>
		new();

	/// <summary>
	/// Starts the native iOS main loop.
	/// </summary>
	public void Run(
		string[] args)
	{
		Current = this;
		UIApplication.Main(args, null, typeof(BareApplicationDelegate));
	}
}

// a plain stack decides the status bar itself; this one asks the visible page
internal sealed class BareStack : UINavigationController
{
	public BareStack(
		UIViewController root) : base(root)
	{ }

	public BareStack(
		ObjCRuntime.NativeHandle handle) : base(handle)
	{ }


	public override UIViewController? ChildViewControllerForStatusBarStyle() =>
		TopViewController;
}
