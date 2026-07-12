using Microsoft.Extensions.DependencyInjection;

namespace BareUI;

public class BareApplication
{
	internal enum ShellKind
	{
		None,
		SinglePage,
		Stack,
		Tabs
	}


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
	readonly bool PreferLargeTitles;
	readonly TabsBuilder? tabsBuilder;
	readonly Type? rootViewModel;

	public IServiceProvider Services { get; }

	internal BareApplication(
		BareApplicationBuilder builder)
	{
		registry = builder.Registry;
		shell = builder.Shell;
		PreferLargeTitles = builder.PreferLargeTitles;
		tabsBuilder = builder.TabsBuilder;
		rootViewModel = builder.RootViewModel;

		builder.Services.AddSingleton<INavigator>(provider => new Navigator(registry, provider, CurrentStack));
		Services = builder.Services.BuildServiceProvider();
	}


	public static BareApplicationBuilder CreateBuilder() =>
		new();

	public void Run(string[] args)
	{
		Current = this;
		UIApplication.Main(args, null, typeof(BareApplicationDelegate));
	}

	internal UIViewController BuildShell()
	{
		PageHost Page(Type? viewModel) =>
			new(registry.CreatePage(registry.CreateViewModel(viewModel!, Services)));

		UINavigationController Stack(Type? viewModel, bool prefersLargeTitles = false)
		{
			UINavigationController stack = new(Page(viewModel));
			stack.NavigationBar.PrefersLargeTitles = prefersLargeTitles;

			return stack;
		}

		switch (shell)
		{
				case ShellKind.SinglePage:
					return Page(rootViewModel);

				case ShellKind.Stack:
					return Stack(rootViewModel, PreferLargeTitles);

				case ShellKind.Tabs:
					UITabBarController controller = new();
					controller.ViewControllers = tabsBuilder?.Definitions
						.Select(UIViewController (definition) =>
						{
							UINavigationController stack = Stack(definition.ViewModel, tabsBuilder.UseLargeTitles);
							stack.ViewControllers![0].TabBarItem = new(definition.Title, UIImage.GetSystemImage(definition.Icon), null);

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
}
