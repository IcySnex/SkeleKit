namespace BareUI;

// one bar per kind, shared app-wide: swapping fields keeps the same native instance, and the
// static root keeps the item peers alive
internal static class InputAccessory
{
	static UIToolbar? done;
	static UIToolbar? navigation;

	static readonly List<UIBarButtonItem> items = [];


	public static UIToolbar Bar(
		KeyboardToolbar kind) =>
		kind is KeyboardToolbar.Navigation
			? navigation ??= Build(arrows: true)
			: done ??= Build(arrows: false);

	static UIToolbar Build(
		bool arrows)
	{
		UIBarButtonItem done = new(UIBarButtonSystemItem.Done, (_, _) => Focused()?.Unfocus());

		UIBarButtonItem[] bar = arrows
			?
			[
				new(UIImage.GetSystemImage("chevron.up"), UIBarButtonItemStyle.Plain, (_, _) => Move(-1)),
				new(UIImage.GetSystemImage("chevron.down"), UIBarButtonItemStyle.Plain, (_, _) => Move(+1)),
				new(UIBarButtonSystemItem.FlexibleSpace),
				done
			]
			:
			[
				new(UIBarButtonSystemItem.FlexibleSpace),
				done
			];

		items.AddRange(bar);

		UIToolbar toolbar = new() { Items = bar };
		toolbar.SizeToFit();

		// the press effect scales the glass outside the fitted bounds, which the container clips
		toolbar.Frame = new(0, 0, toolbar.Frame.Width, toolbar.Frame.Height + 10);

		return toolbar;
	}

	// one host per view, so fields sharing an accessory share the native instance too
	static readonly Dictionary<View, AccessoryHost> hosts = [];

	public static AccessoryHost Host(
		View view)
	{
		if (!hosts.TryGetValue(view, out AccessoryHost? host))
			hosts[view] = host = new(view);

		return host;
	}


	static void Move(
		int direction)
	{
		List<View> inputs = Inputs();

		int index = inputs.FindIndex(static input => input.IsFocused);
		if (index < 0)
			return;

		int target = index + direction;

		if (target >= 0 && target < inputs.Count)
			inputs[target].Focus();
	}

	static View? Focused() =>
		Inputs().Find(static input => input.IsFocused);

	static List<View> Inputs()
	{
		List<View> inputs = [];

		if (BareApplication.TopPage() is { } page)
			Collect(page, inputs);

		return inputs;
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
