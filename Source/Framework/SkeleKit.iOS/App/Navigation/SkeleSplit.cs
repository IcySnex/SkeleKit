namespace SkeleKit;

internal sealed class SkeleSplit : UISplitViewController
{
	readonly SplitViewColumn navigationColumn;
	UINavigationController? leadingVisibleStack;


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

	internal UINavigationController? LeadingVisibleStack
	{
		get
		{
			if (Collapsed)
				return NavigationStack;

			List<(UINavigationController Stack, CGRect Frame)> visible = [];

			foreach ((UISplitViewControllerColumn column, UINavigationController stack) in ColumnStacks())
			{
				if (!IsShowingColumn(column)
					|| !stack.IsViewLoaded
					|| stack.View is not UIView view
					|| view.Hidden
					|| view.Bounds.Width <= 0)
					continue;

				visible.Add((stack, view.ConvertRectToView(view.Bounds, View)));
			}

			if (visible.Count == 0)
				return NavigationStack;

			bool rightToLeft = View.EffectiveUserInterfaceLayoutDirection
				is UIUserInterfaceLayoutDirection.RightToLeft;

			return rightToLeft
				? visible.MaxBy(entry => entry.Frame.GetMaxX()).Stack
				: visible.MinBy(entry => entry.Frame.GetMinX()).Stack;
		}
	}

	internal IEnumerable<UINavigationController> NavigationStacks() =>
		ColumnStacks()
			.Select(entry => entry.Stack)
			.Distinct();

	IEnumerable<(UISplitViewControllerColumn Column, UINavigationController Stack)> ColumnStacks()
	{
		UISplitViewControllerColumn[] columns = OperatingSystem.IsIOSVersionAtLeast(26)
			?
			[
				UISplitViewControllerColumn.Primary,
				UISplitViewControllerColumn.Supplementary,
				UISplitViewControllerColumn.Secondary,
				UISplitViewControllerColumn.Inspector,
				UISplitViewControllerColumn.Compact
			]
			:
			[
				UISplitViewControllerColumn.Primary,
				UISplitViewControllerColumn.Supplementary,
				UISplitViewControllerColumn.Secondary,
				UISplitViewControllerColumn.Compact
			];

		foreach (UISplitViewControllerColumn column in columns)
		{
			if (GetViewController(column) is UINavigationController stack)
				yield return (column, stack);
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

		UINavigationController? current = LeadingVisibleStack;
		if (ReferenceEquals(current, leadingVisibleStack))
			return;

		leadingVisibleStack = current;
		SkeleApplication.Current?.RefreshSidebarRecovery(true);
	}
}
