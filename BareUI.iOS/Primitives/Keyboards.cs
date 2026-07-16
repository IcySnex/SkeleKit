using UIKit;

namespace BareUI;

internal static class Keyboards
{
	public static UIKeyboardAppearance Appearance(
		KeyboardLook look) =>
		look switch
		{
			KeyboardLook.Light => UIKeyboardAppearance.Light,
			KeyboardLook.Dark => UIKeyboardAppearance.Dark,
			_ => UIKeyboardAppearance.Default
		};

	// the control roots both returns: UIKit's retain alone would let the item peers die
	public static (UIToolbar Bar, UIBarButtonItem[] Items) Toolbar(
		Control owner,
		KeyboardToolbar kind)
	{
		UIBarButtonItem done = new(UIBarButtonSystemItem.Done, (_, _) => owner.Unfocus());

		UIBarButtonItem[] items = kind is KeyboardToolbar.Navigation
			?
			[
				new(UIImage.GetSystemImage("chevron.up"), UIBarButtonItemStyle.Plain, (_, _) => MoveFocus(owner, -1)),
				new(UIImage.GetSystemImage("chevron.down"), UIBarButtonItemStyle.Plain, (_, _) => MoveFocus(owner, +1)),
				new(UIBarButtonSystemItem.FlexibleSpace),
				done
			]
			:
			[
				new(UIBarButtonSystemItem.FlexibleSpace),
				done
			];

		UIToolbar bar = new() { Items = items };
		bar.SizeToFit();

		// the press effect scales the glass outside the fitted bounds, which the container clips
		bar.Frame = new(0, 0, bar.Frame.Width, bar.Frame.Height + 10);

		return (bar, items);
	}

	static void MoveFocus(
		Control owner,
		int direction)
	{
		View root = owner;
		while (root.Parent is { } parent)
			root = parent;

		List<View> inputs = [];
		Collect(root, inputs);

		int target = inputs.IndexOf(owner) + direction;

		if (target >= 0 && target < inputs.Count)
			inputs[target].Focus();
	}

	static void Collect(
		View view,
		List<View> inputs)
	{
		if (view is TextField or TextEditor)
		{
			inputs.Add(view);
			return;
		}

		if (view is Panel panel)
			foreach (View child in panel.Children)
				Collect(child, inputs);
	}
}
