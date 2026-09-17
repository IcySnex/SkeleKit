namespace SkeleKit;

/// <summary>
/// Configures adaptive sidebar placements and destinations.
/// </summary>
public sealed class SidebarBuilder
{
	internal Func<View>? FooterFactory { get; private set; }

	/// <summary>
	/// Shows a view of the given type at the sidebar's bottom. iOS 26 and later.
	/// </summary>
	/// <typeparam name="TView">The view type to host.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public SidebarBuilder Footer<TView>()
		where TView : View, new()
	{
		FooterFactory = () => new TView();

		return this;
	}
}
