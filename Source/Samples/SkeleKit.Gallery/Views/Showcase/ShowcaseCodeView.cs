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

		Content = new ScrollView
		{
			Background = Color.Dynamic(
				Color.FromHex(0xf9f9f9),
				Color.FromHex(0x202020)),

			Content = new ScrollView
			{
				Orientation = Orientation.Horizontal,
				Padding = 16,
				Content = ShowcaseBox.CodeText([.. CSharpSyntax.Highlight(spans)])
			}
		};
	}
}
