namespace BareUI;

/// <summary>
/// Provides access to native device haptic feedback.
/// </summary>
public static class Haptics
{
	static UIWindow? Anchor() =>
		UIApplication.SharedApplication
			.ConnectedScenes
			.OfType<UIWindowScene>()
			.SelectMany(scene => scene.Windows)
			.FirstOrDefault(window => window.IsKeyWindow);


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
}
