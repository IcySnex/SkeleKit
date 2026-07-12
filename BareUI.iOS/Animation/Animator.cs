using UIKit;

namespace BareUI;

/// <summary>
/// A running animation that can be paused, scrubbed by a gesture, reversed, or interrupted mid-flight.
/// </summary>
/// <remarks>Hold it in a field for as long as it runs: it owns a native peer, and a collected animator crashes.</remarks>
public sealed class Animator : IDisposable
{
	UIViewPropertyAnimator native = null!;

	Dictionary<View, ViewState>? captured;

	Animator()
	{ }

	/// <summary>
	/// Prepares an animation of the changes made in <paramref name="changes"/>. It does not run until <see cref="Start"/>.
	/// </summary>
	/// <remarks>Only what <paramref name="changes"/> touches is animated. To animate a layout property (Width, Margin, ...), call <see cref="View.LayoutNow"/> at the end of it — and expect the view's bounds to be scrubbed along with everything else.</remarks>
	public static Animator Create(
		Animation animation,
		Action changes)
	{
		Animator animator = new();

		// the changes write the animation's end values into the model; remember what they moved
		Action recorded = () => animator.captured = AnimationCapture.Run(changes);

		animator.native = animation.SpringDamping is { } damping
			? new(animation.Duration, (nfloat)damping, recorded)
			: new(animation.Duration, Curve(animation.Easing), recorded);

		// added first, so the model is consistent by the time the app's own OnCompleted runs
		animator.native.AddCompletion(animator.Reconcile);

		return animator;
	}

	// a reversed animation ends where it started: UIKit puts the native views back and never tells us,
	// leaving the model a value ahead — and Set's equality check would then swallow the next animation
	void Reconcile(
		UIViewAnimatingPosition position)
	{
		if (position is UIViewAnimatingPosition.Start && captured is { } states)
			foreach ((View view, ViewState state) in states)
				view.Restore(state);

		captured = null;
	}


	/// <summary>
	/// How far the animation has run, from 0 to 1. Assign it to scrub, e.g. from a drag.
	/// </summary>
	public double Fraction
	{
		get => native.FractionComplete;
		set => native.FractionComplete = (nfloat)value;
	}

	/// <summary>
	/// Whether the animation is currently running on its own.
	/// </summary>
	public bool IsRunning =>
		native.Running;

	/// <summary>
	/// Whether the animation is running backwards.
	/// </summary>
	public bool IsReversed
	{
		get => native.Reversed;
		set => native.Reversed = value;
	}

	/// <summary>
	/// Runs the animation, after <see cref="Animation.Delay"/> if one was given.
	/// </summary>
	public void Start(
		double delay = 0)
	{
		if (delay > 0)
			native.StartAnimation(delay);
		else
			native.StartAnimation();
	}

	/// <summary>
	/// Freezes the animation where it is, so <see cref="Fraction"/> can drive it instead.
	/// </summary>
	public void Pause() =>
		native.PauseAnimation();

	/// <summary>
	/// Takes a running animation over: pauses it where it is and puts it back on its forward timeline, ready to be scrubbed.
	/// </summary>
	public void Grab()
	{
		native.PauseAnimation();

		// a reversed animator measures FractionComplete along the *reversed* timeline, so a gesture
		// that grabs one mid-spring-back would otherwise scrub from the wrong end
		if (native.Reversed)
		{
			native.FractionComplete = 1 - native.FractionComplete;
			native.Reversed = false;
		}
	}

	/// <summary>
	/// Hands a paused animation back to the animator, running the rest of it. Below 1, <paramref name="durationFactor"/> finishes faster.
	/// </summary>
	public void Continue(
		double durationFactor = 1) =>
		native.ContinueAnimation(null, (nfloat)durationFactor);

	/// <summary>
	/// Turns the animation around, back towards where it started.
	/// </summary>
	public void Reverse() =>
		native.Reversed = !native.Reversed;

	/// <summary>
	/// Ends the animation. It settles where it is, unless <paramref name="finish"/> runs it to the end.
	/// </summary>
	/// <remarks>Stopping without finishing abandons the animation mid-flight: the views keep the values the animation was heading for, so assign what you want them to hold.</remarks>
	public void Stop(
		bool finish = false)
	{
		native.StopAnimation(!finish);

		if (finish)
			native.FinishAnimation(UIViewAnimatingPosition.End);
	}

	/// <summary>
	/// Calls <paramref name="handler"/> when the animation ends, with true if it reached the end rather than being interrupted.
	/// </summary>
	public void OnCompleted(
		Action<bool> handler) =>
		native.AddCompletion(position => handler(position is UIViewAnimatingPosition.End));

	/// <inheritdoc/>
	public void Dispose() =>
		native.Dispose();


	static UIViewAnimationCurve Curve(
		Easing easing) =>
		easing switch
		{
			Easing.Linear => UIViewAnimationCurve.Linear,
			Easing.EaseIn => UIViewAnimationCurve.EaseIn,
			Easing.EaseOut => UIViewAnimationCurve.EaseOut,
			_ => UIViewAnimationCurve.EaseInOut
		};
}
