using UIKit;

namespace BareUI;

/// <summary>
/// A menu-style selection button wrapping <c>UIButton</c> + <c>UIMenu</c>.
/// </summary>
public class Picker<TItem> : Control
	where TItem : class
{
	/// <summary>
	/// The selectable items. Interface-typed, so a literal needs <c>Bindable.From(...)</c>.
	/// </summary>
	public Bindable<IReadOnlyList<TItem>?> ItemsSource
	{
		get => Bindable.From<IReadOnlyList<TItem>?>(items);
		set => itemsBinding = Register(itemsBinding, value, value => Set(ref items, value ?? [], ApplyMenu));
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
	/// How an item is labelled in the menu. Defaults to <c>ToString()</c>.
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


	private protected override UIView CreateNative() =>
		new UIButton(UIButtonType.System)
		{
			Configuration = UIButtonConfiguration.GrayButtonConfiguration,
			ShowsMenuAsPrimaryAction = true,
			ChangesSelectionAsPrimaryAction = true
		};

	private protected override void ApplyProperties() =>
		ApplyMenu();

	UIButton Ui =>
		(UIButton)Native;

	void ApplyMenu()
	{
		UIAction[] actions = new UIAction[items.Count];

		for (int index = 0; index < items.Count; index++)
		{
			TItem item = items[index];

			actions[index] = UIAction.Create(ItemTitle(item), null, null, action => OnSelected(item));

			if (ReferenceEquals(item, selected))
				actions[index].State = UIMenuElementState.On;
		}

		Ui.Menu = UIMenu.Create(actions);
		Ui.SetTitle(selected is { } current ? ItemTitle(current) : placeholder, UIControlState.Normal);
	}

	void OnSelected(
		TItem item)
	{
		Set(ref selected, item, ApplyMenu, affectsMeasure: false);

		selectedBinding?.PushToSource(item);
		SelectionChanged?.Invoke(item);
	}
}
