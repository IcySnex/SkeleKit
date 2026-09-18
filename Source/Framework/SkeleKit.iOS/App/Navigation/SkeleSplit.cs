namespace SkeleKit;

internal sealed class SkeleSplit : UISplitViewController
{
	readonly SplitViewColumn navigationColumn;
	UINavigationController? visibleNavigationStack;


	public SkeleSplit(
		SplitViewBuilder builder) : base(Native(builder.SplitStyle))
	{
		navigationColumn = builder.NavigationTarget;
		PrimaryEdge = Native(builder.PrimaryColumnEdge);
		PreferredSplitBehavior = Native(builder.BehaviorPreference);
		PreferredDisplayMode = Native(
			builder.DisplayPreference,
			builder.BehaviorPreference,
			builder.SplitStyle);
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

	static UISplitViewControllerPrimaryEdge Native(
		SplitViewEdge edge) =>
		edge is SplitViewEdge.Trailing
			? UISplitViewControllerPrimaryEdge.Trailing
			: UISplitViewControllerPrimaryEdge.Leading;

	static UISplitViewControllerSplitBehavior Native(
		SplitViewBehavior behavior) =>
		behavior switch
		{
			SplitViewBehavior.SideBySide => UISplitViewControllerSplitBehavior.Tile,
			SplitViewBehavior.Overlay => UISplitViewControllerSplitBehavior.Overlay,
			SplitViewBehavior.Displace => UISplitViewControllerSplitBehavior.Displace,
			_ => UISplitViewControllerSplitBehavior.Automatic
		};

	static UISplitViewControllerDisplayMode Native(
		SplitViewDisplay display,
		SplitViewBehavior behavior,
		SplitViewStyle style) =>
		display switch
		{
			SplitViewDisplay.SecondaryOnly => UISplitViewControllerDisplayMode.SecondaryOnly,
			SplitViewDisplay.TwoColumns when behavior is SplitViewBehavior.Overlay =>
				UISplitViewControllerDisplayMode.OneOverSecondary,
			SplitViewDisplay.TwoColumns => UISplitViewControllerDisplayMode.OneBesideSecondary,
			SplitViewDisplay.AllColumns when style is SplitViewStyle.DoubleColumn
				&& behavior is SplitViewBehavior.Overlay => UISplitViewControllerDisplayMode.OneOverSecondary,
			SplitViewDisplay.AllColumns when style is SplitViewStyle.DoubleColumn =>
				UISplitViewControllerDisplayMode.OneBesideSecondary,
			SplitViewDisplay.AllColumns when behavior is SplitViewBehavior.Overlay =>
				UISplitViewControllerDisplayMode.TwoOverSecondary,
			SplitViewDisplay.AllColumns when behavior is SplitViewBehavior.Displace =>
				UISplitViewControllerDisplayMode.TwoDisplaceSecondary,
			SplitViewDisplay.AllColumns => UISplitViewControllerDisplayMode.TwoBesideSecondary,
			_ => UISplitViewControllerDisplayMode.Automatic
		};

	internal void ApplyWeights(
		SplitViewBuilder builder)
	{
		SplitViewColumn[] columns = builder.Columns.Keys
			.Where(column => column is not SplitViewColumn.Compact)
			.Where(column => column is not SplitViewColumn.Inspector || OperatingSystem.IsIOSVersionAtLeast(26))
			.ToArray();

		if (!columns.Any(builder.ColumnWeights.ContainsKey))
			return;

		double total = columns.Sum(column => builder.ColumnWeights.GetValueOrDefault(column, 1));
		foreach (SplitViewColumn column in columns)
		{
			nfloat fraction = (nfloat)(builder.ColumnWeights.GetValueOrDefault(column, 1) / total);

			switch (column)
			{
				case SplitViewColumn.Primary:
					PreferredPrimaryColumnWidthFraction = fraction;
					MaximumPrimaryColumnWidth = (nfloat)double.PositiveInfinity;
					break;
				case SplitViewColumn.Supplementary:
					PreferredSupplementaryColumnWidthFraction = fraction;
					MaximumSupplementaryColumnWidth = (nfloat)double.PositiveInfinity;
					break;
				case SplitViewColumn.Inspector when OperatingSystem.IsIOSVersionAtLeast(26):
					PreferredInspectorColumnWidthFraction = fraction;
					MaximumInspectorColumnWidth = (nfloat)double.PositiveInfinity;
					break;
			}
		}
	}

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
