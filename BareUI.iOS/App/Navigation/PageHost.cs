using System.Runtime.Versioning;
using ObjCRuntime;

namespace BareUI;

internal sealed class PageHost : UIViewController
{
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

	UIBarButtonItem Bar(
		ToolbarItem item)
	{
		if (item.Menu.Count > 0)
			return MenuBar(item);

		UIAction action = UIAction.Create(
			item.Text ?? "",
			item.Icon is { } icon ? UIImage.GetSystemImage(icon) : null,
			null,
			_ =>
			{
				if (item.Command is { } command && command.CanExecute(item.CommandParameter))
					command.Execute(item.CommandParameter);
			});

		UIBarButtonItem native = new(action)
		{
			Enabled = item.Command?.CanExecute(item.CommandParameter) ?? true
		};

		if (item.IsPrimary)
			native.Style = UIBarButtonItemStyle.Done;

		// item-level tint: iOS 26 glass buttons do not always follow the bar's TintColor
		if (Page?.BarAccent is { } accent)
			native.TintColor = accent.ToUIColor();

		return native;
	}

	// the actions stay rooted here: UIKit's retain alone would let their managed peers die
	readonly List<UIAction> menuActions = [];

	UIMenu BuildMenu(
		ToolbarItem item)
	{
		UIAction[] entries = new UIAction[item.Menu.Count];

		for (int index = 0; index < item.Menu.Count; index++)
		{
			MenuAction entry = item.Menu[index];

			entries[index] = UIAction.Create(
				entry.Text,
				entry.Icon is { } entryIcon ? UIImage.GetSystemImage(entryIcon) : null,
				null,
				_ =>
				{
					if (entry.Command is { } entryCommand && entryCommand.CanExecute(null))
						entryCommand.Execute(null);
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

		UIBarButtonItem native = item.Icon is { } icon
			? new(UIImage.GetSystemImage(icon), menu)
			: new(item.Text ?? "", menu);

		if (item.IsPrimary)
			native.Style = UIBarButtonItemStyle.Done;

		if (Page?.BarAccent is { } accent)
			native.TintColor = accent.ToUIColor();

		return native;
	}

	static UIView? FirstResponder(
		UIView view)
	{
		if (view.IsFirstResponder)
			return view;

		foreach (UIView child in view.Subviews)
			if (FirstResponder(child) is { } found)
				return found;

		return null;
	}


	UITapGestureRecognizer? dismissKeyboard;
	UIView? keyboardFocus;
	nfloat keyboardCover;
	IUITraitChangeRegistration? themeChange;
	UISearchController? search;
	UIAction? backAction;
	SheetGuard? dismissGuard;
	UITabAccessory? bottomAccessory;

	public PageHost(
		ContentView page)
	{
		this.Page = page;
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
	}

	public PageHost(
		NativeHandle handle) : base(handle)
	{ }


	public ContentView? Page { get; }

	public override UIStatusBarStyle PreferredStatusBarStyle() =>
		Page?.StatusBar switch
		{
			StatusBarStyle.Light => UIStatusBarStyle.LightContent,
			StatusBarStyle.Dark => UIStatusBarStyle.DarkContent,
			_ => UIStatusBarStyle.Default
		};


	[Export("contentSizeChanged:")]
	void ContentSizeChanged(
		NSNotification notification)
	{
		Page?.InvalidateSubtree();
		View?.SetNeedsLayout();
	}

	[Export("keyboardFrameChanged:")]
	void KeyboardFrameChanged(
		NSNotification notification) =>
		ApplyKeyboard(notification, hiding: false);

	[Export("keyboardHidden:")]
	void KeyboardHidden(
		NSNotification notification) =>
		ApplyKeyboard(notification, hiding: true);


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
		NavigationItem.Prompt = page.Prompt;
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
		if (page.TitleStyle is TitleStyle.Large && NavigationController is { } stack)
			stack.NavigationBar.PrefersLargeTitles = true;

		if (page.TitleColor is not null || page.LargeTitleColor is not null)
			ApplyBarAppearance(page);

		ApplyToolbar(page);
		ApplySearch(page);
	}

	// per-item appearances, so the colors leave every other page's bar alone
	void ApplyBarAppearance(
		ContentView page)
	{
		static UINavigationBarAppearance Transparent()
		{
			UINavigationBarAppearance appearance = new();
			appearance.ConfigureWithTransparentBackground();

			return appearance;
		}

		// start from the bar's live appearances: rebuilding from scratch loses the system look
		// (and its collapse transition) just to recolor a title
		UINavigationBar? bar = NavigationController?.NavigationBar;
		UINavigationBarAppearance standard = bar?.StandardAppearance.Copy() as UINavigationBarAppearance ?? new();

		// no scroll-edge appearance means transparent-at-edge; the override must keep that
		UINavigationBarAppearance edge = bar?.ScrollEdgeAppearance?.Copy() as UINavigationBarAppearance ?? Transparent();

		if (page.TitleColor is { } title)
		{
			UIStringAttributes attributes = new() { ForegroundColor = title.ToUIColor() };
			standard.TitleTextAttributes = attributes;
			edge.TitleTextAttributes = attributes;
		}

		if (page.LargeTitleColor is { } large)
		{
			UIStringAttributes attributes = new() { ForegroundColor = large.ToUIColor() };
			standard.LargeTitleTextAttributes = attributes;
			edge.LargeTitleTextAttributes = attributes;
		}

		NavigationItem.StandardAppearance = standard;
		NavigationItem.ScrollEdgeAppearance = edge;
		NavigationItem.CompactAppearance = standard;
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

	void ApplyToolbar(
		ContentView page)
	{
		List<UIBarButtonItem> leading = [];
		List<UIBarButtonItem> trailing = [];

		foreach (ToolbarItem item in page.ToolbarItems)
		{
			UIBarButtonItem native = Bar(item);

			(item.Side is ToolbarSide.Leading ? leading : trailing).Add(native);
		}

		NavigationItem.LeftBarButtonItems = [.. leading];

		// leading items sit next to Back, they do not replace it
		NavigationItem.LeftItemsSupplementBackButton = true;
		NavigationItem.RightBarButtonItems = [.. trailing];

		if (page.BottomToolbarItems.Count == 0)
			return;

		List<UIBarButtonItem> bottom = [];

		foreach (ToolbarItem item in page.BottomToolbarItems)
		{
			// flexible spaces spread the actions across the bar
			if (bottom.Count > 0)
				bottom.Add(new(UIBarButtonSystemItem.FlexibleSpace));

			bottom.Add(Bar(item));
		}

		SetToolbarItems([.. bottom], false);
	}

	void ApplySearch(
		ContentView page)
	{
		if (page.SearchPlaceholder is not { } placeholder)
			return;

		search = new((UIViewController?)null)
		{
			ObscuresBackgroundDuringPresentation = page.SearchObscuresBackground
		};

		search.SearchBar.Placeholder = placeholder;
		search.SearchBar.TextChanged += (_, e) => page.NotifySearch(e.SearchText);
		search.SearchBar.CancelButtonClicked += (_, _) => page.NotifySearchCancelled();

		if (page.SearchScopes.Count > 0)
		{
			search.SearchBar.ScopeButtonTitles = [.. page.SearchScopes];
			search.SearchBar.SelectedScopeButtonIndexChanged += (_, e) => page.NotifySearchScope((int)e.SelectedScope);
		}

		// rooted: SearchResultsUpdater is weak
		searchUpdater = new(this);
		search.SearchResultsUpdater = searchUpdater;
		ApplySearchSuggestions(page);

		NavigationItem.SearchController = search;
		NavigationItem.HidesSearchBarWhenScrolling = page.HidesSearchBarWhenScrolling;

		DefinesPresentationContext = true;
	}

	SearchUpdater? searchUpdater;
	SearchSuggestion[]? suggestionModels;

	// rooted: the controller's retain alone would let the peers die
	UISearchSuggestionItem[]? suggestionItems;

	internal void ApplySearchSuggestions(
		ContentView page)
	{
		if (search is null)
			return;

		suggestionModels = [.. page.SearchSuggestions];
		suggestionItems = new UISearchSuggestionItem[suggestionModels.Length];

		for (int index = 0; index < suggestionModels.Length; index++)
		{
			SearchSuggestion suggestion = suggestionModels[index];

			suggestionItems[index] = new(
				(NSString)suggestion.Text,
				suggestion.Description,
				suggestion.Icon is { } icon ? UIImage.GetSystemImage(icon) : null)
			{
				RepresentedObject = NSNumber.FromInt32(index)
			};
		}

		search.SearchSuggestions = suggestionItems;
	}

	internal void OnSuggestionSelected(
		IUISearchSuggestion suggestion)
	{
		if (suggestion is not UISearchSuggestionItem { RepresentedObject: NSNumber number }
			|| suggestionModels is not { } models
			|| number.Int32Value >= models.Length)
			return;

		SearchSuggestion model = models[number.Int32Value];

		if (Page?.SearchSuggestionCommand is { } command && command.CanExecute(model))
			command.Execute(model);
	}


	protected override void Dispose(
		bool disposing)
	{
		if (disposing)
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);

		base.Dispose(disposing);
	}


	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		if (Page is null)
			return;

		ApplyChrome(Page);

		View!.AddSubview(Page.Realize());

		// we own the insets, so UIKit cannot find the scroll view on its own: telling it which one
		// drives the bar is what collapses a large title (and blurs the bar edge) on scroll
		if (Page.Native.Subviews.FirstOrDefault() is UIScrollView scroll)
			SetContentScrollView(scroll, NSDirectionalRectEdge.Top);

		// numeric keyboards have no return key, so tapping outside is the only way out
		dismissKeyboard = new(() => View.EndEditing(true))
		{
			CancelsTouchesInView = false
		};
		View.AddGestureRecognizer(dismissKeyboard);

		themeChange = RegisterForTraitChanges([typeof(UITraitUserInterfaceStyle)], (_, _) => Page?.ReapplyVisuals());
	}

	public override void ViewWillAppear(
		bool animated)
	{
		base.ViewWillAppear(animated);

		if (Page is null)
			return;

		// before the transition: a lit row fades out with the pop, not after it
		Page.NotifyWillAppear();

		NavigationController?.SetNavigationBarHidden(Page.HidesNavigationBar, animated);

		// with a visible tab bar the items float above it as the tab accessory (the two bars share
		// the bottom edge); everywhere else they get the classic navigation toolbar
		bool floats = Page.BottomToolbarItems.Count > 0
			&& !HidesBottomBarWhenPushed
			&& TabBarController is not null
			&& OperatingSystem.IsIOSVersionAtLeast(26);

		NavigationController?.SetToolbarHidden(Page.BottomToolbarItems.Count == 0 || floats, animated);

		if (TabBarController is { } tabs && OperatingSystem.IsIOSVersionAtLeast(26))
			tabs.SetBottomAccessory(floats ? bottomAccessory ??= BuildAccessory(Page) : null, animated);

		// bar-wide, so every page restores it; null falls back to the app accent
		NavigationController?.NavigationBar.TintColor = Page.BarAccent?.ToUIColor();
		NavigationController?.Toolbar.TintColor = Page.BarAccent?.ToUIColor();

		// here and not ViewDidLoad: whether back has anywhere to go needs the containment settled.
		// a pushed page's natural back keeps its look; a modal root synthesizes one that dismisses
		if (Page.ConfirmLeave is not null
			&& backAction is null
			&& NavigationController is { } leavable
			&& (leavable.ViewControllers?.Length > 1 || leavable.PresentingViewController is not null))
		{
			backAction = UIAction.Create("", null, null, _ => ConfirmBack());
			NavigationItem.BackAction = backAction;
		}

		// a guarded page pins its sheet; DidAttemptToDismiss then routes the swipe into the confirm
		if (NavigationController is { PresentingViewController: not null } sheet)
		{
			sheet.ModalInPresentation = Page.ConfirmLeave is not null;

			if (Page.ConfirmLeave is not null && sheet.PresentationController is { } presentation)
			{
				dismissGuard ??= new(this);
				presentation.Delegate = dismissGuard;
			}
		}
	}

	public override void ViewDidAppear(
		bool animated)
	{
		base.ViewDidAppear(animated);

		// after the transition: flipping it mid-pop would kill an in-flight swipe
		if (NavigationController is { } stack)
		{
			bool free = Page?.ConfirmLeave is null;

			if (stack.InteractivePopGestureRecognizer is { } swipe)
				swipe.Enabled = free;

			// iOS 26 pops from anywhere in the content, not just the edge
			if (OperatingSystem.IsIOSVersionAtLeast(26) && stack.InteractiveContentPopGestureRecognizer is { } contentSwipe)
				contentSwipe.Enabled = free;
		}

		Page?.NotifyAppearing();
	}

	// manual frames do not follow safe-area guides: without this, a chrome change that only moves
	// the insets (search bar activation) leaves the page parked at its old frame. Laying out
	// immediately keeps the reframe inside UIKit's own chrome animation instead of jumping after it
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
		Page.PageSafeArea = new(safe.Left, safe.Top, safe.Right, safe.Bottom);

		// one regime: the page always sits inside the safe area. A view that wants to escape it says
		// so with IgnoresSafeArea and grows back out — nothing depends on what the root happens to be.
		CGRect frame = Inset(View.Bounds, safe, Page.SafeAreaEdges);
		CGRect shrunk = new(
			frame.X,
			frame.Y,
			frame.Width,
			(nfloat)Math.Max(0, frame.Height - keyboardCover));

		Page.Native.Frame = shrunk;

		if (keyboardCover <= 0 || keyboardFocus is not { } focused)
			return;

		// shrinking reflows a layout that can adapt; a top-anchored field just gets clipped, so
		// slide the page up until the focused control clears the keyboard
		Page.Native.LayoutIfNeeded();

		CGRect target = focused.ConvertRectToView(focused.Bounds, View);
		nfloat hidden = target.GetMaxY() + 8 - shrunk.GetMaxY();

		if (hidden > 0)
			Page.Native.Frame = new(
				shrunk.X,
				shrunk.Y - (nfloat)Math.Min(hidden, keyboardCover),
				shrunk.Width,
				shrunk.Height);
	}

