using UIKit;

namespace BareUI;

/// <summary>
/// How strong a haptic tap feels.
/// </summary>
public enum HapticStyle
{
	/// <summary>A light tap.</summary>
	Light,

	/// <summary>A medium tap.</summary>
	Medium,

	/// <summary>A heavy tap.</summary>
	Heavy,

	/// <summary>A soft tap.</summary>
	Soft,

	/// <summary>A rigid tap.</summary>
	Rigid
}

/// <summary>
/// Haptic feedback.
/// </summary>
public static class Haptics
{
	/// <summary>
	/// A tap, for a button or a state change.
	/// </summary>
	public static void Impact(
		HapticStyle style = HapticStyle.Medium)
	{
		using UIImpactFeedbackGenerator generator = new(Style(style));

		generator.Prepare();
		generator.ImpactOccurred();
	}

	/// <summary>
	/// A tick, for moving through a set of values.
	/// </summary>
	public static void Selection()
	{
		using UISelectionFeedbackGenerator generator = new();

		generator.Prepare();
		generator.SelectionChanged();
	}

	/// <summary>
	/// Success, warning or failure.
	/// </summary>
	public static void Notify(
		bool success)
	{
		using UINotificationFeedbackGenerator generator = new();

		generator.Prepare();
		generator.NotificationOccurred(success
			? UINotificationFeedbackType.Success
			: UINotificationFeedbackType.Error);
	}

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
}
