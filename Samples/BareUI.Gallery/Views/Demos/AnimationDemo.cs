using System.Windows.Input;
using BareUI.Gallery.ViewModels.Demos;
using CommunityToolkit.Mvvm.Input;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates spring animations and an interruptible, drag-scrubbed <see cref="Animator"/>.
/// </summary>
[Page]
public class AnimationDemo : ContentView<AnimationDemoViewModel>
{
	const double Distance = 240;

	// hex literals on purpose: a system color resolves natively and cannot lerp, so it would snap
	static readonly LinearGradient ClosedFill = LinearGradient.Vertical(Color.FromHex(0x5856D6), Color.FromHex(0xFF2D55));
	static readonly LinearGradient OpenFill = LinearGradient.Vertical(Color.FromHex(0x32ADE6), Color.FromHex(0x30D158));

	// the animator owns a native peer: a field is what keeps it alive while it runs
	Animator? drag;
	double grabbedAt;
	double panStart;
	bool open;

	public AnimationDemo(
		AnimationDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Animation";

		Border badge = new()
		{
			Background = Colors.Indigo,
			CornerRadius = 16,
			Padding = new Thickness(20, 12),
			HorizontalAlignment = HorizontalAlignment.Center,
			Child = new Label { Style = Styles.Title, Text = "Tap, hold or pinch", TextColor = Colors.White }
		};
		badge.TapCommand = Command.From(() => Bounce(badge));
		badge.LongPressCommand = Command.From(() => Bounce(badge));

		// the pinch drives the transform live; the release springs it home
		badge.Pinched = pinch =>
		{
			if (pinch.State is GestureState.Changed)
				badge.Scale = Math.Clamp(pinch.Scale, 0.5, 2.5);
			else if (pinch.State is GestureState.Ended or GestureState.Canceled)
				View.Animate(Animation.Spring(damping: 0.5), () => badge.Scale = 1);
		};

		Border card = new()
		{
			Background = ClosedFill,
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
		card.Panned = pan => Drag(card, pan);

		Content = new StackPanel
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
		switch (pan.State)
		{
			// a running animator is taken over, never replaced: two animators on one transform fight
			case GestureState.Began:
				drag ??= Prepare(card);
				drag.Stop();

				grabbedAt = drag.Fraction;

				// the recognizer only fires Began after ~10pt of slop; without zeroing, that slop
				// lands in the first Changed as a visible jump
				panStart = pan.Translation.X;
				break;

			case GestureState.Changed:
				if (drag is not null)
					drag.Fraction = Math.Clamp(grabbedAt + (Travel(pan.Translation.X - panStart) / Distance), 0, 1);
				break;

			default:
				if (drag is null)
					break;

				double thrown = Travel(pan.Velocity.X);

				// past halfway, or thrown hard enough, it finishes; otherwise it runs back where it came from
				bool completes = drag.Fraction > 0.5 || thrown > 800;

				drag.IsReversed = !completes;
				drag.Continue(thrown / Distance);
				break;
		}
	}

	// the animation always runs 0 -> 1, so an open card reads a drag back to the left as forward progress
	double Travel(
		double x) =>
		open ? -x : x;

	Animator Prepare(
		View card)
	{
		Animator animator = Animator.Create(Animation.Spring(), () =>
		{
			card.Translation = new(open ? 0 : Distance, 0);
			card.Rotation = open ? 0 : 6;
			card.Background = open ? ClosedFill : OpenFill;
		});

		// it only flips once the animation actually reached the end; a spring-back leaves it as it was
		animator.OnCompleted(finished =>
		{
			if (finished)
				open = !open;

			drag = null;
		});

		// not started: the first Grab readies it, paused at 0, ready to scrub
		return animator;
	}
}
