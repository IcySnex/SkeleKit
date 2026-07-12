namespace BareUI;

/// <summary>
/// How content is scaled to fill the space available to it.
/// </summary>
public enum Stretch
{
	/// <summary>
	/// Content keeps its natural size and is centered.
	/// </summary>
	None,

	/// <summary>
	/// Content is stretched on both axes to fill, ignoring aspect ratio.
	/// </summary>
	Fill,

	/// <summary>
	/// Content is scaled to fit while preserving aspect ratio (letterboxed).
	/// </summary>
	Uniform,

	/// <summary>
	/// Content is scaled to fill while preserving aspect ratio (cropped).
	/// </summary>
	UniformToFill
}
