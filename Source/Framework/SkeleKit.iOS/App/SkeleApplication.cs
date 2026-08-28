using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;

namespace SkeleKit;

/// <summary>
/// The core application instance that handles DI, navigation setup, and the app lifecycle.
/// </summary>
public class SkeleApplication
{
	const double TintTransitionDuration = 0.2;


	internal enum ShellKind
	{
		None,
		SinglePage,
		Stack,
		Tabs
	}

	internal sealed class TabsDelegate : UITabBarControllerDelegate
	{
		readonly SkeleApplication? app;

		public TabsDelegate(
			SkeleApplication app)
		{
			this.app = app;
		}

		public TabsDelegate(
			ObjCRuntime.NativeHandle handle) : base(handle)
		{ }


		public override bool ShouldSelectTab(
			UITabBarController tabBarController,
			UITab tab)
		{
			if (app is { ActionTab.Identifier: string action } && tab.Identifier == action)
			{
				CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() => app.BubbleAction?.Invoke());
				app.AttachBubbleInterceptor(tabBarController);

				return false;
			}

			if (tabBarController.SelectedTab?.Identifier == tab.Identifier)
			{
				HandleReselect(tabBarController);
				return true;
			}

			app?.BeginTabSelection();

			return true;
		}
	}

	internal sealed class SkeleStack : UINavigationController
	{
		public SkeleStack(
			UIViewController root) : base(root)
		{ }

		public SkeleStack(
			ObjCRuntime.NativeHandle handle) : base(handle)
		{ }


		public override UIViewController ChildViewControllerForStatusBarStyle() =>
			TopViewController;
	}



	/// <summary>
	/// The currently running application instance.
	/// </summary>
	public static SkeleApplication? Current { get; private set; }


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

	static UIView? FindBubbleView(
		UIView root)
	{
		if (FindByClass(root, "_UIFloatingTabBarPinnedItemsView") is not UIView pinned)
			return null;

		UIView? bubble = null;

		foreach (UIView subview in pinned.Subviews)
		{
			if (subview.Class.Name == "_UIFloatingTabBarItemView" && (bubble is null || subview.Frame.X > bubble.Frame.X))
				bubble = subview;
		}

		return bubble ?? pinned;
	}

	static UIView? FindByClass(
		UIView root,
		string name)
	{
		if (root.Class.Name == name)
			return root;

		foreach (UIView subview in root.Subviews)
		{
			if (FindByClass(subview, name) is UIView match)
				return match;
		}

		return null;
	}


	internal static ContentView? TopPage()
	{
		UIViewController? top = Root();

		while (top?.PresentedViewController is UIViewController presented)
			top = presented;

		if (top is UITabBarController tabs)
			top = tabs.SelectedViewController;

		if (top is UINavigationController stack)
			top = stack.TopViewController;

		return (top as PageHost)?.Page;
	}

	internal static void HandleReselect(
		UITabBarController controller)
	{
		if (controller.SelectedViewController is not UINavigationController stack)
			return;

		PageHost? root = stack.ViewControllers?.FirstOrDefault() as PageHost;

		ContentView? page = root?.Page;
		object? parameter = page?.TabReselectedCommandParameter;

		if (page?.TabReselectedCommand is ICommand command && command.CanExecute(parameter))
		{
			command.Execute(parameter);
			return;
		}

		if (stack.ViewControllers?.Length > 1)
		{
			stack.PopToRootViewController(true);
			return;
		}

		if (page is not null && PageHost.FindScrolling(page)?.Native is UIScrollView scroll)
			scroll.SetContentOffset(new(scroll.ContentOffset.X, -scroll.AdjustedContentInset.Top), true);
	}


	/// <summary>
	/// Creates a new builder to configure services and the layout shell.
	/// </summary>
	public static SkeleApplicationBuilder CreateBuilder() =>
		new();


	readonly ViewRegistry registry;
	readonly ShellKind shell;
	readonly bool preferLargeTitles;
	readonly TabsBuilder? tabsBuilder;
	readonly Type? rootView;
	Color? tint;
	Appearance appearance;
	TabBarMinimize tabBarMinimizeBehavior;

	internal SkeleApplication(
		SkeleApplicationBuilder builder)
	{
		registry = builder.Registry;
		shell = builder.Shell;
		preferLargeTitles = builder.PreferLargeTitles;
		tabsBuilder = builder.TabsBuilder;
		rootView = builder.RootView;
		tint = builder.Tint;
		appearance = builder.Appearance;
		tabBarMinimizeBehavior = tabsBuilder?.Minimize ?? TabBarMinimize.Never;

		Backgrounded = builder.LifecycleBackground;
		Foregrounded = builder.LifecycleForeground;

		builder.Services.AddSingleton<INavigator>(provider => new Navigator(registry, provider, CurrentStack));
		builder.Services.AddSingleton<ISharer>(_ => new Sharer());
		builder.Services.AddSingleton<ISystemPicker>(_ => new SystemPicker());
		Services = builder.Services.BuildServiceProvider();
	}


	internal UITabAccessory? Accessory { get; private set; }
	View? accessoryContent;
	AccessoryHost? accessoryHost;

	internal UITab? ActionTab { get; private set; }
	internal Action? BubbleAction { get; private set; }
	TabsDelegate? tabsDelegate;
	UILongPressGestureRecognizer? bubbleTap;

	View? footerContent;
	AccessoryHost? footerHost;

	internal bool AccessoryWanted => accessoryContent?.IsVisible.Value is true;
	internal bool IsSwitchingTabs { get; private set; }

	internal Action? Backgrounded { get; set; }
	internal Action? Foregrounded { get; set; }

	internal UIUserInterfaceStyle UserInterfaceStyle =>
		appearance switch
		{
			Appearance.Light => UIUserInterfaceStyle.Light,
			Appearance.Dark => UIUserInterfaceStyle.Dark,
			_ => UIUserInterfaceStyle.Unspecified
		};


	/// <summary>
	/// The built-in service provider for resolving dependencies.
	/// </summary>
	public IServiceProvider Services { get; }

	/// <summary>
	/// The app-wide tint inherited by windows, chrome and views, or null for the system default.
	/// </summary>
	public Color? Tint
	{
		get => tint;
		set
		{
			if (tint == value)
				return;

			if (Current == this && PageHost.InteractiveTintTransition is IUIViewControllerTransitionCoordinator transition)
			{
				transition.NotifyWhenInteractionChanges(context =>
				{
#pragma warning disable CA2011
					if (!context.IsCancelled)
						Tint = value;
#pragma warning restore CA2011
				});

				return;
			}

			tint = value;

			if (Current == this)
				ApplyTint();
		}
	}

	/// <summary>
	/// The app-wide light or dark appearance.
	/// </summary>
	public Appearance Appearance
	{
		get => appearance;
		set
		{
			if (appearance == value)
				return;

			appearance = value;

			if (Current == this)
				ApplyAppearance();
		}
	}

	/// <summary>
	/// When the app's tab bar minimizes as the selected content scrolls.
	/// </summary>
	/// <remarks>
	/// Applies to tab shells on iOS 26 and later. The value configured by
	/// <see cref="TabsBuilder.Minimizes(TabBarMinimize)"/> is the initial value.
	/// </remarks>
	public TabBarMinimize TabBarMinimizeBehavior
	{
		get => tabBarMinimizeBehavior;
		set
		{
			if (tabBarMinimizeBehavior == value)
				return;

			tabBarMinimizeBehavior = value;

			if (Current == this)
				ApplyTabBarMinimize();
		}
	}


	void ApplyTint()
	{
		UIWindowScene[] scenes =
		[
			.. UIApplication.SharedApplication
				.ConnectedScenes
				.OfType<UIWindowScene>()
		];
		UIWindow[] windows =
		[
			.. scenes.SelectMany(scene => scene.Windows)
		];

		void Apply()
		{
			foreach (UIWindow window in windows)
				window.TintColor = tint?.ToUIColor();

			PageHost.TintChanged();
			accessoryContent?.AppTintChanged();
			footerContent?.AppTintChanged();
		}

		if (UIAccessibility.IsReduceMotionEnabled)
		{
			Apply();
			return;
		}

		UIView.AnimateNotify(
			TintTransitionDuration,
			0,
			UIViewAnimationOptions.CurveEaseInOut
				| UIViewAnimationOptions.AllowUserInteraction
				| UIViewAnimationOptions.BeginFromCurrentState,
			Apply,
			static _ => { });
	}

	void ApplyAppearance()
	{
		foreach (UIWindow window in UIApplication.SharedApplication
			.ConnectedScenes
			.OfType<UIWindowScene>()
			.SelectMany(scene => scene.Windows))
			window.OverrideUserInterfaceStyle = UserInterfaceStyle;
	}

	void ApplyTabBarMinimize(
		UITabBarController? controller = null)
	{
		if (!OperatingSystem.IsIOSVersionAtLeast(26))
			return;

		controller ??= CurrentTabs();
		if (controller is null)
			return;

		controller.TabBarMinimizeBehavior = tabBarMinimizeBehavior switch
		{
			TabBarMinimize.OnScrollDown => UITabBarMinimizeBehavior.OnScrollDown,
			TabBarMinimize.OnScrollUp => UITabBarMinimizeBehavior.OnScrollUp,
			_ => UITabBarMinimizeBehavior.Never
		};
	}

	void BeginTabSelection() =>
		IsSwitchingTabs = true;

	void SyncAccessory()
	{
		if (Accessory is null
			|| !OperatingSystem.IsIOSVersionAtLeast(26)
			|| CurrentTabs() is not UITabBarController tabs)
			return;

		bool barHidden = (CurrentStack()?.TopViewController as PageHost)?.HidesBottomBarWhenPushed is true;

		tabs.SetBottomAccessory(AccessoryWanted && !barHidden ? Accessory : null, animated: true);
	}


	internal ContentView RecreatePage(
		ContentView page) =>
		registry.RecreatePage(page, Services);

	internal void CompleteTabSelection(
		PageHost host)
	{
		if (!IsSwitchingTabs || !ReferenceEquals(CurrentStack()?.TopViewController, host))
			return;

		IsSwitchingTabs = false;
	}

	internal void NotifyBackground() =>
		Backgrounded?.Invoke();

	internal void NotifyForeground() =>
		Foregrounded?.Invoke();

	internal void AttachBubbleInterceptor(
		UITabBarController controller)
	{
		if (ActionTab is null || BubbleAction is null)
			return;

		if (bubbleTap is not null)
			return;

		if (FindBubbleView(controller.View!) is not UIView bubble)
			return;

		UILongPressGestureRecognizer recognizer = null!;
		recognizer = new(() =>
		{
			if (recognizer.State is UIGestureRecognizerState.Began)
				BubbleAction?.Invoke();
		});

		recognizer.MinimumPressDuration = 0;
		recognizer.CancelsTouchesInView = true;

		bubbleTap = recognizer;
		bubble.AddGestureRecognizer(recognizer);
	}

	internal UIViewController BuildShell()
	{
		PageHost Page(Type? view) =>
			new(registry.CreatePage(view!, Services));

		UINavigationController Stack(Type? view, bool prefersLargeTitles = false)
		{
			UINavigationController stack = new SkeleStack(Page(view));
			stack.NavigationBar.PrefersLargeTitles = prefersLargeTitles;

			return stack;
		}

		switch (shell)
		{
			case ShellKind.SinglePage:
				return Page(rootView);

			case ShellKind.Stack:
				return Stack(rootView, preferLargeTitles);

			case ShellKind.Tabs:
				UITabBarController controller = new();

				bool pad = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
				PadTabsBuilder? iPad = pad ? tabsBuilder?.Pad : null;

				void Place(UITab tab, TabPlacement placement)
				{
					if (placement is not TabPlacement.Automatic)
					{
						tab.PreferredPlacement = placement switch
						{
							TabPlacement.Pinned => UITabPlacement.Pinned,
							TabPlacement.SidebarOnly => UITabPlacement.SidebarOnly,
							TabPlacement.Optional => UITabPlacement.Optional,
							_ => UITabPlacement.Fixed
						};
					}

					if (placement is TabPlacement.Locked)
						tab.AllowsHiding = false;
				}

				UITab BuildTab(TabsBuilder.Node node, bool grouped)
				{
					if (node is TabsBuilder.GroupNode group)
					{
						UITabGroup native = new(
							group.Title,
							UIImage.GetSystemImage(group.Icon),
							$"group:{group.Title}",
							[.. group.Children.Select(child => BuildTab(child, true))],
							null!);

						// only the outermost group manages the stack; nested ones inherit it
						if (!grouped)
						{
							UINavigationController shared = new();
							shared.NavigationBar.PrefersLargeTitles = tabsBuilder!.UseLargeTitles;

							native.ManagingNavigationController = shared;
						}

						// a group is a sidebar section, never a bar item
						Place(native, TabPlacement.SidebarOnly);

						return native;
					}

					TabsBuilder.Leaf leaf = (TabsBuilder.Leaf)node;

					PageHost root;
					Func<UITab, UIViewController> provider;

					if (grouped)
					{
						root = Page(leaf.View);
						provider = _ => root;
					}
					else
					{
						UINavigationController stack = Stack(leaf.View, tabsBuilder!.UseLargeTitles);
						root = (PageHost)stack.ViewControllers![0];
						provider = _ => stack;
					}

					UITab tab = new(
						leaf.Title,
						UIImage.GetSystemImage(leaf.Icon),
						leaf.View.Name,
						provider);

					Place(tab, iPad?.Placements.GetValueOrDefault(leaf.View, leaf.Placement) ?? leaf.Placement);

					root.Tab = tab;
					root.Page?.ApplyTabBadge();

					return tab;
				}

				List<UITab> tabs = [.. (tabsBuilder?.Nodes ?? []).Select(node => BuildTab(node, false))];

				if (iPad is not null)
					tabs.AddRange(iPad.Nodes.Select(node => BuildTab(node, false)));

				if (tabsBuilder is { SearchView: not null } and ({ BubbleFactory: not null } or { BubbleView: not null }))
					throw new InvalidOperationException("The bubble is single: declare Search or Bubble, not both.");

				if (tabsBuilder?.SearchView is Type searchView)
				{
					UINavigationController stack = Stack(searchView, tabsBuilder.UseLargeTitles);
					PageHost root = (PageHost)stack.ViewControllers![0];

					UISearchTab search = new(_ => stack);
					root.Tab = search;

					root.LoadViewIfNeeded();

					tabs.Add(search);
				}
				else if (tabsBuilder?.BubbleView is Type bubbleView)
				{
					UINavigationController stack = Stack(bubbleView, tabsBuilder.UseLargeTitles);
					UITab bubble;

					if (OperatingSystem.IsIOSVersionAtLeast(26))
					{
						bubble = new UISearchTab(_ => stack)
						{
							Title = tabsBuilder.BubbleTitle!,
							Image = UIImage.GetSystemImage(tabsBuilder.BubbleIcon!),
							AutomaticallyActivatesSearch = false
						};
					}
					else
					{
						bubble = new UITab(
							tabsBuilder.BubbleTitle!,
							UIImage.GetSystemImage(tabsBuilder.BubbleIcon!),
							bubbleView.Name,
							_ => stack);
					}

					PageHost root = (PageHost)stack.ViewControllers![0];
					root.Tab = bubble;
					root.Page?.ApplyTabBadge();
					tabs.Add(bubble);
				}
				else if (tabsBuilder?.BubbleFactory is Func<IServiceProvider, Action> action)
				{
					BubbleAction = action(Services);
					UITab bubble;

					if (OperatingSystem.IsIOSVersionAtLeast(26))
					{
						bubble = new UISearchTab(static _ => new())
						{
							Title = tabsBuilder.BubbleTitle!,
							Image = UIImage.GetSystemImage(tabsBuilder.BubbleIcon!),
							AutomaticallyActivatesSearch = false
						};
					}
					else
					{
						bubble = new UITab(
							tabsBuilder.BubbleTitle!,
							UIImage.GetSystemImage(tabsBuilder.BubbleIcon!),
							$"action:{tabsBuilder.BubbleTitle}",
							static _ => new());
					}

					ActionTab = bubble;
					tabs.Add(bubble);

					if (OperatingSystem.IsIOSVersionAtLeast(26))
						CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() => AttachBubbleInterceptor(controller));
				}

				controller.SetTabs([.. tabs], false);

				tabsDelegate = new(this);
				controller.Delegate = tabsDelegate;

				if (iPad?.UseSidebar is true)
					controller.Mode = UITabBarControllerMode.TabSidebar;

				ApplyTabBarMinimize(controller);

				if (tabsBuilder?.AccessoryFactory is Func<View> accessory && OperatingSystem.IsIOSVersionAtLeast(26))
				{
					accessoryContent = accessory();
					accessoryContent.VisibilityChanged = SyncAccessory;
					accessoryHost = new(accessoryContent);
					Accessory = new(accessoryHost);

					if (AccessoryWanted)
						controller.BottomAccessory = Accessory;
				}

				if (iPad?.FooterFactory is Func<View> footer && OperatingSystem.IsIOSVersionAtLeast(26))
				{
					footerContent = footer();
					footerHost = AccessoryHost.ForKeyboard(footerContent);
					controller.Sidebar.BottomBarView = footerHost;
				}

				return controller;

			case ShellKind.None:
			default:
				throw new InvalidOperationException("Call Tabs(...), Stack<TView>() or SinglePage<TView>() before Run().");
		}
	}


	/// <summary>
	/// Starts the native iOS main loop.
	/// </summary>
	public void Run(
		string[] args)
	{
		Current = this;
		HotReload.Start();
		UIApplication.Main(args, null, typeof(SkeleApplicationDelegate));
	}
}
