namespace BareUI;

/// <summary>
/// How the navigation bar shows the page's title.
/// </summary>
public enum TitleStyle
{
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
/// The page's background.
/// </summary>
public enum PageBackground
{
	/// <summary>
	/// The system background.
	/// </summary>
	Default,

	/// <summary>
	/// The grouped background, for settings-style pages.
	/// </summary>
	Grouped,

	/// <summary>
	/// No background at all.
	/// </summary>
	None
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
