using UIKit;

namespace BareUI;

/// <summary>
/// A running animation that can be paused, scrubbed by a gesture, reversed, or interrupted mid-flight.
/// </summary>
/// <remarks>Hold it in a field for as long as it runs: it owns a native peer, and a collected animator crashes.</remarks>
public sealed class Animator : IDisposable
{
	readonly UIViewPropertyAnimator native;

	Animator(
		UIViewPropertyAnimator native)
	{
		this.native = native;
	}

	/// <summary>
	/// Prepares an animation of the changes made in <paramref name="changes"/>. It does not run until <see cref="Start"/>.
	/// </summary>
	public static Animator Create(
		Animation animation,
		Action changes)
	{
		Action animated = View.Animated(changes);

		return new(animation.SpringDamping is { } damping
			? new(animation.Duration, (nfloat)damping, animated)
			: new(animation.Duration, Curve(animation.Easing), animated));
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
