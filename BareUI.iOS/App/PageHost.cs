using Foundation;
using CoreGraphics;
using ObjCRuntime;
using UIKit;

namespace BareUI;

/// <summary>
/// The hidden <c>UIViewController</c> that hosts one <see cref="ContentView"/>. App code never sees it.
/// </summary>
sealed class PageHost : UIViewController
{
	readonly ContentView? page;

	UITapGestureRecognizer? dismissKeyboard;
	UIView? keyboardFocus;
	nfloat keyboardCover;

	public PageHost(
		ContentView page)
	{
		this.page = page;
		page.Host = this;

		// selector-based, not block-based: a block observer's dispatcher peer can be collected
		NSNotificationCenter.DefaultCenter.AddObserver(
			this,
			new Selector("keyboardFrameChanged:"),
			UIKeyboard.WillChangeFrameNotification,
			null);

		NSNotificationCenter.DefaultCenter.AddObserver(
			this,
			new Selector("keyboardHidden:"),
			UIKeyboard.WillHideNotification,
			null);

		// dynamic type resizes the fonts under us, so every cached measurement in the tree is wrong
		NSNotificationCenter.DefaultCenter.AddObserver(
			this,
			new Selector("contentSizeChanged:"),
			UIApplication.ContentSizeCategoryChangedNotification,
			null);
	}

	[Export("contentSizeChanged:")]
	void ContentSizeChanged(
		NSNotification notification)
	{
		page?.InvalidateSubtree();
		View?.SetNeedsLayout();
	}

	// marshaller needs this; Navigator keeps the managed ref so it stays unused
	public PageHost(
		NativeHandle handle) : base(handle)
	{ }

	/// <summary>
	/// The page this controller hosts.
	/// </summary>
	public ContentView? Page =>
		page;

	// UIViewController.Title also rewrites the tab bar item, which would clobber the tab's own label
	public void SetTitle(
		string? title) =>
		NavigationItem.Title = title;

	// a scrolling root fills the bounds so UIKit can blur the bars behind it, and insets the content itself
	UIScrollView? ScrollRoot =>
		page?.IsRealized is true ? page.Native.Subviews.FirstOrDefault() as UIScrollView : null;

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		if (page is null)
			return;

		ApplyChrome(page);

		View!.AddSubview(page.Realize());

		// numeric keyboards have no return key, so tapping outside is the only way out
		dismissKeyboard = new(() => View.EndEditing(true))
		{
			CancelsTouchesInView = false
		};
		View.AddGestureRecognizer(dismissKeyboard);

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

		SetTitle(page.Title.Value);

		NavigationItem.LargeTitleDisplayMode = page.TitleStyle is TitleStyle.Large
			? UINavigationItemLargeTitleDisplayMode.Always
			: UINavigationItemLargeTitleDisplayMode.Never;

		// the stack owns large titles; a page asking for one turns them on for the bar
		if (page.TitleStyle is TitleStyle.Large && NavigationController is { } stack)
			stack.NavigationBar.PrefersLargeTitles = true;

		ApplyToolbar(page);
		ApplySearch(page);
	}

	UISearchController? search;

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
		search.SearchBar.TextChanged += (sender, e) => page.NotifySearch(e.SearchText ?? "");

		NavigationItem.SearchController = search;
		NavigationItem.HidesSearchBarWhenScrolling = false;

		// the search controller is retained natively only
		DefinesPresentationContext = true;
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

	public override void ViewWillAppear(
		bool animated)
	{
		base.ViewWillAppear(animated);

		if (page is not null)
			NavigationController?.SetNavigationBarHidden(page.HidesNavigationBar, animated);
	}

	public override void ViewDidLayoutSubviews()
	{
		base.ViewDidLayoutSubviews();

		if (page is null)
			return;

		UIEdgeInsets safe = View!.SafeAreaInsets;
		page.PageSafeArea = new(safe.Left, safe.Top, safe.Right, safe.Bottom);

		// one regime: the page always sits inside the safe area. A view that wants to escape it says
		// so with IgnoresSafeArea and grows back out — nothing depends on what the root happens to be.
		CGRect frame = Inset(View.Bounds, safe, page.SafeAreaEdges);
		CGRect shrunk = new(
			frame.X,
			frame.Y,
			frame.Width,
			(nfloat)Math.Max(0, frame.Height - keyboardCover));

		page.Native.Frame = shrunk;

		if (keyboardCover <= 0 || keyboardFocus is not { } focused)
			return;

		// shrinking reflows a layout that can adapt; a top-anchored field just gets clipped, so
		// slide the page up until the focused control clears the keyboard
		page.Native.LayoutIfNeeded();

		CGRect target = focused.ConvertRectToView(focused.Bounds, View);
		nfloat hidden = target.GetMaxY() + 8 - shrunk.GetMaxY();

		if (hidden > 0)
			page.Native.Frame = new(
				shrunk.X,
				shrunk.Y - (nfloat)Math.Min(hidden, keyboardCover),
				shrunk.Width,
				shrunk.Height);
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

	// SwiftUI treats the keyboard as a shrink of the safe area, so any layout adapts — not just scrolling ones
	[Export("keyboardFrameChanged:")]
	void KeyboardFrameChanged(
		NSNotification notification) =>
		ApplyKeyboard(notification, hiding: false);

	[Export("keyboardHidden:")]
	void KeyboardHidden(
		NSNotification notification) =>
		ApplyKeyboard(notification, hiding: true);

	void ApplyKeyboard(
		NSNotification notification,
		bool hiding)
	{
		// the scroll view handles its own case, and doubling up would inset twice
		if (page is null || ScrollRoot is not null || View?.Window is null)
			return;

		// resolved once here, not on every layout pass during the animation
		keyboardFocus = hiding ? null : FirstResponder(page.Native);

		nfloat cover = 0;

		if (!hiding)
		{
			CGRect keyboard = UIKeyboard.FrameEndFromNotification(notification);
			CGRect pageInWindow = View.ConvertRectToView(View.Bounds, null);

			// the safe-area bottom is already deducted by Inset, so do not count it twice
			cover = (nfloat)Math.Max(
				0,
				pageInWindow.GetMaxY() - keyboard.GetMinY() - View.SafeAreaInsets.Bottom);
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

	public override void ViewDidAppear(
		bool animated)
	{
		base.ViewDidAppear(animated);

		page?.NotifyAppearing();
	}

	public override void ViewDidDisappear(
		bool animated)
	{
		base.ViewDidDisappear(animated);

		page?.NotifyDisappearing();

		// popped for good, not just covered
		if (IsMovingFromParentViewController)
			page?.Unrealize();
	}

	protected override void Dispose(
		bool disposing)
	{
		if (disposing)
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);

		base.Dispose(disposing);
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
}
