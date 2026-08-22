namespace SkeleKit;

/// <summary>
/// A segmented control choosing one of a few options.
/// </summary>
public class SegmentedControl : Control
{
	UISegmentedControl Ui => (UISegmentedControl)Native;


	/// <summary>
	/// The segment titles, in order.
	/// </summary>
	public IList<string> Items { get; } = [];

	/// <summary>
	/// The selected segment's index. Two-way by default.
	/// </summary>
	public Bindable<int> SelectedIndex
	{
		get => selectedIndex;
		set => selectedIndexBinding = Register(selectedIndexBinding, value, value => Set(ref selectedIndex, value, ApplySelection, affectsMeasure: false));
	}
	int selectedIndex;
	Binding<int>? selectedIndexBinding;

	/// <summary>
	/// Invoked with the new index whenever the user picks a segment.
	/// </summary>
	public Action<int>? SelectionChanged { get; set; }


	void ApplyItems()
	{
		Ui.RemoveAllSegments();

		for (int index = 0; index < Items.Count; index++)
			Ui.InsertSegment(Items[index], index, false);
	}

	void ApplySelection()
	{
		if (selectedIndex >= 0 && selectedIndex < Items.Count)
			Ui.SelectedSegment = selectedIndex;
	}

	void OnSelectionChanged()
	{
		int value = (int)Ui.SelectedSegment;

		Set(ref selectedIndex, value, affectsMeasure: false);
		selectedIndexBinding?.PushToSource(value);
		SelectionChanged?.Invoke(value);
	}


	private protected override UIView CreateNative()
	{
		UISegmentedControl control = new();
		control.ValueChanged += (_, _) => OnSelectionChanged();

		return control;
	}

	private protected override void ApplyProperties()
	{
		ApplyItems();
		ApplySelection();
	}
}
