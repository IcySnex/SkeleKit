namespace SkeleKit;

/// <summary>
/// A single moment in a custom haptic pattern played through <see cref="IHaptics.Play"/>.
/// </summary>
/// <remarks>
/// Intensity and sharpness range from 0 to 1 and are clamped. Intensity controls how strong the sensation feels, sharpness how crisp against dull.
/// </remarks>
public readonly struct HapticEvent
{
	HapticEvent(
		bool isContinuous,
		double time,
		double duration,
		double intensity,
		double sharpness)
	{
		IsContinuous = isContinuous;
		Time = Math.Max(0, time);
		Duration = Math.Max(0, duration);
		Intensity = (float)Math.Clamp(intensity, 0, 1);
		Sharpness = (float)Math.Clamp(sharpness, 0, 1);
	}


	internal readonly bool IsContinuous;

	internal readonly double Time;

	internal readonly double Duration;

	internal readonly float Intensity;

	internal readonly float Sharpness;


	/// <summary>
	/// Creates a brief, punctuated tap at the given time in the pattern.
	/// </summary>
	/// <param name="time">Seconds from the start of the pattern.</param>
	/// <param name="intensity">Strength of the sensation, from 0 to 1.</param>
	/// <param name="sharpness">Crispness of the sensation, from 0 to 1.</param>
	/// <returns>The event.</returns>
	public static HapticEvent Tap(
		double time,
		double intensity = 1,
		double sharpness = 0.5) =>
		new(false, time, 0, intensity, sharpness);

	/// <summary>
	/// Creates a sustained vibration spanning the given duration.
	/// </summary>
	/// <param name="time">Seconds from the start of the pattern.</param>
	/// <param name="duration">How long the vibration lasts, in seconds.</param>
	/// <param name="intensity">Strength of the sensation, from 0 to 1.</param>
	/// <param name="sharpness">Crispness of the sensation, from 0 to 1.</param>
	/// <returns>The event.</returns>
	public static HapticEvent Continuous(
		double time,
		double duration,
		double intensity = 1,
		double sharpness = 0.5) =>
		new(true, time, duration, intensity, sharpness);
}
