using System.Reflection.Metadata;
using System.Windows.Input;
using ObjCRuntime;

namespace SkeleKit;

internal sealed class PageHost : UIViewController
{
	sealed class SheetGuard : UIAdaptivePresentationControllerDelegate
	{
		readonly PageHost? host;

		public SheetGuard(
			PageHost host)
		{
			this.host = host;
		}

		// ReSharper disable once UnusedMember.Local
		public SheetGuard(
			NativeHandle handle) : base(handle)
		{ }


		public override void DidAttemptToDismiss(
			UIPresentationController presentationController) =>
			host?.ConfirmDismiss();

		public override bool ShouldDismiss(
			UIPresentationController presentationController)
		{
			if (presentationController is not UIPopoverPresentationController)
				return true;

			host?.ConfirmDismiss();
			return false;
		}

		public override UIModalPresentationStyle GetAdaptivePresentationStyle(
			UIPresentationController forPresentationController) =>
			UIModalPresentationStyle.None;
	}


	static readonly List<WeakReference<PageHost>> Live = [];
	static readonly UIImage TransparentScopeBackground = new();

	static IUIViewControllerTransitionCoordinator? appearingTransition;


	internal static IUIViewControllerTransitionCoordinator? InteractiveTintTransition =>
		appearingTransition is { InitiallyInteractive: true } ? appearingTransition : null;


	static UIView? FirstResponder(
		UIView view)
	{
		if (view.IsFirstResponder)
			return view;

		foreach (UIView child in view.Subviews)
		{
			if (FirstResponder(child) is UIView found)
				return found;
		}

		return null;
	}

	static bool IsWithin(
		UIView ancestor,
		UIView? view)
	{
		while (view is not null)
		{
			if (ReferenceEquals(view, ancestor))
				return true;

			view = view.Superview;
		}

		return false;
	}

	static CGRect Inset(
		CGRect bounds,
		UIEdgeInsets insets,
		SafeAreaEdges edges)
	{
		nfloat top = edges.HasFlag(SafeAreaEdges.Top) ? insets.Top : 0;
		nfloat bottom = edges.HasFlag(SafeAreaEdges.Bottom) ? insets.Bottom : 0;
		nfloat leading = edges.HasFlag(SafeAreaEdges.Leading) ? insets.Left : 0;
		nfloat trailing = edges.HasFlag(SafeAreaEdges.Trailing) ? insets.Right : 0;

		return new(
			bounds.X + leading,
			bounds.Y + top,
			bounds.Width - leading - trailing,
			bounds.Height - top - bottom);
	}


	internal static void ReloadLive()
	{
		if (!MetadataUpdater.IsSupported)
			return;

		UIApplication.SharedApplication.InvokeOnMainThread(() =>
		{
			ForEachLive(host => host.Reload());
		});
	}

	internal static void TintChanged() =>
		ForEachLive(host =>
		{
			host.Page?.AppTintChanged();

			if (host.IsViewLoaded
				&& host.Page is ContentView page
				&& ReferenceEquals(host.NavigationController?.TopViewController, host))
			{
				host.ApplyNavigationTint(page);
				host.ApplyToolbarTint(page);
			}
		});

	static void ForEachLive(
		Action<PageHost> action)
	{
		for (int index = Live.Count - 1; index >= 0; index--)
		{
			if (Live[index].TryGetTarget(out PageHost? host))
				action(host);
			else
				Live.RemoveAt(index);
		}
	}

	static void RemoveLive(
		PageHost target)
	{
		for (int index = Live.Count - 1; index >= 0; index--)
		{
			if (!Live[index].TryGetTarget(out PageHost? host) || ReferenceEquals(host, target))
				Live.RemoveAt(index);
		}
	}

	internal static View? FindScrolling(
		View view)
	{
		if (view.Scrolls)
			return view;

		if (view is Panel panel)
		{
			foreach (View child in panel.Children)
			{
				if (FindScrolling(child) is View match)
					return match;
			}
		}

		return null;
	}


