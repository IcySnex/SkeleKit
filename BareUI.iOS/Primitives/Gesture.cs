namespace BareUI;

/// <summary>
/// Where a continuous gesture is in its lifetime.
/// </summary>
public enum GestureState
{
	/// <summary>
	/// The finger went down and the gesture was recognized.
	/// </summary>
	Began,

	/// <summary>
	/// The finger moved.
	/// </summary>
	Changed,

	/// <summary>
	/// The finger lifted.
	/// </summary>
	Ended,

	/// <summary>
	/// The system took the gesture away.
	/// </summary>
	Canceled
}

/// <summary>
/// One update of a drag: how far it has moved from where it started, and how fast it is going.
/// </summary>
/// <param name="State">The current execution state of the gesture lifecycle.</param>
/// <param name="Translation">The cumulative distance moved from the start position.</param>
/// <param name="Velocity">The current speed and direction of the movement.</param>
public readonly record struct PanGesture(
	GestureState State,
	Point Translation,
	Point Velocity);

/// <summary>
/// One update of a pinch: the factor the touched distance has scaled by since the gesture began.
/// </summary>
/// <param name="State">The current execution state of the gesture lifecycle.</param>
/// <param name="Scale">The cumulative scale factor, 1 at the start.</param>
/// <param name="Velocity">The scale change per second.</param>
public readonly record struct PinchGesture(
	GestureState State,
	double Scale,
	double Velocity);

/// <summary>
/// One update of a two-finger rotation, in degrees since the gesture began.
/// </summary>
/// <param name="State">The current execution state of the gesture lifecycle.</param>
/// <param name="Degrees">The cumulative rotation, clockwise positive.</param>
/// <param name="Velocity">The rotation change in degrees per second.</param>
public readonly record struct RotateGesture(
	GestureState State,
	double Degrees,
	double Velocity);
