namespace SkeleKit;

internal enum ThemeField
{
	Tint,
	Appearance,
	TopScrollEdgeStyle,
	NavigationTitleStyle,
	TabBarMinimize
}

/// <summary>
/// The app-wide theme inherited by windows, chrome and pages unless a view overrides it.
/// </summary>
public sealed class Theme
{
	internal event Action<ThemeField>? Changed;


	/// <summary>
	/// The app-wide tint inherited by windows, chrome and views, or null for the system default.
	/// </summary>
	public Color? Tint
	{
		get;
		set => Set(ref field, value, ThemeField.Tint);
	}

	/// <summary>
	/// The app-wide light or dark appearance.
	/// </summary>
	public Appearance Appearance
	{
		get;
		set => Set(ref field, value, ThemeField.Appearance);
	} = Appearance.System;

	/// <summary>
	/// The default treatment where scrolling content meets a page's top chrome.
	/// </summary>
	/// <remarks>
	/// Pages inherit it unless they set their own <see cref="ContentView.TopScrollEdgeStyle"/>.
	/// iOS 26 and later.
	/// </remarks>
	public ScrollEdgeStyle TopScrollEdgeStyle
	{
		get;
		set => Set(ref field, value, ThemeField.TopScrollEdgeStyle);
	} = ScrollEdgeStyle.Automatic;

	/// <summary>
	/// How navigation titles are displayed by default.
	/// </summary>
	/// <remarks>
	/// Pages inherit it unless they set their own <see cref="ContentView.NavigationTitleStyle"/>.
	/// </remarks>
	public TitleStyle NavigationTitleStyle
	{
		get;
		set => Set(ref field, value, ThemeField.NavigationTitleStyle);
	} = TitleStyle.Inline;

	/// <summary>
	/// When the app's tab bar minimizes as the selected content scrolls.
	/// </summary>
	/// <remarks>
	/// Applies to tab shells on iOS 26 and later.
	/// </remarks>
	public TabBarMinimize TabBarMinimize
	{
		get;
		set => Set(ref field, value, ThemeField.TabBarMinimize);
	} = TabBarMinimize.Never;


	void Set<T>(
		ref T field,
		T value,
		ThemeField changed)
	{
		if (EqualityComparer<T>.Default.Equals(field, value))
			return;

		field = value;
		Changed?.Invoke(changed);
	}
}
