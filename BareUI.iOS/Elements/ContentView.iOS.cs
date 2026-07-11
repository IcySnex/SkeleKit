using System.ComponentModel;
using UIKit;

namespace BareUI;

public abstract partial class ContentView
{
	internal PageHost? Host { get; set; }

	/// <summary>
	/// The hosting <c>UIViewController</c>. An escape hatch: app code should not need it.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public UIViewController? Controller =>
		Host;

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

		NotifyLoaded();
	}

	private protected override void OnUnrealized()
	{
		NotifyUnloaded();

		base.OnUnrealized();
	}
}
