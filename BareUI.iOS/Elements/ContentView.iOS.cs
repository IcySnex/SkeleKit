using System.ComponentModel;
using UIKit;

namespace BareUI;

public abstract partial class ContentView
{
	internal PageHost? Host { get; set; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public UIViewController? Controller =>
		Host;

	// the page's safe-area insets, so a view with IgnoresSafeArea knows how far it may bleed
	internal Thickness PageSafeArea { get; set; } = Thickness.Zero;

	partial void ApplyTitleCore() =>
		Host?.NavigationItem.Title = Title.Value;

	private protected override void OnRealized()
	{
		if (ScrollsUnderBars
			&& Content is { IgnoresSafeArea: SafeAreaEdges.None } content
			&& content.Scrolls)
			content.IgnoresSafeArea = SafeAreaEdges.Top | SafeAreaEdges.Bottom;

		base.OnRealized();

		NotifyLoaded();
	}

	private protected override void OnUnrealized()
	{
		NotifyUnloaded();

		base.OnUnrealized();
	}
}
