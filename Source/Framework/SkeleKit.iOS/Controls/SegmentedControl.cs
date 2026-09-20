using System.Collections.Specialized;

namespace SkeleKit;

/// <summary>
/// A segmented control choosing one of a few options.
/// </summary>
public class SegmentedControl : Control
{
	UISegmentedControl Ui => (UISegmentedControl)Native;
	bool itemsHooked;


	/// <summary>
	/// The segment titles, in order.
	/// </summary>
	public BindableList<string> Items
	{
		get => new(items);
		set => itemsBinding = Register(itemsBinding, value.Expression, value.Value, SetItems);
	}
	IReadOnlyList<string> items = new List<string>();
	Binding<IReadOnlyList<string>?>? itemsBinding;

	/// <summary>
	/// The selected segment's index.
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


	void SetItems(
		IReadOnlyList<string>? value)
	{
		if (itemsHooked && items is INotifyCollectionChanged old)
			old.CollectionChanged -= OnItemsChanged;

		items = value ?? [];

		if (itemsHooked && items is INotifyCollectionChanged live)
			live.CollectionChanged += OnItemsChanged;

		if (IsRealized)
		{
			ApplyItems();
			ApplySelection();
		}

		InvalidateMeasure();
	}

	void HookItems()
	{
		if (itemsHooked)
			return;

		itemsHooked = true;
		if (items is INotifyCollectionChanged live)
			live.CollectionChanged += OnItemsChanged;
	}

	void UnhookItems()
	{
		if (!itemsHooked)
			return;

		if (items is INotifyCollectionChanged live)
			live.CollectionChanged -= OnItemsChanged;

		itemsHooked = false;
	}

	void OnItemsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs args)
	{
		ApplyItems();
		ApplySelection();
		InvalidateMeasure();
	}

	void ApplyItems()
	{
		Ui.RemoveAllSegments();

		for (int index = 0; index < items.Count; index++)
			Ui.InsertSegment(items[index], index, false);
	}

	void ApplySelection()
	{
		if (selectedIndex >= 0 && selectedIndex < items.Count)
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
		HookItems();
		ApplyItems();
		ApplySelection();
	}

	private protected override void OnUnrealized() =>
		UnhookItems();
}
