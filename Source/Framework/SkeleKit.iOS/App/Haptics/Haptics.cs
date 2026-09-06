using CoreHaptics;
using Microsoft.Extensions.Logging;

namespace SkeleKit;

internal sealed class Haptics(
	ILogger<Haptics> logger) : IHaptics, IDisposable
{
	static Exception? ToException(
		NSError? error) =>
		error is null ? null : new Exception(error.LocalizedDescription);

	CHHapticEngine? engine;

	static UIWindow? Anchor() =>
		UIApplication.SharedApplication
			.ConnectedScenes
			.OfType<UIWindowScene>()
			.SelectMany(scene => scene.Windows)
			.FirstOrDefault(window => window.IsKeyWindow);

	CHHapticEngine? SharedEngine()
	{
		if (!CHHapticEngine.GetHardwareCapabilities().SupportsHaptics)
		{
			logger.LogWarning("This device or simulator does not support haptics.");
			return null;
		}

		if (engine is not null)
			return engine;

		CHHapticEngine created = new(out NSError? error);
		if (error is not null)
		{
			logger.LogWarning(ToException(error), "Failed to create the Core Haptics engine.");
			created.Dispose();
			return null;
		}

		created.AutoShutdownEnabled = true;
		created.ResetHandler = () =>
		{
			if (!created.Start(out NSError? resetError))
				logger.LogWarning(ToException(resetError), "Failed to restart the Core Haptics engine after a reset.");
		};

		engine = created;
		return engine;
	}


	public void Impact(
		HapticStyle style = HapticStyle.Medium)
	{
		if (Anchor() is not UIWindow anchor)
		{
			logger.LogWarning("Failed to impact because the anchor is not a UIWindow.");
			return;
		}

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

	public void Selection()
	{
		if (Anchor() is not UIWindow anchor)
		{
			logger.LogWarning("Failed to selection because the anchor is not a UIWindow.");
			return;
		}

		using UISelectionFeedbackGenerator generator = UISelectionFeedbackGenerator.GetFeedbackGenerator(anchor);

		generator.Prepare();
		generator.SelectionChanged();
	}

	public void Notify(
		HapticsNotification notification)
	{
		if (Anchor() is not UIWindow anchor)
		{
			logger.LogWarning("Failed to notify because the anchor is not a UIWindow.");
			return;
		}

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

	public void Play(
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
		{
			logger.LogWarning(ToException(patternError), "Failed to create a Core Haptics pattern.");
			return;
		}

		if (!engine.Start(out NSError? startError))
		{
			logger.LogWarning(ToException(startError), "Failed to start the Core Haptics engine.");
			return;
		}

		if (engine.CreatePlayer(pattern, out NSError? playerError) is not ICHHapticPatternPlayer player || playerError is not null)
		{
			logger.LogWarning(ToException(playerError), "Failed to create a Core Haptics pattern player.");
			return;
		}

		if (!player.Start(0, out NSError? playError))
			logger.LogWarning(ToException(playError), "Failed to play a Core Haptics pattern.");
	}

	public void Dispose()
	{
		engine?.Dispose();
		engine = null;
	}
}
