namespace BareUI;

/// <summary>
/// The model state an animation can move. Snapshotted before an animation runs, so a reverted one can be undone.
/// </summary>
readonly record struct ViewState(
	Point Translation,
	double Scale,
	double Rotation,
	double Opacity,
	double CornerRadius,
	Brush? Background,
	double Width,
	double Height,
	Thickness Margin);

/// <summary>
/// Records the views an animation's changes touch, so the model can be put back if UIKit reverts them.
/// </summary>
/// <remarks>An animation's changes run on the main thread, like every layout path, so one scope at a time is enough.</remarks>
static class AnimationCapture
{
	static Dictionary<View, ViewState>? active;

	// the changes block writes the animation's *end* values into the model. UIKit animates the native
	// side and, on a reversed animation, silently puts it back — leaving the model a value ahead.
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
		if (active is { } states && !states.ContainsKey(view))
			states[view] = view.Capture();
	}
}
