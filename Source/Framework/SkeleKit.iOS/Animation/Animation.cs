namespace SkeleKit;

/// <summary>
/// How to animate a change: a duration with an easing curve, or a spring.
/// </summary>
/// <remarks>
/// Describes the timing only, never what changes.
/// </remarks>
public readonly record struct Animation
{
	/// <summary>
	/// The default: 0.3 seconds, eased in and out.
	/// </summary>
	public static Animation Default =>
		new();

	/// <summary>
	/// An animation of <paramref name="duration"/> seconds following a curve.
	/// </summary>
	/// <param name="duration">The running time of the animation in seconds.</param>
	/// <param name="easing">The speed distribution curve over the timeline.</param>
	/// <returns>A new animation configuration.</returns>
	public static Animation Ease(
		double duration,
		Easing easing = Easing.EaseInOut) =>
		new()
		{
			Duration = duration,
			Easing = easing
		};

	/// <summary>
	/// A spring that settles over <paramref name="duration"/> seconds; lower <paramref name="damping"/> bounces more.
	/// </summary>
	/// <param name="duration">The settlement time of the spring in seconds.</param>
	/// <param name="damping">The resistance factor from 0.0 to 1.0.</param>
	/// <returns>A new spring-based animation configuration.</returns>
	public static Animation Spring(
		double duration = 0.5,
		double damping = 0.8) =>
		new()
		{
			Duration = duration,
			SpringDamping = damping
		};


	/// <summary>
	/// Creates the default animation: 0.3 seconds, eased in and out.
	/// </summary>
	public Animation()
	{ }


	/// <summary>
	/// How long the animation runs, in seconds.
	/// </summary>
	public double Duration { get; init; } = 0.3;

	/// <summary>
	/// How long to wait before it starts, in seconds.
	/// </summary>
	public double Delay { get; init; }

	/// <summary>
	/// The curve the animation follows. Ignored when <see cref="SpringDamping"/> is set.
	/// </summary>
	public Easing Easing { get; init; } = Easing.EaseInOut;

	/// <summary>
	/// The damping of a spring, from 0 (bounces forever) to 1 (settles without overshoot), or null for a curve instead.
	/// </summary>
	public double? SpringDamping { get; init; }


	/// <summary>
	/// The same animation, started <paramref name="seconds"/> later.
	/// </summary>
	/// <param name="seconds">The delay timing offset in seconds.</param>
	/// <returns>A copy of this animation with the updated start delay.</returns>
	public Animation After(
		double seconds) =>
		this with { Delay = seconds };
}
