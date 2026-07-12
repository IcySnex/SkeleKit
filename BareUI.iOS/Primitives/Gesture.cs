namespace BareUI;

/// <summary>
/// Where a continuous gesture is in its lifetime.
/// </summary>
public enum GestureState
{
	/// <summary>The finger went down and the gesture was recognized.</summary>
	Began,

	/// <summary>The finger moved.</summary>
	Changed,

	/// <summary>The finger lifted.</summary>
	Ended,

	/// <summary>The system took the gesture away.</summary>
	Cancelled
}

/// <summary>
/// One update of a drag: how far it has moved from where it started, and how fast it is going.
/// </summary>
public readonly record struct PanGesture(
	GestureState State,
	Point Translation,
	Point Velocity);
