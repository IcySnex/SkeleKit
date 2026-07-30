using ObjCRuntime;

namespace SkeleKit;

public partial class ScrollView
{
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


	nfloat keyboardCover;
	UIRefreshControl? refresh;
	bool endsAfterDrag;


	void ApplyRefresh(
		UIScrollView host)
	{
		if (RefreshCommand is null || refresh is not null)
			return;

		refresh = new();
		refresh.ValueChanged += (_, _) => OnRefreshTriggered();

		host.RefreshControl = refresh;
	}

	void EndNativeRefresh()
	{
		refresh?.EndRefreshing();
		ApplyContentInsets();
	}

	void ApplyContentInsets()
	{
		UIScrollView host = (UIScrollView)Native;

		// while refreshing, UIKit holds the spinner open through the top inset; writing ours over
		// it collapses the spinner mid-spin
		if (refresh is { Refreshing: true })
			return;

		Thickness bled = BledInsets;
		bool vertical = Orientation == Orientation.Vertical;

		UIEdgeInsets insets = new(
			vertical ? (nfloat)bled.Top : 0,
			vertical ? 0 : (nfloat)bled.Left,
			(vertical ? (nfloat)bled.Bottom : 0) + keyboardCover,
			vertical ? 0 : (nfloat)bled.Right);

		if (host.ContentInset == insets)
			return;

		// we own the insets, so UIKit will not re-anchor the offset when chrome resizes them
		// (search bar activation, bar collapse): a scroll resting at the top must stay at the
		// top, or the stale offset opens a gap and pins the linked bar collapsed
		bool atTop = host.ContentOffset.Y <= -host.ContentInset.Top + 1;

		host.ContentInset = insets;
		host.VerticalScrollIndicatorInsets = insets;
		host.HorizontalScrollIndicatorInsets = insets;

		if (atTop && Orientation == Orientation.Vertical)
			host.ContentOffset = new(host.ContentOffset.X, -insets.Top);
	}

	partial void ApplyRefreshingCore()
	{
		if (refresh is null)
			return;

		UIScrollView host = (UIScrollView)Native;

		if (IsRefreshing.Value)
		{
			if (!refresh.Refreshing)
				refresh.BeginRefreshing();

			return;
		}

		// finishing under a held finger yanks the inset mid-drag: wait for the release
		if (host.Dragging)
		{
			endsAfterDrag = true;
			return;
		}

		EndNativeRefresh();
	}

	partial void ApplyKeyboardDismissCore() =>
		((UIScrollView)Native).KeyboardDismissMode = keyboardDismiss switch
		{
			KeyboardDismiss.OnDrag => UIScrollViewKeyboardDismissMode.OnDrag,
			KeyboardDismiss.Interactive => UIScrollViewKeyboardDismissMode.Interactive,
			_ => UIScrollViewKeyboardDismissMode.None
		};

	partial void ApplyBehaviorCore()
	{
		UIScrollView host = (UIScrollView)Native;
		bool vertical = Orientation == Orientation.Vertical;

		host.PagingEnabled = Paging;
		host.ShowsVerticalScrollIndicator = ShowsIndicator && vertical;
		host.ShowsHorizontalScrollIndicator = ShowsIndicator && !vertical;

		host.IndicatorStyle = IndicatorStyle switch
		{
			IndicatorStyle.Dark => UIScrollViewIndicatorStyle.Black,
			IndicatorStyle.Light => UIScrollViewIndicatorStyle.White,
			_ => UIScrollViewIndicatorStyle.Default
		};

		host.AutomaticallyAdjustsScrollIndicatorInsets = IndicatorInsets is null;

		if (IndicatorInsets is Thickness insets)
		{
			UIEdgeInsets native = new((nfloat)insets.Top, (nfloat)insets.Left, (nfloat)insets.Bottom, (nfloat)insets.Right);

			host.VerticalScrollIndicatorInsets = native;
			host.HorizontalScrollIndicatorInsets = native;
		}
	}

	partial void ArrangeContent(
		Size viewport) =>
		LayoutContent(viewport);

