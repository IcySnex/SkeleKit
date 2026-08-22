namespace SkeleKit;

/// <summary>
/// How an animation's speed is distributed over its duration.
/// </summary>
public enum Easing
{
	/// <summary>
	/// Constant speed.
	/// </summary>
	Linear,

	/// <summary>
	/// Starts slow.
	/// </summary>
	EaseIn,

	/// <summary>
	/// Ends slow.
	/// </summary>
	EaseOut,

	/// <summary>
	/// Starts and ends slow.
	/// </summary>
	EaseInOut
}
