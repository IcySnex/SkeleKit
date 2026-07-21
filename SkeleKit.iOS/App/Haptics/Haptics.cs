using CoreHaptics;

namespace SkeleKit;

/// <summary>
/// Provides access to native device haptic feedback.
/// </summary>
public static class Haptics
{
	static CHHapticEngine? engine;

	static UIWindow? Anchor() =>
		UIApplication.SharedApplication
			.ConnectedScenes
			.OfType<UIWindowScene>()
			.SelectMany(scene => scene.Windows)
			.FirstOrDefault(window => window.IsKeyWindow);

	static CHHapticEngine? SharedEngine()
	{
		if (CHHapticEngine.GetHardwareCapabilities().SupportsHaptics is false)
			return null;

		if (engine is not null)
			return engine;

		CHHapticEngine created = new(out NSError? error);
		if (error is not null)
			return null;

		created.AutoShutdownEnabled = true;
		created.ResetHandler = () => created.Start(out _);

		engine = created;
		return engine;
	}


	/// <summary>
	/// Triggers impact feedback to simulate physical weight or collisions.
	/// </summary>
	/// <param name="style">The weight profile of the impact sensation.</param>
	public static void Impact(
		HapticStyle style = HapticStyle.Medium)
	{
		if (Anchor() is not UIWindow anchor)
			return;

		UIImpactFeedbackStyle type = style switch
		{
			HapticStyle.Light => UIImpactFeedbackStyle.Light,
			HapticStyle.Heavy => UIImpactFeedbackStyle.Heavy,
			HapticStyle.Soft => UIImpactFeedbackStyle.Soft,
			HapticStyle.Rigid => UIImpactFeedbackStyle.Rigid,
			_ => UIImpactFeedbackStyle.Medium
		};

		using UIImpactFeedbackGenerator generator = UIImpactFeedbackGenerator.GetFeedbackGenerator(type, anchor);

		generator.Prepare();
		generator.ImpactOccurred();
	}

	/// <summary>
	/// Triggers subtle feedback indicating a user selection change.
	/// </summary>
	public static void Selection()
	{
		if (Anchor() is not UIWindow anchor)
			return;

		using UISelectionFeedbackGenerator generator = UISelectionFeedbackGenerator.GetFeedbackGenerator(anchor);

		generator.Prepare();
		generator.SelectionChanged();
	}

	/// <summary>
	/// Triggers notification feedback for successes, warnings, or errors.
	/// </summary>
	/// <param name="notification">The event type being signaled.</param>
	public static void Notify(
		HapticsNotification notification)
	{
		if (Anchor() is not UIWindow anchor)
			return;

		UINotificationFeedbackType type = notification switch
		{
			HapticsNotification.Warning => UINotificationFeedbackType.Warning,
			HapticsNotification.Error => UINotificationFeedbackType.Error,
			_ => UINotificationFeedbackType.Success
		};

		using UINotificationFeedbackGenerator generator = UINotificationFeedbackGenerator.GetFeedbackGenerator(anchor);

		generator.Prepare();
		generator.NotificationOccurred(type);
	}

	/// <summary>
	/// Plays a custom haptic pattern built from taps and sustained vibrations.
	/// </summary>
	/// <remarks>
	/// Does nothing on hardware without a haptic engine, including the simulator.
	/// </remarks>
	/// <param name="events">The events making up the pattern, timed from its start.</param>
	public static void Play(
		params ReadOnlySpan<HapticEvent> events)
	{
		if (events.Length == 0)
			return;

		if (SharedEngine() is not CHHapticEngine engine)
			return;

		CHHapticEvent[] native = new CHHapticEvent[events.Length];
		for (int i = 0; i < events.Length; i++)
		{
			HapticEvent element = events[i];

			CHHapticEventParameter[] parameters =
			[
				new(CHHapticEventParameterId.HapticIntensity, element.Intensity),
				new(CHHapticEventParameterId.HapticSharpness, element.Sharpness)
			];

			native[i] = element.IsContinuous
				? new(CHHapticEventType.HapticContinuous, parameters, element.Time, element.Duration)
				: new(CHHapticEventType.HapticTransient, parameters, element.Time);
		}

		CHHapticDynamicParameter[] dynamics = [];
		CHHapticPattern pattern = new(native, dynamics, out NSError? patternError);
		if (patternError is not null)
			return;

		if (engine.Start(out _) is false)
			return;

		if (engine.CreatePlayer(pattern, out NSError? playerError) is not ICHHapticPatternPlayer player || playerError is not null)
			return;

		player.Start(0, out _);
	}
}
