using System.Windows.Input;
using BareUI.Gallery.ViewModels.Demos;
using CommunityToolkit.Mvvm.Input;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates spring animations and an interruptible, drag-scrubbed <see cref="Animator"/>.
/// </summary>
public class AnimationDemo : ContentView<AnimationDemoViewModel>
{
	// the animator owns a native peer: a field is what keeps it alive while it runs
	Animator? drag;
	bool open;

	public AnimationDemo()
	{
		Title = "Animation";

		Border badge = new()
		{
			Background = Colors.Indigo,
			CornerRadius = 16,
			Padding = new Thickness(20, 12),
			HorizontalAlignment = HorizontalAlignment.Center,
			Child = new Label { Style = Styles.Title, Text = "Tap to spring", TextColor = Colors.White }
		};
		badge.TapCommand = Bindable.From<ICommand?>(new RelayCommand(() => Bounce(badge)));

		Border card = new()
		{
			Background = LinearGradient.Vertical(Colors.Indigo, Colors.Pink),
			CornerRadius = 16,
			Height = 120,
			Padding = new Thickness(16),
			Child = new Label
			{
				Style = Styles.Title,
				Text = "Drag me, let go mid-flight, grab again",
				TextColor = Colors.White,
				MaxLines = 2
			}
		};
		card.OnPan(pan => Drag(card, pan));

		Content = new VStack
		{
			Spacing = 20,
			Margin = new Thickness(16),
			Children =
			{
				new Label { Style = Styles.Caption, Text = "Animation.Spring — a fire-and-forget animation" },
				badge,

				new Label { Style = Styles.Caption, Text = "Animator — the drag scrubs it, the release hands it back" },
				card,
				new Label { Style = Styles.Caption, Text = "Let go past halfway and it settles open; short of it, it springs back." }
			}
		};
	}

	static void Bounce(
		View view) =>
		View.Animate(Animation.Spring(damping: 0.4), () => view.Scale = view.Scale is 1 ? 1.25 : 1);

	void Drag(
		View card,
		PanGesture pan)
	{
		const double distance = 240;

		switch (pan.State)
		{
			case GestureState.Began:
				// building it paused is what lets the drag scrub an animation that UIKit would otherwise run
				drag?.Dispose();
				drag = Animator.Create(Animation.Spring(), () =>
				{
					card.Translation = new(open ? 0 : distance, 0);
					card.Rotation = open ? 0 : 6;
				});
				drag.Start();
				drag.Pause();
				break;

			case GestureState.Changed:
				if (drag is not null)
					drag.Fraction = Math.Clamp((open ? -pan.Translation.X : pan.Translation.X) / distance, 0, 1);
				break;

			default:
				if (drag is null)
					break;

				// past halfway (or thrown hard enough) it finishes; otherwise it runs back to where it came from
				bool completes = drag.Fraction > 0.5 || Math.Abs(pan.Velocity.X) > 800;

				drag.IsReversed = !completes;
				drag.Continue();

				if (completes)
					open = !open;
				break;
		}
	}
}
