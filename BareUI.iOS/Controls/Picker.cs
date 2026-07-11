#if IOS
using UIKit;
#endif

namespace BareUI;

/// <summary>
/// A menu-style selection button wrapping <c>UIButton</c> + <c>UIMenu</c>.
/// </summary>
public class Picker : Control
{
	/// <summary>
	/// The selectable items.
	/// </summary>
	public IReadOnlyList<string> Items { get; set; } = [];

	/// <summary>
	/// Index of the selected item, or -1 for none.
	/// </summary>
	public int SelectedIndex { get; set; } = -1;

	/// <summary>
	/// Text shown when no item is selected.
	/// </summary>
	public string? Placeholder { get; set; }

	/// <summary>
	/// Invoked with the new index whenever the user picks an item.
	/// </summary>
	public Action<int>? SelectionChanged { get; set; }

#if IOS
	private protected override UIView CreateNative()
	{
		UIButton button = new(UIButtonType.System)
		{
			Configuration = UIButtonConfiguration.GrayButtonConfiguration,
			ShowsMenuAsPrimaryAction = true,
			ChangesSelectionAsPrimaryAction = true
		};

		UIAction[] actions = new UIAction[Items.Count];
		for (int i = 0; i < Items.Count; i++)
		{
			int index = i;
			actions[i] = UIAction.Create(
				Items[index],
				null,
				null,
				action =>
				{
					SelectedIndex = index;
					SelectionChanged?.Invoke(index);
				});

			if (index == SelectedIndex)
				actions[i].State = UIMenuElementState.On;
		}

		button.Menu = UIMenu.Create(actions);
		button.SetTitle(
			SelectedIndex >= 0 && SelectedIndex < Items.Count ? Items[SelectedIndex] : Placeholder,
			UIControlState.Normal);

		return button;
	}
#endif
}
