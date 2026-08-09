using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.StylingMotion;

internal sealed partial class AnimationViewModel : ShowcaseViewModel
{
	static readonly Animation[] Timings =
	[
		Animation.Spring(0.5, damping: 0.72),
		Animation.Ease(0.3, Easing.EaseInOut),
		Animation.Ease(0.3, Easing.EaseOut)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedTiming))]
	[NotifyPropertyChangedFor(nameof(AnimationCode))]
	int timingIndex;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ActionTitle))]
	bool isExpanded;

	internal Animation SelectedTiming =>
		Timings[Math.Clamp(TimingIndex, 0, Timings.Length - 1)];

	public string ActionTitle =>
		IsExpanded ? "Collapse" : "Expand";

	public IReadOnlyList<Span> AnimationCode =>
		[new(
			$$"""
			View artwork = new Border { Margin = new(10, 0, 0, 0), Scale = 0.82 };
			View details = new Label { Text = "Collection", Translation = new(24, 0), Opacity = 0 };
			Border card = new()
			{
				Width = 84,
				Height = 84,
				CornerRadius = 20,
				Child = new Overlay { Children = { artwork, details } }
			};

			bool expanded = false;

			void Toggle()
			{
				expanded = !expanded;

				View.Animate(
					{{TimingCode()}},
					() =>
					{
						card.Width = expanded ? 280 : 84;
						card.Height = expanded ? 128 : 84;
						card.CornerRadius = expanded ? 22 : 20;
						artwork.Margin = expanded ? new(48, 0, 0, 0) : new(10, 0, 0, 0);
						artwork.Scale = expanded ? 1 : 0.82;
						details.Translation = expanded ? new(44, 0) : new(24, 0);
						details.Opacity = expanded ? 1 : 0;
					});
			}
			""")];

	public IReadOnlyList<Span> AnimatorCode =>
		[new(
			"""
			Border artwork = new();
			artwork.Translation = new(-88, 0);

			Animator animator = Animator.Create(
				Animation.Spring(0.5, damping: 0.72),
				() =>
				{
					artwork.Translation = new(88, 0);
					artwork.Scale = 1;
					artwork.Opacity = 1;
				});
			animator.Fraction = 0;

			const double distance = 176;
			const double maxReleaseVelocity = 4;
			double grabbedAt = 0;
			double panStart = 0;
			artwork.Panned = pan =>
			{
				switch (pan.State)
				{
					case GestureState.Began:
						animator.Pause();
						grabbedAt = animator.Fraction;
						panStart = pan.Translation.X;
						break;

					case GestureState.Changed:
						animator.Fraction = Math.Clamp(
							grabbedAt + (pan.Translation.X - panStart) / distance,
							0,
							1);
						break;

					default:
						double velocity = pan.Velocity.X;
						bool towardEnd = Math.Abs(velocity) > 600
							? velocity > 0
							: animator.Fraction >= 0.5;

						animator.IsReversed = !towardEnd;
						animator.Continue(Math.Clamp(
							velocity / distance,
							-maxReleaseVelocity,
							maxReleaseVelocity));
						break;
				}
			};
			""")];


	string TimingCode() =>
		Math.Clamp(TimingIndex, 0, Timings.Length - 1) switch
		{
			0 => "Animation.Spring(0.5, damping: 0.72)",
			1 => "Animation.Ease(0.3, Easing.EaseInOut)",
			_ => "Animation.Ease(0.3, Easing.EaseOut)"
		};
}