	public override void ViewDidDisappear(
		bool animated)
	{
		base.ViewDidDisappear(animated);

		Page?.NotifyDisappearing();

		if (IsMovingFromParentViewController)
			Page?.Unrealize();
	}


	// on a modal root there is nothing to pop: the synthesized back button leaves by dismissing
	async void ConfirmBack()
	{
		if (Page?.ConfirmLeave is not { } confirm)
			return;

		if (!await confirm())
			return;

		if (NavigationController is { ViewControllers.Length: > 1 } stack)
			stack.PopViewController(true);
		else
			NavigationController?.DismissViewController(true, null);
	}

	async void ConfirmDismiss()
	{
		if (Page?.ConfirmLeave is not { } confirm)
			return;

		if (await confirm())
			NavigationController?.DismissViewController(true, null);
	}

	[SupportedOSPlatform("ios26.0")]
	UITabAccessory BuildAccessory(
		ContentView page)
	{
		UIStackView stack = new()
		{
			Axis = UILayoutConstraintAxis.Horizontal,
			Distribution = UIStackViewDistribution.EqualSpacing,
			TranslatesAutoresizingMaskIntoConstraints = false
		};

		foreach (ToolbarItem item in page.BottomToolbarItems)
			stack.AddArrangedSubview(AccessoryButton(item, page));

		UIView content = new();
		content.AddSubview(stack);
		NSLayoutConstraint.ActivateConstraints(
		[
			stack.LeadingAnchor.ConstraintEqualTo(content.LeadingAnchor, 24),
			stack.TrailingAnchor.ConstraintEqualTo(content.TrailingAnchor, -24),
			stack.CenterYAnchor.ConstraintEqualTo(content.CenterYAnchor)
		]);

		return new(content);
	}