	// ReSharper disable once CollectionNeverQueried.Local
	readonly List<UIAction> menuActions = [];
	readonly List<ToolbarItem> observedItems = [];
	readonly Dictionary<UIBarButtonItem, ToolbarItem> nativeToolbarItems = [];

	UITapGestureRecognizer? dismissKeyboard;
	UIView? keyboardFocus;
	nfloat keyboardCover;
	bool usesSystemScrollInsets;
	IUITraitChangeRegistration? themeChange;
	UISearchController? search;
	UIAction? backAction;
	SheetGuard? dismissGuard;
	bool hasContentDetent;
	bool contentDetentPending;
	nfloat contentWidth;
	nfloat contentChrome;
	UINavigationBarAppearance? savedScrollEdgeAppearance;
	UINavigationBarAppearance? savedCompactScrollEdgeAppearance;
	bool preservesNavigationBarAppearance;

	public PageHost(
		ContentView page)
	{
		Page = page;
		page.Host = this;

		HidesBottomBarWhenPushed = page.HidesTabBar;

		NSNotificationCenter.DefaultCenter.AddObserver(
			this,
			new("keyboardFrameChanged:"),
			UIKeyboard.WillChangeFrameNotification,
			null);
		NSNotificationCenter.DefaultCenter.AddObserver(
			this,
			new("keyboardHidden:"),
			UIKeyboard.WillHideNotification,
			null);
		NSNotificationCenter.DefaultCenter.AddObserver(
			this,
			new("contentSizeChanged:"),
			UIApplication.ContentSizeCategoryChangedNotification,
			null);

		Live.Add(new(this));
	}

	public PageHost(
		NativeHandle handle) : base(handle)
	{ }


	internal new UITab? Tab { get; set; }


	public ContentView? Page { get; private set; }


	internal void AttachContentDetent() =>
		hasContentDetent = true;

