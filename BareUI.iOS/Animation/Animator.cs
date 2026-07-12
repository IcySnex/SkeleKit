using CoreAnimation;
using CoreFoundation;
using Foundation;

namespace BareUI;

/// <summary>
/// A running animation that can be paused, scrubbed by a gesture, reversed, or interrupted mid-flight.
/// </summary>
/// <remarks>Hold it in a field for as long as it runs: it owns a native peer, and a collected animator stops ticking.</remarks>
public sealed class Animator : IDisposable
{
	// no UIViewPropertyAnimator: it cannot retime, reverse, or rescrub without jumping. The animator
	// integrates its own spring and writes the interpolated state into the model every frame, so the
	// screen and the shadow model are the same thing by construction
	readonly Action changes;
	readonly Motion motion;

	Dictionary<View, ViewState>? start;
	Dictionary<View, ViewState>? end;

	readonly List<Action<bool>> completions = [];

	CADisplayLink? link;
	double lastTime;
	double heading = 1;

	Animator(
		Animation animation,
		Action changes)
	{
		this.changes = changes;

		motion = new(animation);
	}

	/// <summary>
	/// Prepares an animation of the changes made in <paramref name="changes"/>. It does not run until <see cref="Start"/>.
	/// </summary>
	/// <remarks>Only what <paramref name="changes"/> touches is animated, and only its draw-only properties (Translation, Scale, Rotation, Opacity, CornerRadius) interpolate — layout properties snap when it completes.</remarks>
	public static Animator Create(
		Animation animation,
		Action changes) =>
		new(animation, changes);

	// runs the changes once: the model briefly holds the end values while both ends are snapshotted,
	// then the same tick puts everything back at 0, so nothing ever renders
	void Materialize()
	{
		if (start is not null)
			return;

		start = AnimationCapture.Run(changes);
		end = start.Keys.ToDictionary(view => view, view => view.Capture());

		Apply(0);
	}

	void Apply(
		double position)
	{
		CATransaction.Begin();
		CATransaction.DisableActions = true;

		foreach ((View view, ViewState from) in start!)
			view.Apply(position switch
			{
				0 => from,
				1 => end![view],
				_ => ViewState.Lerp(from, end![view], position)
			});

		CATransaction.Commit();
	}


	/// <summary>
	/// How far the animation has run, from 0 to 1. Assign it to scrub, e.g. from a drag.
	/// </summary>
	public double Fraction
	{
		get => motion.Position;
		set
		{
			Materialize();

			motion.Position = value;
			motion.Velocity = 0;

			Apply(value);
		}
	}

	/// <summary>
	/// Whether the animation is currently running on its own.
	/// </summary>
	public bool IsRunning =>
		link is not null;

	/// <summary>
	/// Whether the animation is headed backwards, towards where it started. Takes effect on the next <see cref="Continue"/>.
	/// </summary>
	public bool IsReversed
	{
		get => heading is 0;
		set => heading = value ? 0 : 1;
	}

	/// <summary>
	/// Runs the animation, after <paramref name="delay"/> seconds if given.
	/// </summary>
	public void Start(
		double delay = 0)
	{
		if (delay > 0)
			DispatchQueue.MainQueue.DispatchAfter(
				new DispatchTime(DispatchTime.Now, TimeSpan.FromSeconds(delay)),
				() => Continue());
		else
			Continue();
	}

	/// <summary>
	/// Freezes the animation where it is, so <see cref="Fraction"/> can drive it instead.
	/// </summary>
	public void Pause()
	{
		Materialize();
		StopLink();
	}

	/// <summary>
	/// Takes a running animation over: pauses it where it is, ready to be scrubbed. Same as <see cref="Pause"/>.
	/// </summary>
	public void Grab() =>
		Pause();

	/// <summary>
	/// Runs the animation from wherever it is towards its current heading. For a spring, <paramref name="velocity"/> carries the gesture's speed in, as full travels per second, positive towards the end.
	/// </summary>
	public void Continue(
		double velocity = 0)
	{
		Materialize();

		if (velocity is not 0)
			motion.Velocity = velocity;

		motion.Run(heading);
		StartLink();
	}

	/// <summary>
	/// Turns the animation around, keeping its momentum.
	/// </summary>
	public void Reverse()
	{
		heading = 1 - heading;

		if (link is not null)
			motion.Run(heading);
	}

	/// <summary>
	/// Ends the animation. It settles where it is, unless <paramref name="finish"/> jumps it to the end.
	/// </summary>
	public void Stop(
		bool finish = false)
	{
		StopLink();

		if (!finish || start is null)
			return;

		motion.Position = 1;
		motion.Velocity = 0;

		Apply(1);
		Dispatch(true);
	}

	/// <summary>
	/// Calls <paramref name="handler"/> when the animation ends, with true if it reached the end rather than being interrupted.
	/// </summary>
	public void OnCompleted(
		Action<bool> handler) =>
		completions.Add(handler);

	/// <inheritdoc/>
	public void Dispose() =>
		StopLink();


	void Tick()
	{
		double now = link!.TargetTimestamp;
		double dt = Math.Clamp(now - lastTime, 0.001, 1.0 / 20);
		lastTime = now;

		if (motion.Step(dt))
		{
			Apply(motion.Position);
			return;
		}

		StopLink();
		Apply(motion.Target);
		Dispatch(motion.Target is 1);
	}

	void Dispatch(
		bool finished)
	{
		foreach (Action<bool> handler in completions)
			handler(finished);
	}

	void StartLink()
	{
		if (link is not null)
			return;

		link = CADisplayLink.Create(Tick);
		lastTime = CAAnimation.CurrentMediaTime();

		link.AddToRunLoop(NSRunLoop.Main, NSRunLoopMode.Common);
	}

	void StopLink()
	{
		link?.Invalidate();
		link = null;
	}
}
