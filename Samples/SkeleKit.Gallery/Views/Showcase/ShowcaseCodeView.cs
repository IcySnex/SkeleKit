namespace SkeleKit.Gallery.Views.Showcase;

internal sealed class ShowcaseCodeView : ContentView
{
	public ShowcaseCodeView(
		string title,
		IReadOnlyList<Span> spans,
		Color tint)
	{
		Title = title;
		Tint = tint;
		BarTint = tint;
		BackgroundStyle = PageBackground.Default;

		Content = new Border
		{
			Padding = 16,
			Background = Color.Dynamic(
				Color.FromHex(0xf9f9f9),
				Color.FromHex(0x202020)),

			Child = new TextView
			{
				Spans = [.. spans],
				IsSelectable = true,
				FontSize = 13,
				FontDesign = FontDesign.Monospaced,
				TextColor = Colors.Label,
				LineSpacing = 2
			}
		};
	}
}
