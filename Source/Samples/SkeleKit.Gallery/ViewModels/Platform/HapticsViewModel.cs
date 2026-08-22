using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Platform;

internal sealed partial class HapticsViewModel : ShowcaseViewModel
{
	public HapticsViewModel()
	{
		SelectedImpactStyle = ImpactStyles[1];
		SelectedNotification = Notifications[0];
	}


	public List<ShowcaseOption<HapticStyle>> ImpactStyles { get; } =
	[
		new("Light", HapticStyle.Light),
		new("Medium", HapticStyle.Medium),
		new("Heavy", HapticStyle.Heavy),
		new("Soft", HapticStyle.Soft),
		new("Rigid", HapticStyle.Rigid)
	];

	public List<ShowcaseOption<HapticsNotification>> Notifications { get; } =
	[
		new("Success", HapticsNotification.Success),
		new("Warning", HapticsNotification.Warning),
		new("Error", HapticsNotification.Error)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ImpactCode))]
	ShowcaseOption<HapticStyle> selectedImpactStyle = null!;

	[ObservableProperty]
	string impactResult = "Not triggered";

	[ObservableProperty]
	string selectionResult = "Not triggered";

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(NotificationCode))]
	ShowcaseOption<HapticsNotification> selectedNotification = null!;

	[ObservableProperty]
	string notificationResult = "Not triggered";

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IntensityLabel))]
	[NotifyPropertyChangedFor(nameof(CustomPatternCode))]
	double intensity = 0.7;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SharpnessLabel))]
	[NotifyPropertyChangedFor(nameof(CustomPatternCode))]
	double sharpness = 0.5;

	[ObservableProperty]
	string customPatternResult = "Not played";

	public string IntensityLabel =>
		Number(Intensity);

	public string SharpnessLabel =>
		Number(Sharpness);

	public IReadOnlyList<Span> ImpactCode =>
	[
		new(
			$$"""
			Haptics.Impact(HapticStyle.{{SelectedImpactStyle.Value}});
			""")
	];

	public IReadOnlyList<Span> SelectionCode { get; } =
	[
		new(
			"""
			Haptics.Selection();
			""")
	];

	public IReadOnlyList<Span> NotificationCode =>
	[
		new(
			$$"""
			Haptics.Notify(HapticsNotification.{{SelectedNotification.Value}});
			""")
	];

	public IReadOnlyList<Span> CustomPatternCode =>
	[
		new(
			$$"""
			Haptics.Play(
				HapticEvent.Tap(
					0,
					intensity: {{Number(Intensity)}},
					sharpness: {{Number(Sharpness)}}),
				HapticEvent.Continuous(
					0.1,
					0.3,
					intensity: {{Number(Intensity)}},
					sharpness: {{Number(Sharpness)}}),
				HapticEvent.Tap(
					0.5,
					intensity: {{Number(Intensity)}},
					sharpness: {{Number(Sharpness)}}));
			""")
	];


	[RelayCommand]
	void TriggerImpact()
	{
		Haptics.Impact(SelectedImpactStyle.Value);
		ImpactResult = $"Triggered: {SelectedImpactStyle.Title}";
	}

	[RelayCommand]
	void TriggerSelection()
	{
		Haptics.Selection();
		SelectionResult = "Triggered";
	}

	[RelayCommand]
	void TriggerNotification()
	{
		Haptics.Notify(SelectedNotification.Value);
		NotificationResult = $"Triggered: {SelectedNotification.Title}";
	}

	[RelayCommand]
	void PlayCustomPattern()
	{
		Haptics.Play(
			HapticEvent.Tap(0, Intensity, Sharpness),
			HapticEvent.Continuous(0.1, 0.3, Intensity, Sharpness),
			HapticEvent.Tap(0.5, Intensity, Sharpness));

		CustomPatternResult = "Played";
	}


	static string Number(
		double value) =>
		value.ToString("0.0", CultureInfo.InvariantCulture);
}
