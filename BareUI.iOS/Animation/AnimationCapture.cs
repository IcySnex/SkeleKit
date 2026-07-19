namespace BareUI;

internal static class AnimationCapture
{
	static Dictionary<View, ViewState>? active;


	public static Dictionary<View, ViewState> Run(
		Action changes)
	{
		Dictionary<View, ViewState> states = [];

		Dictionary<View, ViewState>? outer = active;
		active = states;

		try
		{
			changes();
		}
		finally
		{
			active = outer;
		}

		return states;
	}

	public static void Record(
		View view)
	{
		if (active is not null && !active.ContainsKey(view))
			active[view] = view.Capture();
	}
}
