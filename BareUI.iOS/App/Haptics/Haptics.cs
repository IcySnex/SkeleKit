namespace BareUI;

/// <summary>
/// Haptic feedback.
/// </summary>
public static class Haptics
{
	static UIWindow? Anchor() =>
		UIApplication.SharedApplication
			.ConnectedScenes
			.OfType<UIWindowScene>()
			.SelectMany(scene => scene.Windows)
			.FirstOrDefault(window => window.IsKeyWindow);

	static UIImpactFeedbackStyle Style(
		HapticStyle style) =>
		style switch
		{
			HapticStyle.Light => UIImpactFeedbackStyle.Light,
			HapticStyle.Heavy => UIImpactFeedbackStyle.Heavy,
			HapticStyle.Soft => UIImpactFeedbackStyle.Soft,
			HapticStyle.Rigid => UIImpactFeedbackStyle.Rigid,
			_ => UIImpactFeedbackStyle.Medium
		};

	static UINotificationFeedbackType Notification(
		HapticsNotification style) =>
		style switch
		{
			HapticsNotification.Warning => UINotificationFeedbackType.Warning,
			HapticsNotification.Error => UINotificationFeedbackType.Error,
			_ => UINotificationFeedbackType.Success
		};


	/// <summary>
	/// A tap, for a button or a state change.
	/// </summary>
	public static void Impact(
		HapticStyle style = HapticStyle.Medium)
	{
		if (Anchor() is not UIWindow anchor)
			return;

		using UIImpactFeedbackGenerator generator = UIImpactFeedbackGenerator.GetFeedbackGenerator(Style(style), anchor);

		generator.Prepare();
		generator.ImpactOccurred();
	}

	/// <summary>
	/// A tick, for moving through a set of values.
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
	/// Success, warning or failure.
	/// </summary>
	public static void Notify(
		HapticsNotification notification)
	{
		if (Anchor() is not UIWindow anchor)
			return;

		using UINotificationFeedbackGenerator generator = UINotificationFeedbackGenerator.GetFeedbackGenerator(anchor);

		generator.Prepare();
		generator.NotificationOccurred(Notification(notification));
	}
}
