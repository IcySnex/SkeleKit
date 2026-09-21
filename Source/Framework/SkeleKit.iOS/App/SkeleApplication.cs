using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
		Split,
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

	internal sealed class SidebarDelegate : UITabBarControllerSidebarDelegate
	{
		readonly SkeleApplication? app;

		public SidebarDelegate(
			SkeleApplication app)
		{
			this.app = app;
		}

		public SidebarDelegate(
			ObjCRuntime.NativeHandle handle) : base(handle)
		{ }


		public override void SidebarAvailabilityDidChange(
			UITabBarController tabBarController,
			UITabBarControllerSidebar sidebar) =>
			app?.RefreshSidebarRecovery();

		public override void SidebarVisibilityWillChange(
			UITabBarController tabBarController,
			UITabBarControllerSidebar sidebar,
			IUITabBarControllerSidebarAnimating animator)
		{
			animator.AddAnimations(() =>
			{
				if (OperatingSystem.IsIOSVersionAtLeast(27))
					app?.AnimateSplitContentWithSidebar(tabBarController);

				if (sidebar.Hidden)
					app?.RefreshSidebarRecovery();
			});

			if (!sidebar.Hidden)
				app?.RefreshSidebarRecovery();

			animator.AddCompletion(() => app?.RefreshSidebarRecovery());
		}
	}

	internal sealed class SkeleStack : UINavigationController
	{
		nfloat inheritedTopInsetCorrection;
		nfloat originalAdditionalTopInset;
		bool tracksInheritedTopInset;


		public SkeleStack(
			UIViewController root,
			bool prefersLargeTitles = false) : base(root)
		{
			NavigationBar.PrefersLargeTitles = prefersLargeTitles;
		}

		public SkeleStack(
			bool prefersLargeTitles)
		{
			NavigationBar.PrefersLargeTitles = prefersLargeTitles;
		}

		public SkeleStack(
			ObjCRuntime.NativeHandle handle) : base(handle)
		{ }


		public override UIViewController ChildViewControllerForStatusBarStyle() =>
			TopViewController;

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			CorrectInheritedTopInset();
		}

		void CorrectInheritedTopInset()
		{
			// iOS 26 offsets the floating primary column below the tab bar and also passes
			// that bar's top safe area into its navigation stack. iOS 18 doesn't use this
			// layout, while iOS 27's edge-to-edge sidebar needs its native animated inset.
			if (!OperatingSystem.IsIOSVersionAtLeast(26)
				|| OperatingSystem.IsIOSVersionAtLeast(27)
				|| ParentViewController is not UISplitViewController split
				|| split.TabBarController is null
				|| split.Collapsed
				|| !ReferenceEquals(split.GetViewController(UISplitViewControllerColumn.Primary), this))
			{
				RestoreInheritedTopInset();
				return;
			}

			if (!tracksInheritedTopInset)
			{
				originalAdditionalTopInset = AdditionalSafeAreaInsets.Top;
				tracksInheritedTopInset = true;
			}

			CGRect frame = View!.ConvertRectToView(View.Bounds, split.View);
			nfloat inheritedTopInset = View.SafeAreaInsets.Top + inheritedTopInsetCorrection;
			nfloat expectedTopInset = (nfloat)Math.Max(
				0,
				(double)(split.View!.SafeAreaInsets.Top - Math.Max(0, frame.GetMinY())));
			nfloat correction = (nfloat)Math.Max(0, (double)(inheritedTopInset - expectedTopInset));

			if (Math.Abs((double)(correction - inheritedTopInsetCorrection)) < 0.5)
				return;

			inheritedTopInsetCorrection = correction;
			UIEdgeInsets additional = AdditionalSafeAreaInsets;
			additional.Top = originalAdditionalTopInset - correction;
			AdditionalSafeAreaInsets = additional;
		}

		void RestoreInheritedTopInset()
		{
			if (!tracksInheritedTopInset)
				return;

			UIEdgeInsets additional = AdditionalSafeAreaInsets;
			additional.Top = originalAdditionalTopInset;
			AdditionalSafeAreaInsets = additional;
			inheritedTopInsetCorrection = 0;
			tracksInheritedTopInset = false;
		}
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

	static UINavigationController? StackFor(
		UIViewController? controller) =>
		controller switch
		{
			UINavigationController stack => stack,
			UITabBarController tabs => StackFor(tabs.SelectedViewController),
			SkeleSplit split => split.NavigationStack,
			UISplitViewController split => split.ViewControllers
				.Select(StackFor)
				.LastOrDefault(stack => stack is not null),
			_ => null
		};

	static UINavigationController? CurrentShellStack() =>
		StackFor(Root());

	/// <summary>
	/// Walks the presented controller chain of the key window once and reports the deepest
	/// navigation stack plus the active split view it passes through.
	/// </summary>
	static (UINavigationController? Stack, SkeleSplit? Split) WalkShell()
	{
		UIViewController? controller = Root();
		UINavigationController? stack = null;
		SkeleSplit? split = null;

		while (controller is not null)
		{
			if (controller is UINavigationController navigation)
				stack = navigation;
			if (controller is SkeleSplit candidate)
				split = candidate;

			// A presentation is visually above its presenter's container hierarchy.
			// Follow it first, then continue through the selected/visible child so the
			// deepest presented navigation controller becomes the active stack.
			UIViewController? next = controller.PresentedViewController
				?? controller switch
				{
					UITabBarController tabs => tabs.SelectedViewController,
					UINavigationController container => container.TopViewController,
					SkeleSplit active => active.NavigationStack,
					UISplitViewController plain => plain.ViewControllers.LastOrDefault(),
					_ => null
				};

			if (next is null || ReferenceEquals(next, controller))
				break;

			controller = next;
		}

		return (stack, split);
	}

	static UINavigationController? ActiveStack() =>
		WalkShell().Stack;

	static SkeleSplit? ActiveSplit() =>
		WalkShell().Split;

	/// <summary>
	/// Resolves the navigation stack of a split view column. When the split view is collapsed,
	/// every column routes to the single stack the user can actually see.
	/// </summary>
	static UINavigationController? ColumnStack(
		SplitViewColumn column)
	{
		if (ActiveSplit() is not SkeleSplit split)
			return null;

		if (split.Collapsed)
			return split.NavigationStack;

		return split.GetViewController(SkeleSplit.Native(column)) as UINavigationController;
	}

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

		if (top is SkeleSplit split)
			top = split.NavigationStack;

		if (top is UINavigationController stack)
			top = stack.TopViewController;

		return (top as PageHost)?.Page;
	}

	internal static void HandleReselect(
		UITabBarController controller)
	{
		if (StackFor(controller.SelectedViewController) is not UINavigationController stack)
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
	/// <returns>A new application builder.</returns>
	public static SkeleApplicationBuilder CreateBuilder() =>
		new();


	readonly ViewRegistry registry;
	readonly ShellKind shell;
	readonly TabsBuilder? tabsBuilder;
	readonly SplitViewBuilder? splitViewBuilder;
	readonly Type? rootView;
	IReadOnlyList<IApplicationLifecycle> lifecycleServices = [];
	bool nativeApplicationStarted;
	int lifecycleStopped;

	internal SkeleApplication(
		SkeleApplicationBuilder builder)
	{
		registry = builder.Registry;
		shell = builder.Shell;
		tabsBuilder = builder.TabsBuilder;
		splitViewBuilder = builder.SplitViewBuilder;
		rootView = builder.RootView;
		Theme = builder.Theme.Theme;
		Theme.Changed += ApplyThemeChange;

		builder.Services.AddSingleton<INavigator>(provider => new Navigator(registry, provider, ActiveStack, ColumnStack));
		builder.Services.AddSingleton<ISplitView>(_ => new SplitViewService(ActiveSplit));
		builder.Services.AddSingleton<ISharer, Sharer>();
		builder.Services.AddSingleton<ISystemPicker, SystemPicker>();
		builder.Services.AddSingleton<IHaptics, Haptics>();
		builder.Services.AddSingleton<IMailer, Mailer>();

		Services = builder.Services.BuildServiceProvider();

		Navigator = Services.GetRequiredService<INavigator>();
		Sharer = Services.GetRequiredService<ISharer>();
		SystemPicker = Services.GetRequiredService<ISystemPicker>();
		Haptics = Services.GetRequiredService<IHaptics>();
		Mailer = Services.GetRequiredService<IMailer>();
	}


	internal UITabAccessory? Accessory { get; private set; }
	View? accessoryContent;
	AccessoryHost? accessoryHost;

	internal UITab? ActionTab { get; private set; }
	internal Action? BubbleAction { get; private set; }
	TabsDelegate? tabsDelegate;
	SidebarDelegate? sidebarDelegate;
	UILongPressGestureRecognizer? bubbleTap;

	View? footerContent;
	AccessoryHost? footerHost;

	internal bool AccessoryWanted => accessoryContent?.IsVisible.Value is true;
	internal bool IsSwitchingTabs { get; private set; }

	internal UIUserInterfaceStyle UserInterfaceStyle =>
		Theme.Appearance switch
		{
			Appearance.Light => UIUserInterfaceStyle.Light,
			Appearance.Dark => UIUserInterfaceStyle.Dark,
			_ => UIUserInterfaceStyle.Unspecified
		};


	/// <summary>
	/// The built-in service provider for resolving dependencies.
	/// </summary>
	public IServiceProvider Services { get; }

	internal INavigator Navigator { get; }
	internal ISharer Sharer { get; }
	internal ISystemPicker SystemPicker { get; }
	internal IHaptics Haptics { get; }
	internal IMailer Mailer { get; }

	/// <summary>
	/// The app-wide theme inherited by windows, chrome and pages.
	/// </summary>
	public Theme Theme { get; }


	void ApplyThemeChange(
		ThemeField field)
	{
		if (Current != this || !nativeApplicationStarted)
			return;

		switch (field)
		{
			case ThemeField.Tint:
				ApplyTint();
				break;

			case ThemeField.Appearance:
				ApplyAppearance();
				break;

			case ThemeField.TopScrollEdgeStyle:
				PageHost.TopScrollEdgeStyleChanged();
				break;

			case ThemeField.NavigationTitleStyle:
				PageHost.TitleStyleChanged();
				break;

			case ThemeField.TabBarMinimize:
				ApplyTabBarMinimize();
				break;
		}
	}

	void ApplyTint()
	{
		// let an in-flight interactive transition settle before recoloring
		if (PageHost.InteractiveTintTransition is IUIViewControllerTransitionCoordinator transition)
		{
			transition.NotifyWhenInteractionChanges(context =>
			{
				if (!context.IsCancelled)
					ApplyTintNow();
			});

			return;
		}

		ApplyTintNow();
	}

	void ApplyTintNow()
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
				window.TintColor = Theme.Tint?.ToUIColor();

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

		controller.TabBarMinimizeBehavior = Theme.TabBarMinimize switch
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

		bool barHidden = (CurrentShellStack()?.TopViewController as PageHost)?.HidesBottomBarWhenPushed is true;

		tabs.SetBottomAccessory(AccessoryWanted && !barHidden ? Accessory : null, animated: true);
	}


	internal ContentView RecreatePage(
		ContentView page) =>
		registry.RecreatePage(page, Services);

	internal void CompleteTabSelection(
		PageHost host)
	{
		if (!IsSwitchingTabs || !ReferenceEquals(CurrentShellStack()?.TopViewController, host))
			return;

		IsSwitchingTabs = false;
	}

	internal bool ShowsSidebarRecovery(
		PageHost host)
	{
		return OperatingSystem.IsIOSVersionAtLeast(27)
			&& CurrentTabs() is UITabBarController tabs
			&& tabs.TraitCollection.UserInterfaceIdiom is UIUserInterfaceIdiom.Phone
			&& tabs.Sidebar.IsAvailable
			&& tabs.Sidebar.Hidden
			&& tabs.SelectedViewController is SkeleSplit split
			&& ReferenceEquals(split.NavigationStack?.TopViewController, host);
	}

	internal static UIBarButtonItem SidebarRecoveryItem()
	{
		UIAction show = UIAction.Create(
			"Show App Sidebar",
			UIImage.GetSystemImage("sidebar.leading"),
			null,
			static _ =>
			{
				if (CurrentTabs() is UITabBarController tabs)
					tabs.Sidebar.Hidden = false;
			});

		UIBarButtonItem item = new(show)
		{
			AccessibilityIdentifier = "SkeleKit.TabSidebarRecovery"
		};

		if (OperatingSystem.IsIOSVersionAtLeast(27))
			item.VisibilityPriority = UIBarButtonItemVisibilityPriority.High;

		return item;
	}

	internal void RefreshSidebarRecovery()
	{
		if (CurrentTabs()?.SelectedViewController is SkeleSplit split)
		{
			if (split.NavigationStack?.TopViewController is PageHost host)
				host.RefreshSidebarRecovery();
			return;
		}

		if (CurrentShellStack()?.TopViewController is PageHost current)
			current.RefreshSidebarRecovery();
	}

	void AnimateSplitContentWithSidebar(
		UITabBarController tabs)
	{
		if (tabs.SelectedViewController is not UISplitViewController split)
			return;

		foreach (PageHost host in split.ViewControllers
			.Select(controller => (controller as UINavigationController)?.TopViewController)
			.OfType<PageHost>())
			host.AnimateAlongsideSidebar();
	}

	internal void SplitNavigationStackChanged(
		SkeleSplit split,
		UINavigationController? previous,
		UINavigationController? current)
	{
		if (!ReferenceEquals(CurrentTabs()?.SelectedViewController, split))
			return;

		if (previous?.TopViewController is PageHost previousHost)
			previousHost.RefreshSidebarRecovery();

		if (!ReferenceEquals(previous, current)
			&& current?.TopViewController is PageHost currentHost)
			currentHost.RefreshSidebarRecovery();
	}

	async Task InvokeLifecycleAsync(
		Func<IApplicationLifecycle, Task> callback,
		bool reverse = false)
	{
		if (reverse)
		{
			for (int i = lifecycleServices.Count - 1; i >= 0; i--)
				await callback(lifecycleServices[i]);

			return;
		}

		foreach (IApplicationLifecycle lifecycle in lifecycleServices)
			await callback(lifecycle);
	}

	void ObserveLifecycle(
		Task transition,
		string name) =>
		ObserveLifecycleCore(transition, name);

	async void ObserveLifecycleCore(
		Task transition,
		string name)
	{
		try
		{
			await transition;
		}
		catch (Exception exception)
		{
			Services.GetRequiredService<ILogger<SkeleApplication>>()
				.LogError(exception, "Application lifecycle transition {Transition} failed.", name);
		}
	}

	internal void NotifyBackground() =>
		ObserveLifecycle(
			InvokeLifecycleAsync(lifecycle => lifecycle.EnterBackgroundAsync(), reverse: true),
			nameof(IApplicationLifecycle.EnterBackgroundAsync));

	internal void NotifyForeground() =>
		ObserveLifecycle(
			InvokeLifecycleAsync(lifecycle => lifecycle.EnterForegroundAsync()),
			nameof(IApplicationLifecycle.EnterForegroundAsync));

	internal void NotifyNativeApplicationStarted() =>
		nativeApplicationStarted = true;

	internal void NotifyStopping()
	{
		if (Interlocked.Exchange(ref lifecycleStopped, 1) != 0)
			return;

		ObserveLifecycle(
			InvokeLifecycleAsync(lifecycle => lifecycle.StopAsync(), reverse: true),
			nameof(IApplicationLifecycle.StopAsync));
	}

	internal void AttachBubbleInterceptor(
		UITabBarController controller)
	{
		if (!OperatingSystem.IsIOSVersionAtLeast(26)
			|| ActionTab is null
			|| BubbleAction is null)
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

	static void Hide(
		UITab tab)
	{
		// The sidebar hides through the projected placement, the tab bar through the
		// key-value hidden property that the .NET binding does not project yet.
		tab.PreferredPlacement = UITabPlacement.SidebarOnly;
		tab.AllowsHiding = false;
		tab.SetValueForKey(NSNumber.FromBoolean(true), new NSString("hidden"));
	}

	internal UIViewController BuildShell()
	{
		PageHost Page(Type? view) =>
			new(registry.CreatePage(view!, Services));

		UINavigationController Stack(Type? view)
			=> new SkeleStack(Page(view), Theme.NavigationTitleStyle is TitleStyle.Large);

		(SkeleSplit Controller, Dictionary<SplitViewColumn, PageHost> Hosts) Split(
			SplitViewBuilder builder)
		{
			SkeleSplit split = new(builder);
			Dictionary<SplitViewColumn, PageHost> hosts = [];

			foreach ((SplitViewColumn column, Type view) in builder.Columns)
			{
				if (column is SplitViewColumn.Inspector
					&& !OperatingSystem.IsIOSVersionAtLeast(26))
					continue;

				PageHost host = Page(view);
				SkeleStack stack = new(host, Theme.NavigationTitleStyle is TitleStyle.Large);
				split.SetViewController(stack, SkeleSplit.Native(column));
				hosts[column] = host;
			}

			split.ApplyWeights(builder);

			return (split, hosts);
		}

		switch (shell)
		{
			case ShellKind.SinglePage:
				return Page(rootView);

			case ShellKind.Stack:
				return Stack(rootView);

			case ShellKind.Split:
				return Split(splitViewBuilder!).Controller;

			case ShellKind.Tabs:
				UITabBarController controller = new();
				HashSet<string> compactExcludedTabs = [];

				SidebarBuilder? sidebar = tabsBuilder?.SidebarConfiguration;

				void Place(UITab tab, TabPlacement placement)
				{
					if (placement is TabPlacement.Hidden)
					{
						Hide(tab);
						compactExcludedTabs.Add(tab.Identifier);
						return;
					}

					tab.PreferredPlacement = placement switch
					{
						TabPlacement.Automatic => UITabPlacement.Automatic,
						TabPlacement.Default => UITabPlacement.Default,
						TabPlacement.Optional => UITabPlacement.Optional,
						TabPlacement.Movable => UITabPlacement.Movable,
						TabPlacement.Pinned => UITabPlacement.Pinned,
						TabPlacement.Fixed => UITabPlacement.Fixed,
						TabPlacement.SidebarOnly => UITabPlacement.SidebarOnly,
						_ => throw new ArgumentOutOfRangeException(nameof(placement), placement, null)
					};

					if (placement is TabPlacement.SidebarOnly)
						compactExcludedTabs.Add(tab.Identifier);
				}

				UITab BuildTab(TabsBuilder.Node node, bool grouped)
				{
					if (node is TabsBuilder.GroupNode group)
					{
						UITabGroup native = new(
							group.Title,
							group.Icon.ResolveLocal(),
							$"group:{group.Title}",
							[.. group.Children.Select(child => BuildTab(child, true))],
							null!);

						// only the outermost group manages the stack; nested ones inherit it
						if (!grouped)
						{
							UINavigationController shared = new SkeleStack(Theme.NavigationTitleStyle is TitleStyle.Large);

							native.ManagingNavigationController = shared;
						}

						// a group is a sidebar section, never a bar item
						Place(native, group.Placement);

						return native;
					}

					if (node is TabsBuilder.SplitLeaf splitLeaf)
					{
						if (grouped)
							throw new InvalidOperationException("A split view can't be nested inside a tab group.");

						(SkeleSplit split, Dictionary<SplitViewColumn, PageHost> hosts) = Split(splitLeaf.Split);
						UITab splitTab = new(
							splitLeaf.Title,
							splitLeaf.Icon.ResolveLocal(),
							$"split:{splitLeaf.Title}",
							_ => split);

						Place(splitTab, splitLeaf.Placement);

						// the navigation column is the tab's primary interaction surface,
						// so only its root page drives the tab badge
						if (hosts.TryGetValue(splitLeaf.Split.NavigationTarget, out PageHost? badgeRoot))
						{
							badgeRoot.Tab = splitTab;
							badgeRoot.Page?.ApplyTabBadge();
						}

						return splitTab;
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
						UINavigationController stack = Stack(leaf.View);
						root = (PageHost)stack.ViewControllers![0];
						provider = _ => stack;
					}

					UITab tab = new(
						leaf.Title,
						leaf.Icon.ResolveLocal(),
						leaf.View.Name,
						provider);

					Place(tab, leaf.Placement);

					root.Tab = tab;
					root.Page?.ApplyTabBadge();

					return tab;
				}

				List<UITab> tabs = [.. (tabsBuilder?.Nodes ?? []).Select(node => BuildTab(node, false))];

				if (tabsBuilder is { SearchView: not null } and ({ BubbleFactory: not null } or { BubbleView: not null }))
					throw new InvalidOperationException("The bubble is single: declare Search or Bubble, not both.");

				UITab? prominentTab = null;

				if (tabsBuilder?.SearchView is Type searchView)
				{
					UINavigationController stack = Stack(searchView);
					PageHost root = (PageHost)stack.ViewControllers![0];

					UITab search;
					if (tabsBuilder.SearchBubble)
					{
						search = new UISearchTab(_ => stack);
						if (OperatingSystem.IsIOSVersionAtLeast(27))
							prominentTab = search;
					}
					else
					{
						search = new UITab(
							root.Page?.Title.Value ?? "Search",
							UIImage.GetSystemImage("magnifyingglass"),
							searchView.Name,
							_ => stack);
					}
					root.Tab = search;

					root.LoadViewIfNeeded();

					tabs.Add(search);
				}
				else if (tabsBuilder?.BubbleView is Type bubbleView)
				{
					UINavigationController stack = Stack(bubbleView);
					UIImage? bubbleImage = tabsBuilder.BubbleIcon is ImageSource icon ? icon.ResolveLocal() : null;
					UITab bubble;

					if (OperatingSystem.IsIOSVersionAtLeast(27))
					{
						bubble = new UITab(
							tabsBuilder.BubbleTitle!,
							bubbleImage,
							bubbleView.Name,
							_ => stack)
						{
							PreferredPlacement = UITabPlacement.Pinned
						};
						prominentTab = bubble;
					}
					else if (OperatingSystem.IsIOSVersionAtLeast(26))
					{
						bubble = new UISearchTab(_ => stack)
						{
							Title = tabsBuilder.BubbleTitle!,
							Image = bubbleImage,
							AutomaticallyActivatesSearch = false
						};
					}
					else
					{
						bubble = new UITab(
							tabsBuilder.BubbleTitle!,
							bubbleImage,
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
					UIImage? bubbleImage = tabsBuilder.BubbleIcon is ImageSource icon ? icon.ResolveLocal() : null;
					UITab bubble;

					if (OperatingSystem.IsIOSVersionAtLeast(27))
					{
						bubble = new UITab(
							tabsBuilder.BubbleTitle!,
							bubbleImage,
							$"action:{tabsBuilder.BubbleTitle}",
							static _ => new())
						{
							PreferredPlacement = UITabPlacement.Pinned
						};
						prominentTab = bubble;
					}
					else if (OperatingSystem.IsIOSVersionAtLeast(26))
					{
						bubble = new UISearchTab(static _ => new())
						{
							Title = tabsBuilder.BubbleTitle!,
							Image = bubbleImage,
							AutomaticallyActivatesSearch = false
						};
					}
					else
					{
						bubble = new UITab(
							tabsBuilder.BubbleTitle!,
							bubbleImage,
							$"action:{tabsBuilder.BubbleTitle}",
							static _ => new());
					}

					ActionTab = bubble;
					tabs.Add(bubble);

					if (OperatingSystem.IsIOSVersionAtLeast(26))
						CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() => AttachBubbleInterceptor(controller));
				}

				controller.SetTabs([.. tabs], false);
				if (compactExcludedTabs.Count > 0)
					controller.CompactTabIdentifiers =
					[
						.. tabs
							.Where(tab => !compactExcludedTabs.Contains(tab.Identifier))
							.Select(tab => tab.Identifier)
					];
				if (OperatingSystem.IsIOSVersionAtLeast(27) && prominentTab is not null)
					controller.ProminentTabIdentifier = prominentTab.Identifier;

				tabsDelegate = new(this);
				controller.Delegate = tabsDelegate;

				if (sidebar is not null)
				{
					controller.Mode = UITabBarControllerMode.TabSidebar;
					if (OperatingSystem.IsIOSVersionAtLeast(27))
					{
						controller.Sidebar.PreferredPlacement = UITabBarControllerSidebarPlacement.Sidebar;
						sidebarDelegate = new(this);
						controller.Sidebar.Delegate = sidebarDelegate;
					}
				}

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

				if (sidebar?.FooterFactory is Func<View> footer && OperatingSystem.IsIOSVersionAtLeast(26))
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
	/// <param name="args">The application command-line arguments.</param>
	public void Run(
		string[] args)
	{
		Current = this;
		lifecycleServices = Services.GetServices<IApplicationLifecycle>().ToArray();
		InvokeLifecycleAsync(lifecycle => lifecycle.StartAsync())
			.GetAwaiter()
			.GetResult();

		HotReload.Start();

		try
		{
			UIApplication.Main(args, null, typeof(SkeleApplicationDelegate));
		}
		finally
		{
			NotifyStopping();
		}
	}
}
