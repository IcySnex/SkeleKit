using UIKit;

namespace BareUI;

/// <summary>
/// A menu-style selection button wrapping <c>UIButton</c> + <c>UIMenu</c>.
/// </summary>
public class Picker : Control
{
	/// <summary>
	/// The selectable items. Not bindable: C# forbids implicit conversions from interface types.
	/// </summary>
	public IReadOnlyList<string> Items
	{
		get => items;
		set => Set(ref items, value, ApplyMenu);
	}
	IReadOnlyList<string> items = [];

	/// <summary>
	/// Index of the selected item, or -1 for none.
	/// </summary>
	public Bindable<int> SelectedIndex
	{
		get => selectedIndex;
		set => selectedIndexBinding = Register(selectedIndexBinding, value, value => Set(ref selectedIndex, value, ApplyMenu));
	}
	int selectedIndex = -1;
	Binding<int>? selectedIndexBinding;

	/// <summary>
	/// Text shown when no item is selected.
	/// </summary>
	public Bindable<string?> Placeholder
	{
		get => placeholder;
		set => placeholderBinding = Register(placeholderBinding, value, value => Set(ref placeholder, value, ApplyMenu));
	}
	string? placeholder;
	Binding<string?>? placeholderBinding;

	/// <summary>
	/// Invoked with the new index whenever the user picks an item.
	/// </summary>
	public Action<int>? SelectionChanged { get; set; }


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

		for (int i = 0; i < items.Count; i++)
		{
			int index = i;

			actions[index] = UIAction.Create(items[index], null, null, action => OnSelected(index));

			if (index == selectedIndex)
				actions[index].State = UIMenuElementState.On;
		}

		Ui.Menu = UIMenu.Create(actions);
		Ui.SetTitle(
			selectedIndex >= 0 && selectedIndex < items.Count ? items[selectedIndex] : placeholder,
			UIControlState.Normal);
	}

	void OnSelected(
		int index)
	{
		Set(ref selectedIndex, index, affectsMeasure: false);
		selectedIndexBinding?.PushToSource(index);
		SelectionChanged?.Invoke(index);
	}
}
