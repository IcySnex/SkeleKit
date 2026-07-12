namespace BareUI;

/// <summary>
/// The model state an animation can move. An animation's two ends are snapshots of this.
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
	Thickness Margin)
{
	// unclamped, so a spring's overshoot carries through; only what lerps cleanly is interpolated —
	// brushes and layout stay at the start values until the animation settles on its exact end state
	public static ViewState Lerp(
		ViewState a,
		ViewState b,
		double t) =>
		a with
		{
			Translation = new(
				a.Translation.X + ((b.Translation.X - a.Translation.X) * t),
				a.Translation.Y + ((b.Translation.Y - a.Translation.Y) * t)),
			Scale = a.Scale + ((b.Scale - a.Scale) * t),
			Rotation = a.Rotation + ((b.Rotation - a.Rotation) * t),
			Opacity = Math.Clamp(a.Opacity + ((b.Opacity - a.Opacity) * t), 0, 1),
			CornerRadius = Math.Max(a.CornerRadius + ((b.CornerRadius - a.CornerRadius) * t), 0)
		};
}

/// <summary>
/// Records the views an animation's changes touch, and where they stood, so the animation knows both of its ends.
/// </summary>
/// <remarks>An animation's changes run on the main thread, like every layout path, so one scope at a time is enough.</remarks>
static class AnimationCapture
{
	static Dictionary<View, ViewState>? active;

	// the changes block writes the animation's *end* values into the model; the returned snapshots
	// hold each touched view's *start*, so a spring-back can run to real values instead of reversing
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
