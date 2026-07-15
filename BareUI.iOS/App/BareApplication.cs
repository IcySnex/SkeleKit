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


	readonly ViewRegistry registry;
	readonly ShellKind shell;
	readonly bool preferLargeTitles;
	readonly TabsBuilder? tabsBuilder;
	readonly Type? rootViewModel;

	/// <summary>
	/// The built-in service provider for resolving dependencies.
	/// </summary>
	public IServiceProvider Services { get; }

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
				controller.ViewControllers = tabsBuilder?.Definitions
					.Select(UIViewController (definition) =>
					{
						UINavigationController stack = Stack(definition.ViewModel, tabsBuilder.UseLargeTitles);

						PageHost root = (PageHost)stack.ViewControllers![0];
						root.TabBarItem = new(definition.Title, UIImage.GetSystemImage(definition.Icon), null);

						// the fresh item wiped any badge the page set during construction
						root.Page?.ApplyTabBadge();

						return stack;
					})
					.ToArray();

				if (tabsBuilder?.UseSidebar is true && OperatingSystem.IsIOSVersionAtLeast(18))
					controller.Mode = UITabBarControllerMode.TabSidebar;

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
