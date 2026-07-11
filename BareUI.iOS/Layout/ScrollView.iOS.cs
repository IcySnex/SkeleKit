using CoreGraphics;
using ObjCRuntime;
using UIKit;

namespace BareUI;

public partial class ScrollView
{
	private protected override UIView CreateNative() =>
		new ScrollHost(this);

	nfloat keyboardCover;

	private protected override void ApplyProperties()
	{
		UIScrollView host = (UIScrollView)Native;
		bool vertical = Orientation == Orientation.Vertical;

		// only bounce along the axis that scrolls
		host.AlwaysBounceVertical = vertical;
		host.AlwaysBounceHorizontal = !vertical;

		// we own the insets: UIKit's adjustment guesses which edges to inset, and every guess it makes
		// is wrong for us — Always insets across the scroll axis (phantom horizontal scrolling),
		// ScrollableAxes drops the cross-axis inset entirely (content under the notch)
		host.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never;

		ApplyKeyboardDismiss();
		ApplyRefresh(host);
	}

	UIRefreshControl? refresh;

	void ApplyRefresh(
		UIScrollView host)
	{
		if (RefreshCommand is null || refresh is not null)
			return;

		refresh = new();
		refresh.ValueChanged += async (sender, e) =>
		{
			try
			{
				if (RefreshCommand is { } command)
					await command();
			}
			finally
			{
				refresh.EndRefreshing();
			}
		};

		host.RefreshControl = refresh;
	}

	// insets along the scroll axis let the content pass under the bar; the cross axis is handled as
	// layout padding in LayoutContent, because a cross-axis inset is what makes a scroll view drift
	void ApplyContentInsets()
	{
		UIScrollView host = (UIScrollView)Native;
		Thickness bled = BledInsets;
		bool vertical = Orientation == Orientation.Vertical;

		UIEdgeInsets insets = new(
			vertical ? (nfloat)bled.Top : 0,
			vertical ? 0 : (nfloat)bled.Left,
			(vertical ? (nfloat)bled.Bottom : 0) + keyboardCover,
			vertical ? 0 : (nfloat)bled.Right);

		if (host.ContentInset != insets)
		{
			host.ContentInset = insets;
			host.VerticalScrollIndicatorInsets = insets;
			host.HorizontalScrollIndicatorInsets = insets;
		}
	}

	partial void ApplyKeyboardDismissCore() =>
		((UIScrollView)Native).KeyboardDismissMode = keyboardDismiss switch
		{
			KeyboardDismiss.OnDrag => UIScrollViewKeyboardDismissMode.OnDrag,
			KeyboardDismiss.Interactive => UIScrollViewKeyboardDismissMode.Interactive,
			_ => UIScrollViewKeyboardDismissMode.None
		};

	partial void ArrangeContent(
		Size viewport) =>
		LayoutContent(viewport);

	// lay out content, report scrollable size
	internal void LayoutContent(
		Size viewport)
	{
		UIScrollView host = (UIScrollView)Native;

		View? content = Content;
		if (content is null)
		{
			host.ContentSize = CGSize.Empty;
			return;
		}

		ApplyContentInsets();

		bool vertical = Orientation == Orientation.Vertical;
		Thickness bled = BledInsets;

		// across the scroll axis the bleed becomes padding, so content never sits under the notch
		double padLeft = vertical ? bled.Left : 0;
		double padRight = vertical ? bled.Right : 0;
		double padTop = vertical ? 0 : bled.Top;
		double padBottom = vertical ? 0 : bled.Bottom;

		viewport = new(
			Math.Max(0, viewport.Width - padLeft - padRight),
			Math.Max(0, viewport.Height - padTop - padBottom));

		Size probe = vertical
			? new(viewport.Width, double.PositiveInfinity)
			: new(double.PositiveInfinity, viewport.Height);

		content.Measure(probe);
		Size desired = content.DesiredSize;

		double width = vertical ? viewport.Width : desired.Width;
		double height = vertical ? desired.Height : viewport.Height;

		content.Arrange(new(padLeft, padTop, width, height));
		host.ContentSize = new CGSize(width + padLeft + padRight, height + padTop + padBottom);
	}

	// UIKit never moves anything for the keyboard: inset the content by however much it covers,
	// then scroll the focused control back into view
	internal void OnKeyboardChanged(
		NSNotification notification,
		bool hiding)
	{
		if (!AvoidsKeyboard || !IsRealized)
			return;

		UIScrollView host = (UIScrollView)Native;
		if (host.Window is null)
			return;

		// the keyboard may belong to a field in some other scroll view
		UIView? focused = FirstResponder(host);
		if (!hiding && focused is null)
			return;

		nfloat covered = 0;

		if (!hiding)
		{
			CGRect keyboard = UIKeyboard.FrameEndFromNotification(notification);
			CGRect hostInWindow = host.ConvertRectToView(host.Bounds, null);

			// the safe-area bottom is already in the adjusted inset, so do not count it twice
			covered = (nfloat)Math.Max(
				0,
				hostInWindow.GetMaxY() - keyboard.GetMinY() - host.SafeAreaInsets.Bottom);
		}

		keyboardCover = covered;

		double duration = UIKeyboard.AnimationDurationFromNotification(notification);

		// composed with the safe-area insets, never replacing them
		UIView.Animate(duration, ApplyContentInsets);

		if (hiding || focused is null)
			return;

		CGRect target = focused.ConvertRectToView(focused.Bounds, host);
		host.ScrollRectToVisible(target.Inset(0, -8), true);
	}

	internal void OnScrolled(
		double offset) =>
		Scrolled?.Invoke(offset);

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
}

/// <summary>
/// The native <c>UIScrollView</c> that hosts a <see cref="ScrollView"/> and drives its content layout.
/// </summary>
sealed class ScrollHost : UIScrollView
{
	readonly ScrollView? element;

	public ScrollHost(
		ScrollView element)
	{
		this.element = element;

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
	}

	// see LayoutHost
	public ScrollHost(
		NativeHandle handle) : base(handle)
	{ }

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		element?.LayoutContent(new(Bounds.Width, Bounds.Height));
	}

	public override CGPoint ContentOffset
	{
		get => base.ContentOffset;
		set
		{
			base.ContentOffset = value;

			element?.OnScrolled(value.Y + AdjustedContentInset.Top);
		}
	}

	[Export("keyboardFrameChanged:")]
	void KeyboardFrameChanged(
		NSNotification notification) =>
		element?.OnKeyboardChanged(notification, hiding: false);

	[Export("keyboardHidden:")]
	void KeyboardHidden(
		NSNotification notification) =>
		element?.OnKeyboardChanged(notification, hiding: true);

	protected override void Dispose(
		bool disposing)
	{
		if (disposing)
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);

		base.Dispose(disposing);
	}
}
