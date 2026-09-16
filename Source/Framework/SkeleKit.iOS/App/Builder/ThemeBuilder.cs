namespace SkeleKit;

/// <summary>
/// Configures the app-wide theme.
/// </summary>
public sealed class ThemeBuilder
{
	internal Theme Theme { get; } = new();


	/// <summary>
	/// Sets the app-wide tint inherited by windows, chrome and views.
	/// </summary>
	/// <param name="tint">The tint color, or null for the system default.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public ThemeBuilder Tint(
		Color? tint)
	{
		Theme.Tint = tint;
		return this;
	}

	/// <summary>
	/// Sets the app-wide light or dark appearance.
	/// </summary>
	/// <param name="appearance">The appearance.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public ThemeBuilder Appearance(
		Appearance appearance)
	{
		Theme.Appearance = appearance;
		return this;
	}

	/// <summary>
	/// Sets the default treatment where scrolling content meets a page's top chrome.
	/// </summary>
	/// <remarks>
	/// iOS 26 and later.
	/// </remarks>
	/// <param name="topScrollEdgeStyle">The inherited scroll edge style.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public ThemeBuilder TopScrollEdgeStyle(
		ScrollEdgeStyle topScrollEdgeStyle)
	{
		Theme.TopScrollEdgeStyle = topScrollEdgeStyle;
		return this;
	}

	/// <summary>
	/// Sets how navigation titles are displayed by default.
	/// </summary>
	/// <param name="navigationTitleStyle">The inherited title style.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public ThemeBuilder NavigationTitleStyle(
		TitleStyle navigationTitleStyle)
	{
		Theme.NavigationTitleStyle = navigationTitleStyle;
		return this;
	}

	/// <summary>
	/// Lets the tab bar minimize as the content scrolls.
	/// </summary>
	/// <remarks>
	/// iOS 26 and later.
	/// </remarks>
	/// <param name="tabBarMinimize">When the bar minimizes.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public ThemeBuilder TabBarMinimize(
		TabBarMinimize tabBarMinimize = SkeleKit.TabBarMinimize.OnScrollDown)
	{
		Theme.TabBarMinimize = tabBarMinimize;
		return this;
	}
}
