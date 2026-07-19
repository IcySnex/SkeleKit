namespace BareUI;

internal readonly record struct ViewState(
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
	public static ViewState Lerp(
		ViewState a,
		ViewState b,
		double t) =>
		new()
		{
			Translation = new(
				a.Translation.X + (b.Translation.X - a.Translation.X) * t,
				a.Translation.Y + (b.Translation.Y - a.Translation.Y) * t),
			Scale = a.Scale + (b.Scale - a.Scale) * t,
			Rotation = a.Rotation + (b.Rotation - a.Rotation) * t,
			Opacity = Math.Clamp(a.Opacity + (b.Opacity - a.Opacity) * t, 0, 1),
			CornerRadius = Math.Max(a.CornerRadius + (b.CornerRadius - a.CornerRadius) * t, 0),
			Background = Brush.Lerp(a.Background, b.Background, t) ?? a.Background,
			Width = LerpLength(a.Width, b.Width, t),
			Height = LerpLength(a.Height, b.Height, t),
			Margin = new(
				a.Margin.Left + (b.Margin.Left - a.Margin.Left) * t,
				a.Margin.Top + (b.Margin.Top - a.Margin.Top) * t,
				a.Margin.Right + (b.Margin.Right - a.Margin.Right) * t,
				a.Margin.Bottom + (b.Margin.Bottom - a.Margin.Bottom) * t)
		};

	
	static double LerpLength(
		double from,
		double to,
		double t) =>
		double.IsNaN(from) || double.IsNaN(to)
			? from
			: Math.Max(from + (to - from) * t, 0);
}
