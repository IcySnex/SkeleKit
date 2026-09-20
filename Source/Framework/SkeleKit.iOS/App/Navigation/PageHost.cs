using System.Reflection.Metadata;
using System.Windows.Input;
using ObjCRuntime;

namespace SkeleKit;

internal interface INavigationAccessoryScrollSource
{
	event Action<double>? ScrollOffsetChanged;
}

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

	internal static void TopScrollEdgeStyleChanged() =>
		ForEachLive(host =>
		{
			if (!host.IsViewLoaded || host.Page is not ContentView page)
				return;

			if (page.NavigationAccessory is null)
				host.ApplyTopScrollEdgeStyle(page);
			else
				host.NavigationAccessoryChanged();
		});

	internal static void TitleStyleChanged() =>
		ForEachLive(host =>
		{
			if (ReferenceEquals(host.NavigationController?.TopViewController, host))
				host.ApplyTitleStyleChange();
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

		if (view is Container container)
		{
			foreach (View child in container.LogicalChildren)
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
	UIBarButtonItemGroup? sidebarRecoveryGroup;
	SheetGuard? dismissGuard;
	bool hasContentDetent;
	bool contentDetentPending;
	nfloat contentWidth;
	nfloat contentChrome;
	UINavigationBarAppearance? savedScrollEdgeAppearance;
	UINavigationBarAppearance? savedCompactScrollEdgeAppearance;
	bool preservesNavigationBarAppearance;
	bool hasCustomNavigationBarMinimization;
	UIView? navigationAccessoryHost;
	UIView? navigationAccessoryContent;
	UIVisualEffectView? navigationAccessoryMaterial;
	View? hostedNavigationAccessory;
	INavigationAccessoryScrollSource? navigationAccessoryScrollSource;
	UIScrollEdgeElementContainerInteraction? navigationAccessoryScrollEdge;
	UIScrollView? topScrollEdgeScrollView;
	UIScrollEdgeEffectStyle? previousTopScrollEdgeStyle;
	UIScrollEdgeEffectStyle? appliedTopScrollEdgeStyle;
	nfloat navigationAccessoryHeight;
	nfloat navigationAccessoryBaseInset;
	double navigationAccessoryScrollOffset;

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

		Size desired = page.MeasurePageContent(new(Math.Max(0, width), maximum));

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

		RemoveNavigationAccessory();
		RestoreTopScrollEdgeStyle();
		ApplyChrome(page);

		UIView native = page.Realize();
		usesSystemScrollInsets = page.ScrollsUnderBars
			&& page.SafeAreaEdges == SafeAreaEdges.All
			&& page.AutomaticScrollBleed is ISystemInsetScroll scrolling
			&& scrolling.UseSystemContentInsets();

		View!.AddSubview(native);

		UIScrollView? scroll = FindScrolling(page)?.Native as UIScrollView;
		if (scroll is not null)
			SetContentScrollView(scroll, NSDirectionalRectEdge.Top | NSDirectionalRectEdge.Bottom);

		ApplyTopScrollEdgeStyle(page, scroll);
		InstallNavigationAccessory(page, scroll);
	}

	internal void NavigationAccessoryChanged()
	{
		if (!IsViewLoaded || Page is not ContentView page)
			return;

		RemoveNavigationAccessory();
		UIScrollView? scroll = FindScrolling(page)?.Native as UIScrollView;
		ApplyTopScrollEdgeStyle(page, scroll);
		InstallNavigationAccessory(page, scroll);
		ApplyNavigationBarMinimization(page);
		ApplyBarAppearance(page);
		SetNavigationAccessoryActive(
			ReferenceEquals(NavigationController?.TopViewController, this)
			&& !page.HidesNavigationBar);

		View?.SetNeedsLayout();
		NavigationController?.NavigationBar.SetNeedsLayout();
	}

	void InstallNavigationAccessory(
		ContentView page,
		UIScrollView? scrollView)
	{
		if (page.NavigationAccessory is not View accessory
			|| View is not UIView controllerView
			|| NavigationController is not UINavigationController navigation
			|| page.HidesNavigationBar)
			return;

		UINavigationBar navigationBar = navigation.NavigationBar;
		hostedNavigationAccessory = accessory;
		navigationAccessoryBaseInset = AdditionalSafeAreaInsets.Top;

		navigationAccessoryHost = new()
		{
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin,
			ClipsToBounds = false,
			Hidden = true,
			UserInteractionEnabled = false
		};
		navigationAccessoryScrollOffset = scrollView is null
			? 0
			: scrollView.ContentOffset.Y + scrollView.AdjustedContentInset.Top;

		if (OperatingSystem.IsIOSVersionAtLeast(26))
		{
			if (scrollView is not null)
				ApplyNavigationAccessoryScrollEdgeInteraction(scrollView);
		}
		else
		{
			navigationAccessoryMaterial = new(
				UIBlurEffect.FromStyle(UIBlurEffectStyle.SystemChromeMaterial))
			{
				Alpha = 0,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
				Hidden = true,
				UserInteractionEnabled = false
			};
			// UINavigationBar may reorder its private content views after layout.
			navigationAccessoryMaterial.Layer.ZPosition = -1;
			navigationBar.InsertSubview(navigationAccessoryMaterial, 0);

			if (scrollView is INavigationAccessoryScrollSource source)
			{
				navigationAccessoryScrollSource = source;
				source.ScrollOffsetChanged += UpdateNavigationAccessoryMaterial;
			}
		}

		navigationAccessoryContent = new()
		{
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
			UserInteractionEnabled = false
		};
		navigationAccessoryContent.AddSubview(accessory.Realize());
		navigationAccessoryHost.AddSubview(navigationAccessoryContent);
		navigationBar.AddSubview(navigationAccessoryHost);
		SetNavigationAccessoryActive(
			ReferenceEquals(navigation.TopViewController, this)
			&& !navigation.NavigationBarHidden);

		controllerView.SetNeedsLayout();
		navigationBar.SetNeedsLayout();
	}

	void RemoveNavigationAccessory()
	{
		if (navigationAccessoryHost is null && hostedNavigationAccessory is null)
			return;

		if (navigationAccessoryScrollEdge is not null)
		{
			navigationAccessoryHost?.RemoveInteraction(navigationAccessoryScrollEdge);
			navigationAccessoryScrollEdge.Dispose();
			navigationAccessoryScrollEdge = null;
		}
		if (navigationAccessoryScrollSource is not null)
			navigationAccessoryScrollSource.ScrollOffsetChanged -= UpdateNavigationAccessoryMaterial;
		navigationAccessoryScrollSource = null;

		hostedNavigationAccessory?.Unrealize();
		hostedNavigationAccessory = null;

		navigationAccessoryContent?.RemoveFromSuperview();
		navigationAccessoryContent?.Dispose();
		navigationAccessoryContent = null;

		navigationAccessoryMaterial?.RemoveFromSuperview();
		navigationAccessoryMaterial?.Dispose();
		navigationAccessoryMaterial = null;

		navigationAccessoryHost?.RemoveFromSuperview();
		navigationAccessoryHost?.Dispose();
		navigationAccessoryHost = null;
		navigationAccessoryHeight = 0;
		navigationAccessoryScrollOffset = 0;

		UIEdgeInsets additional = AdditionalSafeAreaInsets;
		additional.Top = navigationAccessoryBaseInset;
		AdditionalSafeAreaInsets = additional;
	}

	internal void ApplyTopScrollEdgeStyle(
		ContentView page)
	{
		if (!IsViewLoaded)
			return;

		ApplyTopScrollEdgeStyle(page, FindScrolling(page)?.Native as UIScrollView);
	}

	void ApplyTopScrollEdgeStyle(
		ContentView page,
		UIScrollView? scrollView)
	{
		if (!OperatingSystem.IsIOSVersionAtLeast(26))
			return;

		if (!ReferenceEquals(topScrollEdgeScrollView, scrollView))
		{
			RestoreTopScrollEdgeStyle();
			topScrollEdgeScrollView = scrollView;
			previousTopScrollEdgeStyle = scrollView?.TopEdgeEffect.Style;
		}

		if (scrollView is null)
			return;

		ScrollEdgeStyle effective = page.TopScrollEdgeStyle
			?? SkeleApplication.Current?.Theme.TopScrollEdgeStyle
			?? ScrollEdgeStyle.Automatic;

		UIScrollEdgeEffectStyle style = effective switch
		{
			ScrollEdgeStyle.Soft => UIScrollEdgeEffectStyle.SoftStyle,
			ScrollEdgeStyle.Hard => UIScrollEdgeEffectStyle.HardStyle,
			_ => UIScrollEdgeEffectStyle.AutomaticStyle
		};

		appliedTopScrollEdgeStyle = style;
		scrollView.TopEdgeEffect.Style = style;

		ApplyNavigationAccessoryScrollEdgeInteraction(scrollView);
	}

	void RestoreTopScrollEdgeStyle()
	{
		if (OperatingSystem.IsIOSVersionAtLeast(26)
			&& topScrollEdgeScrollView is UIScrollView scrollView
			&& previousTopScrollEdgeStyle is UIScrollEdgeEffectStyle previous
			&& appliedTopScrollEdgeStyle is UIScrollEdgeEffectStyle applied
			&& scrollView.TopEdgeEffect.Style.Equals(applied))
			scrollView.TopEdgeEffect.Style = previous;

		topScrollEdgeScrollView = null;
		previousTopScrollEdgeStyle = null;
		appliedTopScrollEdgeStyle = null;
	}

	void ApplyNavigationAccessoryScrollEdgeInteraction(
		UIScrollView scrollView)
	{
		if (!OperatingSystem.IsIOSVersionAtLeast(26)
			|| navigationAccessoryHost is not UIView host)
			return;

		if (navigationAccessoryScrollEdge is not null)
			return;

		navigationAccessoryScrollEdge = new()
		{
			Edge = UIRectEdge.Top,
			ScrollView = scrollView
		};
		host.AddInteraction(navigationAccessoryScrollEdge);
	}

	void SetNavigationAccessoryActive(
		bool active)
	{
		if (navigationAccessoryHost is not UIView host)
			return;

		host.Alpha = 1;
		host.Transform = CGAffineTransform.MakeIdentity();
		if (navigationAccessoryContent is UIView content)
		{
			content.Alpha = 1;
			content.Transform = CGAffineTransform.MakeIdentity();
		}
		host.Hidden = !active;
		if (navigationAccessoryMaterial is UIVisualEffectView material)
			material.Hidden = !active;

		if (!active || NavigationController?.NavigationBar is not UINavigationBar navigationBar)
			return;

		UpdateNavigationAccessoryMaterial(navigationAccessoryScrollOffset);
		navigationBar.BringSubviewToFront(host);
		View?.SetNeedsLayout();
		navigationBar.SetNeedsLayout();
	}

	void TransitionNavigationAccessory(
		bool active,
		bool animated)
	{
		if (navigationAccessoryHost is not UIView host
			|| navigationAccessoryContent is not UIView content)
			return;

		IUIViewControllerTransitionCoordinator? transition = animated
			? this.GetTransitionCoordinator()
			: null;
		if (transition is null)
		{
			SetNavigationAccessoryActive(active);
			return;
		}

		if (active)
		{
			SetNavigationAccessoryActive(true);
			content.Alpha = 0;
		}
		else if (host.Hidden)
		{
			return;
		}

		nfloat slide = 0;
		if (transition.PresentationStyle is UIModalPresentationStyle.None
			&& NavigationController is SkeleApplication.SkeleStack { Delegate: null } navigation)
		{
			bool pushes = active
				? IsMovingToParentViewController
				: !IsMovingFromParentViewController;
			nfloat forward = transition.ContainerView.Bounds.Width;
			if (navigation.NavigationBar.EffectiveUserInterfaceLayoutDirection
				is UIUserInterfaceLayoutDirection.RightToLeft)
				forward = -forward;
			slide = (active ? 1 : -1) * (pushes ? forward : -forward);

			if (active)
				content.Transform = CGAffineTransform.MakeTranslation(slide, 0);
		}

		bool scheduled = transition.AnimateAlongsideTransition(
			_ =>
			{
				content.Alpha = active ? 1 : 0;
				content.Transform = active
					? CGAffineTransform.MakeIdentity()
					: CGAffineTransform.MakeTranslation(slide, 0);
			},
			context =>
			{
				if (!ReferenceEquals(navigationAccessoryHost, host)
					|| !ReferenceEquals(navigationAccessoryContent, content))
					return;

				SetNavigationAccessoryActive(context.IsCancelled ? !active : active);
			});

		if (!scheduled)
			SetNavigationAccessoryActive(active);
	}

	void LayoutNavigationAccessory()
	{
		if (navigationAccessoryHost is not UIView host
			|| hostedNavigationAccessory is not View accessory
			|| View is not UIView controllerView
			|| NavigationController is not UINavigationController navigation
			|| navigation.NavigationBarHidden)
			return;

		UINavigationBar navigationBar = navigation.NavigationBar;

		UIEdgeInsets safe = controllerView.SafeAreaInsets;
		nfloat left = safe.Left > 0 ? safe.Left : 0;
		nfloat right = safe.Right > 0 ? safe.Right : 0;
		nfloat width = (nfloat)Math.Max(0, navigationBar.Bounds.Width - left - right);

		accessory.Measure(new((double)width, double.PositiveInfinity));
		nfloat height = (nfloat)Math.Max(0, accessory.DesiredSize.Height);

		if (Math.Abs((double)(height - navigationAccessoryHeight)) > 0.5)
		{
			navigationAccessoryHeight = height;
			UIEdgeInsets additional = AdditionalSafeAreaInsets;
			additional.Top = navigationAccessoryBaseInset + height;
			AdditionalSafeAreaInsets = additional;
		}

		host.Frame = new(left, navigationBar.Bounds.Height, width, navigationAccessoryHeight);
		if (navigationAccessoryContent is UIView content)
		{
			content.Bounds = new(0, 0, width, navigationAccessoryHeight);
			content.Center = new(width / 2, navigationAccessoryHeight / 2);
		}
		if (navigationAccessoryMaterial is UIVisualEffectView material)
		{
			CGRect barFrame = navigationBar.ConvertRectToView(navigationBar.Bounds, controllerView);
			nfloat materialTop = -barFrame.Y;
			material.Frame = new(
				left,
				materialTop,
				width,
				-materialTop + navigationBar.Bounds.Height + navigationAccessoryHeight);
			UpdateNavigationAccessoryMaterial(navigationAccessoryScrollOffset);
		}

		accessory.Arrange(new(0, 0, (double)width, (double)navigationAccessoryHeight));
		navigationBar.BringSubviewToFront(host);
	}

	void UpdateNavigationAccessoryMaterial(
		double offset)
	{
		navigationAccessoryScrollOffset = offset;
		if (navigationAccessoryMaterial is not UIVisualEffectView material)
			return;

		nfloat alpha = (nfloat)Math.Clamp(offset / 8, 0, 1);
		if (Math.Abs((double)(material.Alpha - alpha)) > 0.001)
			material.Alpha = alpha;
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

		UpdateSystemInsets();
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
			item.Icon?.ResolveLocal(),
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

		ApplyVisibilityPriority(native, item);
		nativeToolbarItems[native] = item;
		native.TintColor = EffectiveBarTint(Page, item);

		return native;
	}

	static UIColor? EffectiveBarTint(
		ContentView? page,
		ToolbarItem? item = null) =>
		(item?.Tint ?? page?.BarTintValue ?? SkeleApplication.Current?.Theme.Tint)?.ToUIColor();

	UIMenu BuildMenu(
		ToolbarItem item)
	{
		UIAction[] entries = new UIAction[item.Menu.Count];

		for (int index = 0; index < item.Menu.Count; index++)
		{
			MenuAction entry = item.Menu[index];

			entries[index] = UIAction.Create(
				entry.Text,
				entry.Icon?.ResolveLocal(),
				null,
				_ =>
				{
					if (entry.Command is ICommand entryCommand && entryCommand.CanExecute(entry.CommandParameter))
						entryCommand.Execute(entry.CommandParameter);
				});
			entries[index].Subtitle = entry.Subtitle;

			if (entry.IsDestructive)
				entries[index].Attributes = UIMenuElementAttributes.Destructive;
		}

		menuActions.AddRange(entries);

		return UIMenu.Create(entries);
	}

	static void ApplyVisibilityPriority(
		UIBarButtonItem native,
		ToolbarItem item)
	{
		if (!OperatingSystem.IsIOSVersionAtLeast(27))
			return;

		native.VisibilityPriority = item.VisibilityPriority switch
		{
			ToolbarVisibilityPriority.Low => UIBarButtonItemVisibilityPriority.Low,
			ToolbarVisibilityPriority.High => UIBarButtonItemVisibilityPriority.High,
			_ => UIBarButtonItemVisibilityPriority.Standard
		};
	}

	UIBarButtonItem MenuBar(
		ToolbarItem item)
	{
		UIMenu menu = BuildMenu(item);

		UIBarButtonItem native = item.Icon is ImageSource icon
			? new(icon.ResolveLocal(), menu)
			: new(item.Text ?? "", menu);

		if (item.IsPrimary)
			native.Style = UIBarButtonItemStyle.Done;

		ApplyVisibilityPriority(native, item);
		nativeToolbarItems[native] = item;
		native.TintColor = EffectiveBarTint(Page, item);

		return native;
	}

	void ApplyChrome(
		ContentView page)
	{
		// ContentView owns the visible page fill; the controller must not introduce another background.
		View!.BackgroundColor = UIColor.Clear;

		NavigationItem.Title = page.Title.Value;
		NavigationItem.Prompt = page.Prompt.Value;
		NavigationItem.BackButtonTitle = page.BackButtonTitleValue;
		NavigationItem.BackButtonDisplayMode = page.BackButtonStyle switch
		{
			BackButtonStyle.Generic => UINavigationItemBackButtonDisplayMode.Generic,
			BackButtonStyle.Minimal => UINavigationItemBackButtonDisplayMode.Minimal,
			_ => UINavigationItemBackButtonDisplayMode.Default
		};

		ApplyTitleStyle(page);
		ApplyNavigationBarMinimization(page);

		ApplyBarAppearance(page);

		ApplyToolbar(page);
		ApplySearch(page);
	}

	internal void ApplyNavigationBarMinimization(
		ContentView page)
	{
		if (!OperatingSystem.IsIOSVersionAtLeast(27))
			return;

		// Only opt-in pages may carry a minimization configuration. Assigning one flips the bar
		// into UIKit's minimization machinery even when it says Never, and on iOS 27 that stalls
		// the tab sidebar morph: the title moves while the content's safe area follows a second
		// late. Pages at their defaults must keep the system state they were born with.
		if (page.NavigationAccessory is not null
			|| (page.NavigationBarMinimizeBehavior is NavigationBarMinimize.Never
				&& page.NavigationBarMinimizeSafeAreaAdjustment is NavigationBarMinimizeSafeArea.Automatic
				&& page.NavigationBarMinimizeRestorationBehavior is NavigationBarMinimizeRestore.Automatic))
		{
			if (!hasCustomNavigationBarMinimization)
				return;

			NavigationItem.NavigationBarMinimization = new()
			{
				MinimizationBehavior = UIBarMinimizationBehavior.Never,
				SafeAreaAdjustment = UIBarMinimizationSafeAreaAdjustment.Automatic,
				RestorationBehavior = UIBarMinimizationRestorationBehavior.Automatic
			};
			hasCustomNavigationBarMinimization = false;
			NavigationController?.NavigationBar.SetNeedsLayout();
			return;
		}

		NavigationItem.NavigationBarMinimization = new()
		{
			MinimizationBehavior = page.EffectiveNavigationBarMinimizeBehavior switch
			{
				NavigationBarMinimize.Automatic => UIBarMinimizationBehavior.Automatic,
				NavigationBarMinimize.OnScrollDown => UIBarMinimizationBehavior.OnScrollDown,
				NavigationBarMinimize.OnScrollUp => UIBarMinimizationBehavior.OnScrollUp,
				_ => UIBarMinimizationBehavior.Never
			},
			SafeAreaAdjustment = page.NavigationBarMinimizeSafeAreaAdjustment switch
			{
				NavigationBarMinimizeSafeArea.Enabled => UIBarMinimizationSafeAreaAdjustment.Enabled,
				NavigationBarMinimizeSafeArea.Disabled => UIBarMinimizationSafeAreaAdjustment.Disabled,
				_ => UIBarMinimizationSafeAreaAdjustment.Automatic
			},
			RestorationBehavior = page.NavigationBarMinimizeRestorationBehavior switch
			{
				NavigationBarMinimizeRestore.AtScrollEdge => UIBarMinimizationRestorationBehavior.AtScrollEdge,
				_ => UIBarMinimizationRestorationBehavior.Automatic
			}
		};
		hasCustomNavigationBarMinimization = true;

		NavigationController?.NavigationBar.SetNeedsLayout();
	}

	void ApplyTitleStyle(
		ContentView page)
	{
		TitleStyle effective = page.NavigationTitleStyle
			?? SkeleApplication.Current?.Theme.NavigationTitleStyle
			?? TitleStyle.Inline;

		bool large = effective is TitleStyle.Large;

		NavigationItem.LargeTitleDisplayMode = large
			? UINavigationItemLargeTitleDisplayMode.Always
			: UINavigationItemLargeTitleDisplayMode.Never;

		// the stack-wide preference gates large titles: lift it when a page needs them, but never
		// lower it again. Inline pages opt out per-item through the display mode above, and
		// flipping the gate while a container resizes (sidebar toggle, split collapse) makes the
		// large title jump and only settle once the transition finishes.
		if (large
			&& NavigationController is UINavigationController navigation
			&& !navigation.NavigationBar.PrefersLargeTitles)
			navigation.NavigationBar.PrefersLargeTitles = true;
	}

	internal void ApplyTitleStyleChange()
	{
		if (!IsViewLoaded || Page is not ContentView page)
			return;

		ApplyTitleStyle(page);

		NavigationController?.View?.SetNeedsLayout();
		NavigationController?.NavigationBar.SetNeedsLayout();
		NavigationController?.View?.LayoutIfNeeded();
		NavigationController?.NavigationBar.LayoutIfNeeded();

		if (page.NavigationAccessory is not null)
			LayoutNavigationAccessory();
		else
		{
			View?.SetNeedsLayout();
			View?.LayoutIfNeeded();
		}
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
		bool continuousAccessory = page.NavigationAccessory is not null
			&& !OperatingSystem.IsIOSVersionAtLeast(26);
		UINavigationBarAppearance standard = continuousAccessory
			? Transparent()
			: bar?.StandardAppearance.Copy() as UINavigationBarAppearance ?? new();
		UINavigationBarAppearance edge = continuousAccessory
			? Transparent()
			: page.ScrollsUnderBars
				? bar?.ScrollEdgeAppearance?.Copy() as UINavigationBarAppearance ?? Transparent()
				: standard.Copy() as UINavigationBarAppearance ?? new();

		UIStringAttributes titleAttributes = new()
		{
			ForegroundColor = page.TitleColorValue?.ToUIColor() ?? UIColor.Label
		};
		standard.TitleTextAttributes = titleAttributes;
		edge.TitleTextAttributes = titleAttributes;

		UIStringAttributes largeTitleAttributes = new()
		{
			ForegroundColor = page.LargeTitleColorValue?.ToUIColor() ?? UIColor.Label
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

	internal void ApplyBindableChrome(
		ContentView page)
	{
		if (!IsViewLoaded)
			return;

		NavigationItem.BackButtonTitle = page.BackButtonTitleValue;
		ApplyBarAppearance(page);
		ApplyToolbarTint(page);

		if (ReferenceEquals(NavigationController?.TopViewController, this))
		{
			ApplyNavigationTint(page);
			SetNeedsStatusBarAppearanceUpdate();
		}
	}

	void ApplyThemeChange()
	{
		if (Page is not ContentView page)
			return;

		page.ReapplyVisuals();

		if (!IsViewLoaded)
			return;

		ApplyBarAppearance(page);

		if (!ReferenceEquals(NavigationController?.TopViewController, this))
			return;

		ApplyNavigationTint(page);
		ApplyToolbarTint(page);
		SetNeedsStatusBarAppearanceUpdate();

		NavigationController?.NavigationBar.SetNeedsLayout();
		NavigationController?.NavigationBar.LayoutIfNeeded();
	}

	void ApplyKeyboard(
		NSNotification notification,
		bool hiding)
	{
		if (Page is null || !Page.IsRealized || Page.Native.Subviews.FirstOrDefault() is UIScrollView || View?.Window is null)
			return;

		keyboardFocus = hiding ? null : FirstResponder(Page.Native);

		nfloat cover = 0;
		if (!hiding)
		{
			CGRect keyboard = UIKeyboard.FrameEndFromNotification(notification);
			CGRect pageInWindow = View.ConvertRectToView(View.Bounds, null);

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

	UIBarButtonItemGroup SidebarRecoveryGroup() =>
		sidebarRecoveryGroup ??= new(
			[SkeleApplication.SidebarRecoveryItem()],
			null);

	internal void RefreshSidebarRecovery()
	{
		if (!IsViewLoaded || sidebarRecoveryGroup is not UIBarButtonItemGroup recovery)
			return;

		recovery.Hidden = SkeleApplication.Current?.ShowsSidebarRecovery(this) is not true;
	}

	void ApplyToolbar(
		ContentView page)
	{
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

		if (OperatingSystem.IsIOSVersionAtLeast(27))
		{
			UIBarButtonItemGroup recovery = SidebarRecoveryGroup();
			recovery.Hidden = SkeleApplication.Current?.ShowsSidebarRecovery(this) is not true;

			List<UIBarButtonItemGroup> groups = [recovery];
			if (leading.Count > 0)
				groups.Add(new([.. leading], null));

			NavigationItem.LeadingItemGroups = [.. groups];
		}
		else
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
		if (page.SearchPlaceholderValue is not string placeholder)
			return;

		search = new((UIViewController?)null)
		{
			ObscuresBackgroundDuringPresentation = page.SearchObscuresBackground
		};

		search.SearchBar.Placeholder = placeholder;
		search.SearchBar.Text = page.SearchText.Value ?? string.Empty;
		search.SearchBar.TextChanged += (_, e) =>
		{
			if (page.HidesSearchScopesWhenEmpty)
				search.SearchBar.SetShowsScopeBar(!string.IsNullOrEmpty(e.SearchText), true);

			page.NotifySearch(e.SearchText);
		};
		search.SearchBar.SearchButtonClicked += (_, _) => page.NotifySearchSubmitted();
		search.SearchBar.SelectedScopeButtonIndexChanged += (_, e) => page.NotifySearchScope((int)e.SelectedScope);
		search.SearchBar.CancelButtonClicked += (_, _) =>
		{
			if (page.HidesSearchScopesWhenEmpty)
				search.SearchBar.SetShowsScopeBar(false, true);

			page.NotifySearchCanceled();
		};

		if (page.SearchScopeValues.Count > 0)
		{
			search.SearchBar.ScopeButtonTitles = [.. page.SearchScopeValues];
			search.SearchBar.SelectedScopeButtonIndex = page.SearchScopeIndex.Value;
			search.SearchBar.ScopeBarBackgroundImage = TransparentScopeBackground;

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

	internal void ApplySearchConfiguration(
		ContentView page)
	{
		if (!IsViewLoaded)
			return;

		if (page.SearchPlaceholderValue is not string placeholder)
		{
			NavigationItem.SearchController = null;
			search = null;
			return;
		}

		if (search is null)
		{
			ApplySearch(page);
			return;
		}

		search.SearchBar.Placeholder = placeholder;
		search.SearchBar.ScopeButtonTitles = page.SearchScopeValues.Count > 0
			? [.. page.SearchScopeValues]
			: null;
		search.SearchBar.SelectedScopeButtonIndex = page.SearchScopeIndex.Value;
		search.SearchBar.ShowsScopeBar = page.SearchScopeValues.Count > 0
			&& (!page.HidesSearchScopesWhenEmpty || !string.IsNullOrEmpty(page.SearchText.Value));
	}

	internal void ApplySearchText(
		string value)
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
			RemoveNavigationAccessory();
			RestoreTopScrollEdgeStyle();
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
		Page?.StatusBarValue switch
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

		UpdateSystemInsets();
		InstallPage();

		// numeric keyboards have no return key, so tapping outside is the only way out
		dismissKeyboard = new(() => View!.EndEditing(true))
		{
			CancelsTouchesInView = false
		};
		dismissKeyboard.ShouldReceiveTouch = (_, touch) =>
			FirstResponder(View!) is not UIView focused || !IsWithin(focused, touch.View);
		View!.AddGestureRecognizer(dismissKeyboard);

		themeChange = RegisterForTraitChanges([typeof(UITraitUserInterfaceStyle)], (_, _) => ApplyThemeChange());
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
		TransitionNavigationAccessory(!Page.HidesNavigationBar, animated);
		ApplyTitleStyle(Page);
		ApplyBarAppearance(Page);

		// a bottom toolbar and the floating tab bar share the same edge: the toolbar only shows when
		// the tab bar is gone — a page that wants one sets HidesTabBar
		bool hasToolbar = Page.BottomToolbarItems.Count > 0
			&& (HidesBottomBarWhenPushed || TabBarController is null);

		NavigationController?.SetToolbarHidden(!hasToolbar, animated);

		if (TabBarController is UITabBarController tabs
			&& OperatingSystem.IsIOSVersionAtLeast(26)
			&& SkeleApplication.Current is { Accessory: { } accessory } app)
			tabs.SetBottomAccessory(app.AccessoryWanted && !HidesBottomBarWhenPushed ? accessory : null, animated);

		ApplyNavigationTint(Page);
		ApplyToolbar(Page);

		ApplyBackGuard();
		ApplySheetGuard();
	}

	public override void ViewDidAppear(
		bool animated)
	{
		base.ViewDidAppear(animated);

		ApplyPopGestures();

		SkeleApplication.Current?.CompleteTabSelection(this);
		Page?.NotifyAppeared();

		if (Page is ContentView page)
		{
			SetNavigationAccessoryActive(!page.HidesNavigationBar);
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

		TransitionNavigationAccessory(false, animated);
		PreserveNavigationBarAppearance();
		Page?.NotifyDisappearing();
	}

	public override void ViewSafeAreaInsetsDidChange()
	{
		base.ViewSafeAreaInsetsDidChange();

		View?.SetNeedsLayout();
	}

	internal void AnimateAlongsideSidebar()
	{
		if (!IsViewLoaded
			|| View is not UIView view
			|| Page is not { IsRealized: true } page)
			return;

		void Layout()
		{
			view.SetNeedsLayout();
			view.LayoutIfNeeded();
			page.Native.LayoutIfNeeded();
		}

		double duration = UIView.InheritedAnimationDuration;
		if (duration <= 0)
		{
			Layout();
			return;
		}

		UIView.AnimateNotify(
			duration,
			0,
			UIViewAnimationOptions.AllowUserInteraction
				| UIViewAnimationOptions.BeginFromCurrentState,
			Layout,
			static _ => { });
	}

	public override void ViewLayoutMarginsDidChange()
	{
		base.ViewLayoutMarginsDidChange();

		UpdateSystemInsets();
	}

	void UpdateSystemInsets()
	{
		if (Page is not ContentView page)
			return;

		NSDirectionalEdgeInsets insets = SystemMinimumLayoutMargins;
		page.UpdatePageSystemInsets(new(
			insets.Leading,
			insets.Top,
			insets.Trailing,
			insets.Bottom),
			View?.EffectiveUserInterfaceLayoutDirection == UIUserInterfaceLayoutDirection.RightToLeft);
	}

	public override void ViewDidLayoutSubviews()
	{
		base.ViewDidLayoutSubviews();

		if (Page is null)
			return;

		LayoutNavigationAccessory();
		UpdateSystemInsets();
		UIEdgeInsets safe = View!.SafeAreaInsets;
		Thickness pageSafeArea = usesSystemScrollInsets
			? new(safe.Left, 0, safe.Right, 0)
			: new(safe.Left, safe.Top, safe.Right, safe.Bottom);
		CGRect frame = View.Bounds;
		nfloat availableWidth = frame.Width;
		if (Page.SafeAreaEdges.HasFlag(SafeAreaEdges.Leading))
			availableWidth -= safe.Left;
		if (Page.SafeAreaEdges.HasFlag(SafeAreaEdges.Trailing))
			availableWidth -= safe.Right;
		nfloat chrome = (nfloat)ChromeHeight(Page);

		if (contentWidth != availableWidth || contentChrome != chrome)
		{
			contentWidth = availableWidth;
			contentChrome = chrome;
			ContentMeasureInvalidated();
		}

		Page.UpdatePageLayout(pageSafeArea, keyboardCover);
		Page.ApplyHostFrame(new(frame.X, frame.Y, frame.Width, frame.Height));

		if (keyboardCover <= 0 || keyboardFocus is not UIView focused)
			return;

		Page.Native.LayoutIfNeeded();

		CGRect target = focused.ConvertRectToView(focused.Bounds, View);
		nfloat bottomInset = Page.SafeAreaEdges.HasFlag(SafeAreaEdges.Bottom) ? safe.Bottom : 0;
		nfloat visibleBottom = frame.GetMaxY() - bottomInset - keyboardCover;
		nfloat hidden = target.GetMaxY() + 8 - visibleBottom;

		if (hidden > 0)
		{
			Page.UpdatePageLayout(pageSafeArea, keyboardCover, Math.Min(hidden, keyboardCover));
			Page.Native.LayoutIfNeeded();
		}
	}

	public override void ViewDidDisappear(
		bool animated)
	{
		base.ViewDidDisappear(animated);

		RestoreNavigationBarAppearance();
		Page?.NotifyDisappeared();
		if (!ReferenceEquals(NavigationController?.TopViewController, this))
			SetNavigationAccessoryActive(false);

		if (IsMovingFromParentViewController)
			Page?.Unrealize();
	}
}
