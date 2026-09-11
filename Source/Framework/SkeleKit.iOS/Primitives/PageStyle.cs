namespace SkeleKit;

/// <summary>
/// How the navigation bar shows the page's title.
/// </summary>
public enum TitleStyle
{

	/// <summary>
	/// Follows the navigation stack's large-title preference.
	/// </summary>
	Automatic,
	
	/// <summary>
	/// The standard inline title.
	/// </summary>
	Inline,

	/// <summary>
	/// A large title that collapses to inline as the content scrolls.
	/// </summary>
	Large
}

/// <summary>
/// How the next pushed page's back button represents this page.
/// </summary>
public enum BackButtonStyle
{
	/// <summary>
	/// The page title, shortened to "Back" when space runs out.
	/// </summary>
	Default,

	/// <summary>
	/// Always the generic "Back", never the title.
	/// </summary>
	Generic,

	/// <summary>
	/// The chevron alone.
	/// </summary>
	Minimal
}

/// <summary>
/// The status bar look a page asks for.
/// </summary>
public enum StatusBarStyle
{
	/// <summary>
	/// Follows the system appearance.
	/// </summary>
	Default,

	/// <summary>
	/// White content, for dark page backgrounds.
	/// </summary>
	Light,

	/// <summary>
	/// Black content, for light page backgrounds.
	/// </summary>
	Dark
}
