namespace BareUI;

public abstract partial class ContentView
{
	internal PageHost? Host { get; set; }

	// the page's safe-area insets, so a view with IgnoresSafeArea knows how far it may bleed
	internal Thickness PageSafeArea { get; set; } = Thickness.Zero;

	partial void ApplyTitleCore() =>
		Host?.SetTitle(Title.Value);

	// a scrolling page bleeds vertically by default, so its content slides under the bars and they
	// blur over it. Never horizontally: nothing goes under the notch unless it asks to.
	private protected override void OnRealized()
	{
		if (ScrollsUnderBars
			&& Content is { IgnoresSafeArea: SafeAreaEdges.None } content
			&& content.Scrolls)
			content.IgnoresSafeArea = SafeAreaEdges.Top | SafeAreaEdges.Bottom;

		base.OnRealized();
	}
}
