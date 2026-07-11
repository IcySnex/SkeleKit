namespace BareUI;

public abstract partial class ContentView
{
	internal PageHost? Host { get; set; }

	// the page's safe-area insets, so a view with IgnoresSafeArea knows how far it may bleed
	internal Thickness PageSafeArea { get; set; } = Thickness.Zero;

	partial void ApplyTitleCore() =>
		Host?.SetTitle(Title.Value);
}
