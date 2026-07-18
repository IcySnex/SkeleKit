using CoreAnimation;
using CoreFoundation;

namespace BareUI;

/// <summary>
/// A running animation that can be paused, scrubbed by a gesture, reversed, or interrupted mid-flight.
/// </summary>
/// <remarks>
/// Hold it in a field for as long as it runs: it owns a native peer, and a collected animator stops ticking.
/// </remarks>
public sealed class Animator : IDisposable
{
	readonly Action changes;
	readonly Motion motion;
	readonly List<Action<bool>> completions = [];

	Dictionary<View, ViewState>? start;
	Dictionary<View, ViewState>? end;

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
	/// Prepares an animation of the changes made in <paramref name="changes"/>.
	/// </summary>
	/// <remarks>
	/// It does not run until <see cref="Start"/>.<br/>
	/// Only what <paramref name="changes"/> touches is animated. Transforms, Opacity, CornerRadius, colors, gradients and layout lengths all interpolate; what has no in-between (a Material, a system color, an auto-sized Width) snaps when the animation settles.
	/// </remarks>
	public static Animator Create(
		Animation animation,
		Action changes) =>
		new(animation, changes);


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
		{
			view.Apply(position switch
			{
				0 => from,
				1 => end![view],
				_ => ViewState.Lerp(from, end![view], position)
			});
		}

		CATransaction.Commit();
	}

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


	/// <summary>
	/// How far the animation has run, from 0 to 1.
	/// </summary>
	/// <remarks>
	/// Assign it to scrub, e.g. from a drag.
	/// </remarks>
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
	public bool IsRunning => link is not null;

	/// <summary>
	/// Whether the animation is headed backwards, towards where it started.
	/// </summary>
	/// <remarks>
	/// Takes effect on the next <see cref="Continue"/>.
	/// </remarks>
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
		{
			DispatchQueue.MainQueue.DispatchAfter(
				new(DispatchTime.Now, TimeSpan.FromSeconds(delay)),
				() => Continue());
		}
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
	/// Runs the animation from wherever it is towards its current heading.
	/// </summary>
	/// <remarks>
	/// For a spring, <paramref name="velocity"/> carries the gesture's speed in, as full travels per second, positive towards the end.
	/// </remarks>
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
	/// Ends the animation.
	/// </summary>
	/// <remarks>
	/// It settles where it is, unless <paramref name="finish"/> jumps it to the end.
	/// </remarks>
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
}
