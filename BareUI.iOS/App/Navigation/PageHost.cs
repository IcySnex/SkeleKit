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

	static UIBarButtonItem Bar(
		ToolbarItem item)
	{
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

	public PageHost(
		ContentView page)
	{
		this.Page = page;
		page.Host = this;

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

		NavigationItem.LargeTitleDisplayMode = page.TitleStyle is TitleStyle.Large
			? UINavigationItemLargeTitleDisplayMode.Always
			: UINavigationItemLargeTitleDisplayMode.Never;

		// the stack owns large titles; a page asking for one turns them on for the bar
		if (page.TitleStyle is TitleStyle.Large && NavigationController is { } stack)
			stack.NavigationBar.PrefersLargeTitles = true;

		ApplyToolbar(page);
		ApplySearch(page);
	}

	void ApplyKeyboard(
		NSNotification notification,
		bool hiding)
	{
		if (Page is null || !Page.IsRealized || Page.Native.Subviews.FirstOrDefault() is not null || View?.Window is null)
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
		NavigationItem.RightBarButtonItems = [.. trailing];
	}

	void ApplySearch(
		ContentView page)
	{
		if (page.SearchPlaceholder is not { } placeholder)
			return;

		search = new((UIViewController?)null)
		{
			ObscuresBackgroundDuringPresentation = false
		};

		search.SearchBar.Placeholder = placeholder;
		search.SearchBar.TextChanged += (_, e) => page.NotifySearch(e.SearchText);

		NavigationItem.SearchController = search;
		NavigationItem.HidesSearchBarWhenScrolling = false;

		DefinesPresentationContext = true;
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

		if (Page is not null)
			NavigationController?.SetNavigationBarHidden(Page.HidesNavigationBar, animated);
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

	public override void ViewDidAppear(
		bool animated)
	{
		base.ViewDidAppear(animated);

		Page?.NotifyAppearing();
	}

	public override void ViewDidDisappear(
		bool animated)
	{
		base.ViewDidDisappear(animated);

		Page?.NotifyDisappearing();

		if (IsMovingFromParentViewController)
			Page?.Unrealize();
	}
}
