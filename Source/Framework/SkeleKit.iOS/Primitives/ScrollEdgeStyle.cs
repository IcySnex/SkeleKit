namespace SkeleKit;

/// <summary>
/// The visual treatment where scrolling content meets an overlaid edge.
/// </summary>
public enum ScrollEdgeStyle
{
	/// <summary>
	/// The system chooses the edge treatment.
	/// </summary>
	Automatic,

	/// <summary>
	/// A soft edge that blends scrolling content beneath the overlay.
	/// </summary>
	Soft,

	/// <summary>
	/// A hard cutoff with a dividing line.
	/// </summary>
	Hard
}