	UIButton AccessoryButton(
		ToolbarItem item,
		ContentView page)
	{
		UIButtonConfiguration configuration = UIButtonConfiguration.PlainButtonConfiguration;
		configuration.Title = item.Text;

		if (item.Icon is { } icon)
			configuration.Image = UIImage.GetSystemImage(icon);

		// a configuration paints from its own foreground color, not the view tint
		if (page.BarAccent is { } foreground)
			configuration.BaseForegroundColor = foreground.ToUIColor();

		UIButton button = new() { Configuration = configuration };

		if (item.Menu.Count > 0)
		{
			button.Menu = BuildMenu(item);
			button.ShowsMenuAsPrimaryAction = true;
		}
		else
		{
			UIAction action = UIAction.Create(
				"",
				null,
				null,
				_ =>
				{
					if (item.Command is { } command && command.CanExecute(item.CommandParameter))
						command.Execute(item.CommandParameter);
				});

			menuActions.Add(action);
			button.AddAction(action, UIControlEvent.TouchUpInside);
			button.Enabled = item.Command?.CanExecute(item.CommandParameter) ?? true;
		}

		if (page.BarAccent is { } accent)
			button.TintColor = accent.ToUIColor();

		return button;
	}

	// the presentation controller holds its delegate weakly; the dismissGuard field roots this
	sealed class SheetGuard : UIAdaptivePresentationControllerDelegate
	{
		readonly PageHost? host;

		public SheetGuard(
			PageHost host)
		{
			this.host = host;
		}

		public SheetGuard(
			NativeHandle handle) : base(handle)
		{ }


		public override void DidAttemptToDismiss(
			UIPresentationController presentationController) =>
			host?.ConfirmDismiss();
	}

	sealed class SearchUpdater : UISearchResultsUpdating
	{
		readonly PageHost? host;

		public SearchUpdater(
			PageHost host)
		{
			this.host = host;
		}

		public SearchUpdater(
			NativeHandle handle) : base(handle)
		{ }


		// typing is already covered by the search bar's TextChanged
		public override void UpdateSearchResultsForSearchController(
			UISearchController searchController)
		{ }

		public override void UpdateSearchResults(
			UISearchController searchController,
			IUISearchSuggestion suggestion) =>
			host?.OnSuggestionSelected(suggestion);
	}
}
