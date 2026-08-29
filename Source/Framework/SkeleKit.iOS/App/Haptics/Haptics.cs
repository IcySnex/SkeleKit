using CoreHaptics;

namespace SkeleKit;

internal sealed class Haptics : IHaptics, IDisposable
{
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


	public void Impact(
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

	public void Selection()
	{
		if (Anchor() is not UIWindow anchor)
			return;

		using UISelectionFeedbackGenerator generator = UISelectionFeedbackGenerator.GetFeedbackGenerator(anchor);

		generator.Prepare();
		generator.SelectionChanged();
	}

	public void Notify(
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
			return;

		if (!engine.Start(out _))
			return;

		if (engine.CreatePlayer(pattern, out NSError? playerError) is not ICHHapticPatternPlayer player || playerError is not null)
			return;

		player.Start(0, out _);
	}

	public void Dispose()
	{
		engine?.Dispose();
		engine = null;
	}
}