	internal void ContentMeasureInvalidated()
	{
		if (contentDetentPending || !hasContentDetent)
			return;

		contentDetentPending = true;

		CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() =>
		{
			contentDetentPending = false;

			if (NavigationController?.SheetPresentationController is not UISheetPresentationController sheet)
				return;

			if (UIAccessibility.IsReduceMotionEnabled)
				sheet.InvalidateDetents();
			else
				sheet.AnimateChanges(sheet.InvalidateDetents);
		});
	}

	internal double MeasureContent(
		double maximum)
	{
		if (Page is not ContentView page || View is not UIView view)
			return maximum;

		UIEdgeInsets safe = view.SafeAreaInsets;
		double width = view.Bounds.Width;

		if (page.SafeAreaEdges.HasFlag(SafeAreaEdges.Leading))
			width -= safe.Left;
		if (page.SafeAreaEdges.HasFlag(SafeAreaEdges.Trailing))
			width -= safe.Right;

		Size desired = page.HostMeasure(new(Math.Max(0, width), maximum));

		return desired.Height + ChromeHeight(page);
	}

	double ChromeHeight(
		ContentView page)
	{
		if (NavigationController is not UINavigationController navigation)
			return 0;

		double height = 0;

		if (page.SafeAreaEdges.HasFlag(SafeAreaEdges.Top) && !navigation.NavigationBarHidden)
			height += Math.Max(View?.SafeAreaInsets.Top ?? 0, navigation.NavigationBar.Bounds.Height);
		if (page.SafeAreaEdges.HasFlag(SafeAreaEdges.Bottom) && !navigation.ToolbarHidden)
			height += navigation.Toolbar.Bounds.Height;

		return height;
	}


	// ReSharper disable once UnusedMember.Local
	// ReSharper disable once UnusedParameter.Local
	[Export("contentSizeChanged:")]
	void ContentSizeChanged(
		NSNotification notification)
	{
		Page?.InvalidateSubtree();
		View?.SetNeedsLayout();
	}

	// ReSharper disable once UnusedMember.Local
	[Export("keyboardFrameChanged:")]
	void KeyboardFrameChanged(
		NSNotification notification) =>
		ApplyKeyboard(notification, hiding: false);

	// ReSharper disable once UnusedMember.Local
	[Export("keyboardHidden:")]
	void KeyboardHidden(
		NSNotification notification) =>
		ApplyKeyboard(notification, hiding: true);


	void InstallPage()
	{
		if (Page is not ContentView page)
			return;

		ApplyChrome(page);

		UIView native = page.Realize();
		usesSystemScrollInsets = page.ScrollsUnderBars
			&& page.SafeAreaEdges == SafeAreaEdges.All
			&& page.AutomaticScrollBleed is ISystemInsetScroll scrolling
			&& scrolling.UseSystemContentInsets();

		View!.AddSubview(native);

		if (FindScrolling(page)?.Native is UIScrollView scroll)
			SetContentScrollView(scroll, NSDirectionalRectEdge.Top | NSDirectionalRectEdge.Bottom);
	}

	void Reload()
	{
		if (Page is not ContentView old
			|| !IsViewLoaded
			|| SkeleApplication.Current is not SkeleApplication app)
			return;

		ContentView fresh = app.RecreatePage(old);
		if (ReferenceEquals(fresh, old))
			return;

		old.Unrealize();
		old.Host = null;

		Page = fresh;
		fresh.Host = this;

		InstallPage();
		NavigationController?.SetNavigationBarHidden(fresh.HidesNavigationBar, false);
		ApplyLeaveGuard();

		View!.SetNeedsLayout();
	}

	UIBarButtonItem Bar(
		ToolbarItem item)
	{
		if (item.Menu.Count > 0)
			return MenuBar(item);

		UIAction action = UIAction.Create(
			item.Text ?? "",
			item.Icon is string icon ? UIImage.GetSystemImage(icon) : null,
			null,
			_ =>
			{
				if (item.Command is ICommand command && command.CanExecute(item.CommandParameter))
					command.Execute(item.CommandParameter);
			});

		UIBarButtonItem native = new(action)
		{
			Enabled = item.Command?.CanExecute(item.CommandParameter) ?? true
		};

		if (item.IsPrimary)
			native.Style = UIBarButtonItemStyle.Done;

		nativeToolbarItems[native] = item;
		native.TintColor = EffectiveBarTint(Page, item);

		return native;
	}

	static UIColor? EffectiveBarTint(
		ContentView? page,
		ToolbarItem? item = null) =>
		(item?.Tint ?? page?.BarTint ?? SkeleApplication.Current?.Tint)?.ToUIColor();

	UIMenu BuildMenu(
		ToolbarItem item)
	{
		UIAction[] entries = new UIAction[item.Menu.Count];

		for (int index = 0; index < item.Menu.Count; index++)
		{
			MenuAction entry = item.Menu[index];

			entries[index] = UIAction.Create(
				entry.Text,
				entry.Icon is string entryIcon ? UIImage.GetSystemImage(entryIcon) : null,
				null,
				_ =>
				{
					if (entry.Command is ICommand entryCommand && entryCommand.CanExecute(entry.CommandParameter))
						entryCommand.Execute(entry.CommandParameter);
				});

			if (entry.IsDestructive)
				entries[index].Attributes = UIMenuElementAttributes.Destructive;
		}

		menuActions.AddRange(entries);

		return UIMenu.Create(entries);
	}

	UIBarButtonItem MenuBar(
		ToolbarItem item)
	{
		UIMenu menu = BuildMenu(item);

		UIBarButtonItem native = item.Icon is string icon
			? new(UIImage.GetSystemImage(icon), menu)
			: new(item.Text ?? "", menu);

		if (item.IsPrimary)
			native.Style = UIBarButtonItemStyle.Done;

		nativeToolbarItems[native] = item;
		native.TintColor = EffectiveBarTint(Page, item);

		return native;
	}

	void ApplyChrome(
		ContentView page)
	{
		View!.BackgroundColor = page.BackgroundStyle switch
		{
			PageBackground.Grouped => UIColor.SystemGroupedBackground,
			PageBackground.None => UIColor.Clear,
			_ => UIColor.SystemBackground
		};

		NavigationItem.Title = page.Title.Value;
		NavigationItem.Prompt = page.Prompt.Value;
		NavigationItem.BackButtonTitle = page.BackButtonTitle;
		NavigationItem.BackButtonDisplayMode = page.BackButtonStyle switch
		{
			BackButtonStyle.Generic => UINavigationItemBackButtonDisplayMode.Generic,
			BackButtonStyle.Minimal => UINavigationItemBackButtonDisplayMode.Minimal,
			_ => UINavigationItemBackButtonDisplayMode.Default
		};

		NavigationItem.LargeTitleDisplayMode = page.TitleStyle is TitleStyle.Large
			? UINavigationItemLargeTitleDisplayMode.Always
			: UINavigationItemLargeTitleDisplayMode.Never;

		// the stack owns large titles; a page asking for one turns them on for the bar
		if (page.TitleStyle is TitleStyle.Large && NavigationController is UINavigationController stack)
			stack.NavigationBar.PrefersLargeTitles = true;

		ApplyBarAppearance(page);

		ApplyToolbar(page);
		ApplySearch(page);
	}

	void ApplyBarAppearance(
		ContentView page)
	{
		static UINavigationBarAppearance Transparent()
		{
			UINavigationBarAppearance appearance = new();
			appearance.ConfigureWithTransparentBackground();

			return appearance;
		}

		UINavigationBar? bar = NavigationController?.NavigationBar;
		UINavigationBarAppearance standard = bar?.StandardAppearance.Copy() as UINavigationBarAppearance ?? new();
		UINavigationBarAppearance edge = page.ScrollsUnderBars
			? bar?.ScrollEdgeAppearance?.Copy() as UINavigationBarAppearance ?? Transparent()
			: standard.Copy() as UINavigationBarAppearance ?? new();

		UIStringAttributes titleAttributes = new()
		{
			ForegroundColor = page.TitleColor?.ToUIColor() ?? UIColor.Label
		};
		standard.TitleTextAttributes = titleAttributes;
		edge.TitleTextAttributes = titleAttributes;

		UIStringAttributes largeTitleAttributes = new()
		{
			ForegroundColor = page.LargeTitleColor?.ToUIColor() ?? UIColor.Label
		};
		standard.LargeTitleTextAttributes = largeTitleAttributes;
		edge.LargeTitleTextAttributes = largeTitleAttributes;

		NavigationItem.StandardAppearance = standard;
		NavigationItem.ScrollEdgeAppearance = edge;
		NavigationItem.CompactAppearance = standard.Copy() as UINavigationBarAppearance ?? standard;
		NavigationItem.CompactScrollEdgeAppearance = edge.Copy() as UINavigationBarAppearance ?? edge;
	}

	void ApplyNavigationTint(
		ContentView page)
	{
		if (NavigationController?.NavigationBar is UINavigationBar bar)
			bar.TintColor = EffectiveBarTint(page);
	}

	void ApplyKeyboard(
		NSNotification notification,
		bool hiding)
	{
		// a scrolling root avoids the keyboard itself, and doubling up would inset twice
		if (Page is null || !Page.IsRealized || Page.Native.Subviews.FirstOrDefault() is UIScrollView || View?.Window is null)
			return;

		// resolved once here, not on every layout pass during the animation
		keyboardFocus = hiding ? null : FirstResponder(Page.Native);

		nfloat cover = 0;
		if (!hiding)
		{
			CGRect keyboard = UIKeyboard.FrameEndFromNotification(notification);
			CGRect pageInWindow = View.ConvertRectToView(View.Bounds, null);

			// the safe-area bottom is already deducted by Inset, so do not count it twice
			cover = (nfloat)Math.Max(0, pageInWindow.GetMaxY() - keyboard.GetMinY() - View.SafeAreaInsets.Bottom);
		}

		if (cover == keyboardCover)
			return;

		keyboardCover = cover;

		double duration = UIKeyboard.AnimationDurationFromNotification(notification);
		UIView.Animate(duration, () =>
		{
			View.SetNeedsLayout();
			View.LayoutIfNeeded();
		});
	}

	void ObserveToolbar(
		ContentView page)
	{
		foreach (ToolbarItem item in observedItems)
			item.Changed -= OnToolbarItemChanged;

		observedItems.Clear();

		foreach (ToolbarItem item in page.ToolbarItems.Concat(page.BottomToolbarItems))
		{
			item.Changed += OnToolbarItemChanged;
			observedItems.Add(item);
		}
	}

	void OnToolbarItemChanged()
	{
		if (Page is ContentView page)
			ApplyToolbar(page);
	}

	void ApplyToolbar(
		ContentView page)
	{
		// a rebuild replaces every native item, so the old menus' actions can go too
		menuActions.Clear();
		nativeToolbarItems.Clear();
		ObserveToolbar(page);

		List<UIBarButtonItem> leading = [];
		List<UIBarButtonItem> trailing = [];

		foreach (ToolbarItem item in page.ToolbarItems)
		{
			if (!item.IsVisible)
				continue;

			UIBarButtonItem native = Bar(item);

			(item.Side is ToolbarSide.Leading ? leading : trailing).Add(native);
		}

		NavigationItem.LeftBarButtonItems = [.. leading];

		// leading items sit next to Back, they do not replace it
		NavigationItem.LeftItemsSupplementBackButton = true;
		NavigationItem.RightBarButtonItems = [.. trailing];

		if (page.BottomToolbarItems.Count == 0)
		{
			ApplyToolbarTint(page);
			return;
		}

		List<UIBarButtonItem> bottom = [];

		foreach (ToolbarItem item in page.BottomToolbarItems)
		{
			if (!item.IsVisible)
				continue;

			// flexible spaces spread the actions across the bar
			if (bottom.Count > 0)
				bottom.Add(new(UIBarButtonSystemItem.FlexibleSpace));

			bottom.Add(Bar(item));
		}

		SetToolbarItems([.. bottom], false);
		ApplyToolbarTint(page);
	}

	void ApplyToolbarTint(
		ContentView page)
	{
		foreach ((UIBarButtonItem native, ToolbarItem item) in nativeToolbarItems)
			native.TintColor = EffectiveBarTint(page, item);

		if (NavigationController?.Toolbar is not UIToolbar toolbar)
			return;

		toolbar.TintColor = EffectiveBarTint(page);

		toolbar.SetNeedsLayout();
		toolbar.LayoutIfNeeded();
	}

	void ApplySearch(
		ContentView page)
	{
		if (page.SearchPlaceholder is not string placeholder)
			return;

		search = new((UIViewController?)null)
		{
			ObscuresBackgroundDuringPresentation = page.SearchObscuresBackground
		};

		search.SearchBar.Placeholder = placeholder;
		search.SearchBar.Text = page.SearchText.Value;
		search.SearchBar.TextChanged += (_, e) =>
		{
			if (page.HidesSearchScopesWhenEmpty)
				search.SearchBar.SetShowsScopeBar(!string.IsNullOrEmpty(e.SearchText), true);

			page.NotifySearch(e.SearchText);
		};
		search.SearchBar.SearchButtonClicked += (_, _) => page.NotifySearchSubmitted();
		search.SearchBar.CancelButtonClicked += (_, _) =>
		{
			if (page.HidesSearchScopesWhenEmpty)
				search.SearchBar.SetShowsScopeBar(false, true);

			page.NotifySearchCanceled();
		};

		if (page.SearchScopes.Count > 0)
		{
			search.SearchBar.ScopeButtonTitles = [.. page.SearchScopes];
			search.SearchBar.SelectedScopeButtonIndex = page.SearchScopeIndex.Value;
			search.SearchBar.ScopeBarBackgroundImage = TransparentScopeBackground;
			search.SearchBar.SelectedScopeButtonIndexChanged += (_, e) => page.NotifySearchScope((int)e.SelectedScope);

			if (page.HidesSearchScopesWhenEmpty)
			{
				search.SearchBar.ShowsScopeBar = !string.IsNullOrEmpty(page.SearchText.Value);
				search.ScopeBarActivation = UISearchControllerScopeBarActivation.Manual;
			}
		}

		NavigationItem.PreferredSearchBarPlacement = UINavigationItemSearchBarPlacement.Stacked;

		if (OperatingSystem.IsIOSVersionAtLeast(26))
		{
			NavigationItem.SearchBarPlacementAllowsToolbarIntegration = false;
			NavigationItem.SearchBarPlacementAllowsExternalIntegration = false;
		}

		NavigationItem.SearchController = search;
		NavigationItem.PreferredSearchBarPlacement = UINavigationItemSearchBarPlacement.Stacked;
		NavigationItem.HidesSearchBarWhenScrolling = page.HidesSearchBarWhenScrolling;

		DefinesPresentationContext = true;
	}

	internal void ApplySearchText(
		string? value)
	{
		if (search is null)
			return;

		if (search.SearchBar.Text != value)
			search.SearchBar.Text = value;

		if (Page?.HidesSearchScopesWhenEmpty is true)
			search.SearchBar.SetShowsScopeBar(!string.IsNullOrEmpty(value), true);
	}

	internal void ApplySearchScope(
		int value)
	{
		if (search is not null && search.SearchBar.SelectedScopeButtonIndex != value)
			search.SearchBar.SelectedScopeButtonIndex = value;
	}

	void ApplyBackGuard()
	{
		if (Page?.ConfirmLeave is not null
			&& backAction is null
			&& NavigationController is UINavigationController leavable
			&& (leavable.ViewControllers?.Length > 1 || leavable.PresentingViewController is not null))
		{
			backAction = UIAction.Create("", null, null, _ => ConfirmBack());
			NavigationItem.BackAction = backAction;
		}
	}

	void ApplySheetGuard()
	{
		if (NavigationController is not { PresentingViewController: not null } sheet)
			return;

		bool popover = sheet.PresentationController is UIPopoverPresentationController;

		sheet.ModalInPresentation = Page?.ConfirmLeave is not null && !popover;

		if (Page?.ConfirmLeave is not null && sheet.PresentationController is UIPresentationController presentation)
		{
			dismissGuard ??= new(this);
			presentation.Delegate = dismissGuard;
		}
	}

	void ApplyPopGestures()
	{
		if (NavigationController is not UINavigationController stack)
			return;

		bool free = Page?.ConfirmLeave is null;

		if (stack.InteractivePopGestureRecognizer is UIGestureRecognizer swipe)
			swipe.Enabled = free;

		// iOS 26 pops from anywhere in the content, not just the edge
		if (OperatingSystem.IsIOSVersionAtLeast(26) && stack.InteractiveContentPopGestureRecognizer is UIGestureRecognizer contentSwipe)
			contentSwipe.Enabled = free;
	}

	void PreserveNavigationBarAppearance()
	{
		if (SkeleApplication.Current?.IsSwitchingTabs is not true
			|| Page is not ContentView page
			|| FindScrolling(page)?.Native is not UIScrollView scroll
			|| scroll.ContentOffset.Y + scroll.AdjustedContentInset.Top <= 0.5
			|| NavigationController?.NavigationBar is not UINavigationBar bar)
			return;

		savedScrollEdgeAppearance = NavigationItem.ScrollEdgeAppearance;
		savedCompactScrollEdgeAppearance = NavigationItem.CompactScrollEdgeAppearance;
		preservesNavigationBarAppearance = true;

		NavigationItem.ScrollEdgeAppearance = bar.StandardAppearance;
		NavigationItem.CompactScrollEdgeAppearance = bar.CompactAppearance ?? bar.StandardAppearance;
	}

	void RestoreNavigationBarAppearance()
	{
		if (!preservesNavigationBarAppearance)
			return;

		NavigationItem.ScrollEdgeAppearance = savedScrollEdgeAppearance;
		NavigationItem.CompactScrollEdgeAppearance = savedCompactScrollEdgeAppearance;
		savedScrollEdgeAppearance = null;
		savedCompactScrollEdgeAppearance = null;
		preservesNavigationBarAppearance = false;
	}

	// ReSharper disable once AsyncVoidMethod
	async void ConfirmBack()
	{
		if (Page?.ConfirmLeave is Func<Task<bool>> confirm && !await confirm())
			return;

		if (NavigationController is { ViewControllers.Length: > 1 } stack)
			stack.PopViewController(true);
		else
			NavigationController?.DismissViewController(true, null);
	}

	// ReSharper disable once AsyncVoidMethod
	async void ConfirmDismiss()
	{
		if (Page?.ConfirmLeave is Func<Task<bool>> confirm && !await confirm())
			return;

		NavigationController?.DismissViewController(true, null);
	}


	protected override void Dispose(
		bool disposing)
	{
		if (disposing)
		{
			RemoveLive(this);
			menuActions.Clear();
			themeChange?.Dispose();
			themeChange = null;
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);
		}

		base.Dispose(disposing);
	}


	internal void ApplyLeaveGuard()
	{
		if (Page is null || !IsViewLoaded)
			return;

		ApplyBackGuard();
		ApplySheetGuard();
		ApplyPopGestures();
	}


	public override UIStatusBarStyle PreferredStatusBarStyle() =>
		Page?.StatusBar switch
		{
			StatusBarStyle.Light => UIStatusBarStyle.LightContent,
			StatusBarStyle.Dark => UIStatusBarStyle.DarkContent,
			_ => UIStatusBarStyle.Default
		};

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		if (Page is null)
			return;

		InstallPage();

		// numeric keyboards have no return key, so tapping outside is the only way out
		dismissKeyboard = new(() => View!.EndEditing(true))
		{
			CancelsTouchesInView = false
		};
		dismissKeyboard.ShouldReceiveTouch = (_, touch) =>
			FirstResponder(View!) is not UIView focused || !IsWithin(focused, touch.View);
		View!.AddGestureRecognizer(dismissKeyboard);

		themeChange = RegisterForTraitChanges([typeof(UITraitUserInterfaceStyle)], (_, _) => Page?.ReapplyVisuals());
	}

	public override void ViewWillAppear(
		bool animated)
	{
		base.ViewWillAppear(animated);
		RestoreNavigationBarAppearance();

		if (Page is null)
			return;

		// before the transition: a lit row fades out with the pop, not after it
		IUIViewControllerTransitionCoordinator? previous = appearingTransition;
		appearingTransition = animated ? this.GetTransitionCoordinator() : null;

		try
		{
			Page.NotifyAppearing();
		}
		finally
		{
			appearingTransition = previous;
		}

		NavigationController?.SetNavigationBarHidden(Page.HidesNavigationBar, animated);
		ApplyBarAppearance(Page);

		// a bottom toolbar and the floating tab bar share the same edge: the toolbar only shows when
		// the tab bar is gone — a page that wants one sets HidesTabBar
		bool hasToolbar = Page.BottomToolbarItems.Count > 0
			&& (HidesBottomBarWhenPushed || TabBarController is null);

		NavigationController?.SetToolbarHidden(!hasToolbar, animated);

		// hiding the tab bar does not hide its accessory: keep the two in sync
		if (TabBarController is UITabBarController tabs
			&& OperatingSystem.IsIOSVersionAtLeast(26)
			&& SkeleApplication.Current is { Accessory: { } accessory } app)
			tabs.SetBottomAccessory(app.AccessoryWanted && !HidesBottomBarWhenPushed ? accessory : null, animated);

		// bar-wide, so every page restores it before becoming visible
		ApplyNavigationTint(Page);
		ApplyToolbar(Page);

		// here and not ViewDidLoad: whether back has anywhere to go needs the containment settled
		ApplyBackGuard();
		ApplySheetGuard();
	}

	public override void ViewDidAppear(
		bool animated)
	{
		base.ViewDidAppear(animated);

		// after the transition: flipping the gestures mid-pop would kill an in-flight swipe
		ApplyPopGestures();

		SkeleApplication.Current?.CompleteTabSelection(this);
		Page?.NotifyAppeared();

		if (Page is ContentView page)
		{
			ApplyBarAppearance(page);
			ApplyToolbar(page);
			NavigationController?.NavigationBar.SetNeedsLayout();
			NavigationController?.NavigationBar.LayoutIfNeeded();
		}
	}

	public override void ViewWillDisappear(
		bool animated)
	{
		base.ViewWillDisappear(animated);

		PreserveNavigationBarAppearance();
		Page?.NotifyDisappearing();
	}

	public override void ViewSafeAreaInsetsDidChange()
	{
		base.ViewSafeAreaInsetsDidChange();

		View?.SetNeedsLayout();
		View?.LayoutIfNeeded();
	}

	public override void ViewDidLayoutSubviews()
	{
		base.ViewDidLayoutSubviews();

		if (Page is null)
			return;

		UIEdgeInsets safe = View!.SafeAreaInsets;
		Page.PageSafeArea = usesSystemScrollInsets
			? Thickness.Zero
			: new(safe.Left, safe.Top, safe.Right, safe.Bottom);

		// UIKit can only keep a root scroller's adjusted inset synchronized with navigation bars,
		// tab bars and refresh controls when the scroller actually spans the controller.
		CGRect frame = usesSystemScrollInsets
			? View.Bounds
			: Inset(View.Bounds, safe, Page.SafeAreaEdges);
		nfloat chrome = (nfloat)ChromeHeight(Page);

		if (contentWidth != frame.Width || contentChrome != chrome)
		{
			contentWidth = frame.Width;
			contentChrome = chrome;
			ContentMeasureInvalidated();
		}

		CGRect shrunk = new(
			frame.X,
			frame.Y,
			frame.Width,
			(nfloat)Math.Max(0, frame.Height - keyboardCover));

		Page.Native.Frame = shrunk;

		if (keyboardCover <= 0 || keyboardFocus is not UIView focused)
			return;

		// shrinking reflows a layout that can adapt; a top-anchored field just gets clipped, so
		// slide the page up until the focused control clears the keyboard
		Page.Native.LayoutIfNeeded();

		CGRect target = focused.ConvertRectToView(focused.Bounds, View);
		nfloat hidden = target.GetMaxY() + 8 - shrunk.GetMaxY();

		if (hidden > 0)
		{
			Page.Native.Frame = new(
				shrunk.X,
				shrunk.Y - (nfloat)Math.Min(hidden, keyboardCover),
				shrunk.Width,
				shrunk.Height);
		}
	}

	public override void ViewDidDisappear(
		bool animated)
	{
		base.ViewDidDisappear(animated);

		RestoreNavigationBarAppearance();
		Page?.NotifyDisappeared();

		if (IsMovingFromParentViewController)
			Page?.Unrealize();
	}
}
