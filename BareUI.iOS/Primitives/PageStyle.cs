namespace BareUI;

/// <summary>
/// How the navigation bar shows the page's title.
/// </summary>
public enum TitleStyle
{
	/// <summary>The standard inline title.</summary>
	Inline,

	/// <summary>A large title that collapses to inline as the content scrolls.</summary>
	Large
}

/// <summary>
/// The page's background.
/// </summary>
public enum PageBackground
{
	/// <summary>The system background.</summary>
	Default,

	/// <summary>The grouped background, for settings-style pages.</summary>
	Grouped,

	/// <summary>No background at all.</summary>
	None
}
