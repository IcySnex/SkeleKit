namespace SkeleKit;

/// <summary>
/// Provides native device haptic feedback.
/// </summary>
public interface IHaptics
{
	/// <summary>
	/// Triggers impact feedback to simulate physical weight or collisions.
	/// </summary>
	/// <param name="style">The weight profile of the impact sensation.</param>
	void Impact(
		HapticStyle style = HapticStyle.Medium);

	/// <summary>
	/// Triggers subtle feedback indicating a user selection change.
	/// </summary>
	void Selection();

	/// <summary>
	/// Triggers notification feedback for successes, warnings, or errors.
	/// </summary>
	/// <param name="notification">The event type being signaled.</param>
	void Notify(
		HapticsNotification notification);

	/// <summary>
	/// Plays a custom haptic pattern built from taps and sustained vibrations.
	/// </summary>
	/// <param name="events">The events making up the pattern, timed from its start.</param>
	void Play(
		params ReadOnlySpan<HapticEvent> events);
}