	partial void ScrollToCore(
		double offset,
		bool animated)
	{
		if (!IsRealized)
			return;

		UIScrollView host = (UIScrollView)Native;

		host.SetContentOffset(
			Orientation == Orientation.Vertical
				? new(host.ContentOffset.X, (nfloat)offset)
				: new((nfloat)offset, host.ContentOffset.Y),
			animated);
	}


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
		Thickness padding = Padding;

		double padLeft = padding.Left + (vertical ? bled.Left : 0);
		double padRight = padding.Right + (vertical ? bled.Right : 0);
		double padTop = padding.Top + (vertical ? 0 : bled.Top);
		double padBottom = padding.Bottom + (vertical ? 0 : bled.Bottom);

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
		host.ContentSize = new(width + padLeft + padRight, height + padTop + padBottom);
	}

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
			bool intersects = keyboard.GetMaxX() > hostInWindow.GetMinX()
				&& keyboard.GetMinX() < hostInWindow.GetMaxX();

			if (intersects)
			{
				// the safe-area bottom is already in the adjusted inset, so do not count it twice
				covered = (nfloat)Math.Max(
					0,
					hostInWindow.GetMaxY() - keyboard.GetMinY() - host.SafeAreaInsets.Bottom);
			}
		}

		keyboardCover = covered;

		double duration = UIKeyboard.AnimationDurationFromNotification(notification);
		UIView.Animate(duration, ApplyContentInsets);

		if (hiding || focused is null)
			return;

		CGRect target = focused.ConvertRectToView(focused.Bounds, host).Inset(0, -8);
		UIEdgeInsets adjusted = host.AdjustedContentInset;
		nfloat visibleTop = host.ContentOffset.Y + adjusted.Top;
		nfloat visibleBottom = host.ContentOffset.Y + host.Bounds.Height - adjusted.Bottom;

		if (target.GetMinY() < visibleTop || target.GetMaxY() > visibleBottom)
			host.ScrollRectToVisible(target, true);
	}

	internal void OnDragEnded()
	{
		if (!endsAfterDrag)
			return;

		endsAfterDrag = false;
		EndNativeRefresh();
	}

	internal void OnScrolled(
		double offset) =>
		Scrolled?.Invoke(offset);


	private protected override UIView CreateNative() =>
		new ScrollHost(this);

	private protected override void ApplyProperties()
	{
		UIScrollView host = (UIScrollView)Native;
		bool vertical = Orientation == Orientation.Vertical;

		host.AlwaysBounceVertical = vertical;
		host.AlwaysBounceHorizontal = !vertical;

		host.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never;

		ApplyKeyboardDismiss();
		ApplyBehavior();
		ApplyRefresh(host);
	}
}

internal sealed class ScrollHost : UIScrollView
{
	readonly ScrollView? element;

	public ScrollHost(
		ScrollView element)
	{
		this.element = element;

		NSNotificationCenter.DefaultCenter.AddObserver(this, new("keyboardFrameChanged:"), UIKeyboard.WillChangeFrameNotification, null);
		NSNotificationCenter.DefaultCenter.AddObserver(this, new("keyboardHidden:"), UIKeyboard.WillHideNotification, null);
	}

	public ScrollHost(
		NativeHandle handle) : base(handle)
	{ }


	// ReSharper disable once UnusedMember.Local
	[Export("keyboardFrameChanged:")]
	void KeyboardFrameChanged(
		NSNotification notification) =>
		element?.OnKeyboardChanged(notification, hiding: false);

	// ReSharper disable once UnusedMember.Local
	[Export("keyboardHidden:")]
	void KeyboardHidden(
		NSNotification notification) =>
		element?.OnKeyboardChanged(notification, hiding: true);


	public override CGPoint ContentOffset
	{
		get => base.ContentOffset;
		set
		{
			base.ContentOffset = value;

			element?.OnScrolled(value.Y + AdjustedContentInset.Top);

			if (!Dragging)
				element?.OnDragEnded();
		}
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		element?.LayoutContent(new(Bounds.Width, Bounds.Height));
	}


	protected override void Dispose(
		bool disposing)
	{
		if (disposing)
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);

		base.Dispose(disposing);
	}
}
