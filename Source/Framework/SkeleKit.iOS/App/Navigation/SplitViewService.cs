namespace SkeleKit;

internal sealed class SplitViewService : ISplitView
{
	readonly Func<SkeleSplit?> activeSplit;

	public SplitViewService(
		Func<SkeleSplit?> activeSplit)
	{
		this.activeSplit = activeSplit;
		SkeleSplit.CollapsedChanged += OnSplitCollapsedChanged;
	}


	public event Action<bool>? CollapsedChanged;


	void OnSplitCollapsedChanged(
		SkeleSplit split)
	{
		if (ReferenceEquals(split, activeSplit()))
			CollapsedChanged?.Invoke(split.Collapsed);
	}

	SkeleSplit Active() =>
		activeSplit()
		?? throw new InvalidOperationException("There is no active split view controller.");

	static void EnsureAvailable(
		SplitViewColumn column)
	{
		if (column is SplitViewColumn.Inspector
			&& !OperatingSystem.IsIOSVersionAtLeast(26))
			throw new PlatformNotSupportedException("The split view inspector column requires iOS 26 or later.");
	}

	static void EnsureShowable(
		SplitViewColumn column)
	{
		EnsureAvailable(column);

		// UIKit manages the compact column itself.
		if (column is SplitViewColumn.Compact)
			throw new InvalidOperationException("The compact column can't be shown; the split view manages it while collapsed.");
	}

	static void EnsureHideable(
		SplitViewColumn column)
	{
		EnsureShowable(column);

		// UISplitViewController.HideColumn doesn't support the secondary column.
		if (column is SplitViewColumn.Secondary)
			throw new InvalidOperationException("The secondary column can't be hidden by UISplitViewController.");
	}


	public bool IsCollapsed =>
		Active().Collapsed;

	public bool IsShowing(
		SplitViewColumn column)
	{
		EnsureAvailable(column);

		if (!OperatingSystem.IsIOSVersionAtLeast(26))
			throw new PlatformNotSupportedException("Querying split view column visibility requires iOS 26 or later. Use Show or Hide instead.");

		return Active().IsShowingColumn(SkeleSplit.Native(column));
	}

	public void Show(
		SplitViewColumn column)
	{
		EnsureShowable(column);
		Active().ShowColumn(SkeleSplit.Native(column));
	}

	public void Hide(
		SplitViewColumn column)
	{
		EnsureHideable(column);
		Active().HideColumn(SkeleSplit.Native(column));
	}

	public void Toggle(
		SplitViewColumn column)
	{
		EnsureHideable(column);

		if (!OperatingSystem.IsIOSVersionAtLeast(26))
			throw new PlatformNotSupportedException("Toggling a split view column requires iOS 26 or later. Use Show or Hide instead.");

		if (Active().IsShowingColumn(SkeleSplit.Native(column)))
			Hide(column);
		else
			Show(column);
	}
}
