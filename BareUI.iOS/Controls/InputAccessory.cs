using CoreGraphics;

namespace BareUI;

// the control roots this: UIKit's retain alone would let the item peers die
internal sealed class InputAccessory
{
	readonly UIBarButtonItem[] items;

	public UIToolbar Bar { get; }


	public InputAccessory(
		Control owner,
		KeyboardToolbar kind)
	{
		UIBarButtonItem done = new(UIBarButtonSystemItem.Done, (_, _) => owner.Unfocus());

		items = kind is KeyboardToolbar.Navigation
			?
			[
				new(UIImage.GetSystemImage("chevron.up"), UIBarButtonItemStyle.Plain, (_, _) => Move(owner, -1)),
				new(UIImage.GetSystemImage("chevron.down"), UIBarButtonItemStyle.Plain, (_, _) => Move(owner, +1)),
				new(UIBarButtonSystemItem.FlexibleSpace),
				done
			]
			:
			[
				new(UIBarButtonSystemItem.FlexibleSpace),
				done
			];

		Bar = new(new CGRect(0, 0, 0, 44)) { Items = items };
		Bar.SizeToFit();
	}


	static void Move(
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
