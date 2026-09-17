namespace SkeleKit;

internal sealed class SkeleSplit : UISplitViewController
{
	readonly SplitViewColumn navigationColumn;
	UINavigationController? visibleNavigationStack;


	public SkeleSplit(
		SplitViewStyle style,
		SplitViewColumn navigationColumn) : base(Native(style))
	{
		this.navigationColumn = navigationColumn;
	}

	public SkeleSplit(
		ObjCRuntime.NativeHandle handle) : base(handle)
	{ }


	internal UINavigationController? NavigationStack
	{
		get
		{
			if (Collapsed)
			{
				// while collapsed UIKit shows a single column: the compact stack when declared,
				// otherwise the top column, which viewControllers exposes as its only entry
				if (GetViewController(UISplitViewControllerColumn.Compact) is UINavigationController compact)
					return compact;

				if (ViewControllers?.LastOrDefault() is UINavigationController visible)
					return visible;
			}

			return GetViewController(NavigationColumn) as UINavigationController;
		}
	}

	UISplitViewControllerColumn NavigationColumn =>
		navigationColumn is SplitViewColumn.Inspector
			&& !OperatingSystem.IsIOSVersionAtLeast(26)
				? UISplitViewControllerColumn.Secondary
				: Native(navigationColumn);

	internal static UISplitViewControllerColumn Native(
		SplitViewColumn column)
	{
		if (column is SplitViewColumn.Inspector)
		{
			if (!OperatingSystem.IsIOSVersionAtLeast(26))
				throw new PlatformNotSupportedException("The split view inspector column requires iOS 26 or later.");

			return UISplitViewControllerColumn.Inspector;
		}

		return column switch
		{
			SplitViewColumn.Primary => UISplitViewControllerColumn.Primary,
			SplitViewColumn.Supplementary => UISplitViewControllerColumn.Supplementary,
			SplitViewColumn.Secondary => UISplitViewControllerColumn.Secondary,
			SplitViewColumn.Compact => UISplitViewControllerColumn.Compact,
			_ => UISplitViewControllerColumn.Secondary
		};
	}

	static UISplitViewControllerStyle Native(
		SplitViewStyle style) =>
		style is SplitViewStyle.TripleColumn
			? UISplitViewControllerStyle.TripleColumn
			: UISplitViewControllerStyle.DoubleColumn;

	public override void ViewDidLayoutSubviews()
	{
		base.ViewDidLayoutSubviews();

		if (!OperatingSystem.IsIOSVersionAtLeast(27))
			return;

		UINavigationController? current = NavigationStack;
		if (ReferenceEquals(current, visibleNavigationStack))
			return;

		UINavigationController? previous = visibleNavigationStack;
		visibleNavigationStack = current;
		SkeleApplication.Current?.SplitNavigationStackChanged(this, previous, current);
	}
}
