using System.Collections.Specialized;

namespace SkeleKit;

/// <summary>
/// A menu-style selection button wrapping <c>UIButton</c> + <c>UIMenu</c>.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class Picker<TItem> : Control
	where TItem : class
{
	bool hooked;


	UIButton Ui => (UIButton)Native;


	/// <summary>
	/// The selectable items.
	/// </summary>
	/// <remarks>
	/// Live when the list is an <c>ObservableCollection</c>.
	/// </remarks>
	public BindableList<TItem> ItemsSource
	{
		get => new(items);
		set => itemsBinding = Register(itemsBinding, value.Expression, value.Value, SetItems);
	}
	IReadOnlyList<TItem> items = [];
	Binding<IReadOnlyList<TItem>?>? itemsBinding;

	/// <summary>
	/// The selected item, or null for none.
	/// </summary>
	public Bindable<TItem?> SelectedItem
	{
		get => selected;
		set => selectedBinding = Register(selectedBinding, value, value => Set(ref selected, value, ApplyMenu));
	}
	TItem? selected;
	Binding<TItem?>? selectedBinding;

	/// <summary>
	/// How an item is labeled in the menu. Defaults to <c>ToString()</c>.
	/// </summary>
	public Func<TItem, string> ItemTitle { get; set; } = item => item.ToString() ?? "";

	/// <summary>
	/// Text shown when nothing is selected.
	/// </summary>
	public Bindable<string?> Placeholder
	{
		get => placeholder;
		set => placeholderBinding = Register(placeholderBinding, value, value => Set(ref placeholder, value, ApplyMenu));
	}
	string? placeholder;
	Binding<string?>? placeholderBinding;

	/// <summary>
	/// Invoked with the newly selected item.
	/// </summary>
	public Action<TItem>? SelectionChanged { get; set; }


	void SetItems(
		IReadOnlyList<TItem>? value)
	{
		IReadOnlyList<TItem> next = value ?? [];
		if (ReferenceEquals(items, next))
			return;

		if (hooked && items is INotifyCollectionChanged old)
			old.CollectionChanged -= OnItemsChanged;

		items = next;

		if (hooked && items is INotifyCollectionChanged live)
			live.CollectionChanged += OnItemsChanged;

		if (IsRealized)
			ApplyMenu();

		InvalidateMeasure();
	}

	void HookItems()
	{
		if (hooked)
			return;

		hooked = true;

		if (items is INotifyCollectionChanged live)
			live.CollectionChanged += OnItemsChanged;
	}

	void UnhookItems()
	{
		if (!hooked)
			return;

		if (items is INotifyCollectionChanged live)
			live.CollectionChanged -= OnItemsChanged;

		hooked = false;
	}

	void OnItemsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs args)
	{
		ApplyMenu();
		InvalidateMeasure();
	}

	void ApplyMenu()
	{
		UIAction[] actions = new UIAction[items.Count];

		for (int index = 0; index < items.Count; index++)
		{
			TItem item = items[index];

			actions[index] = UIAction.Create(ItemTitle(item), null, null, _ => OnSelected(item));

			if (ReferenceEquals(item, selected))
				actions[index].State = UIMenuElementState.On;
		}

		Ui.Menu = UIMenu.Create(actions);
		Ui.SetTitle(selected is TItem current ? ItemTitle(current) : placeholder, UIControlState.Normal);
	}

	void OnSelected(
		TItem item)
	{
		Set(ref selected, item, ApplyMenu);

		selectedBinding?.PushToSource(item);
		SelectionChanged?.Invoke(item);
	}


	private protected override UIView CreateNative() =>
		new UIButton(UIButtonType.System)
		{
			Configuration = UIButtonConfiguration.GrayButtonConfiguration,
			ShowsMenuAsPrimaryAction = true
		};

	private protected override void ApplyProperties()
	{
		HookItems();
		ApplyMenu();
	}

	private protected override void OnUnrealized() =>
		UnhookItems();
}
