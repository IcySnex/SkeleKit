using Xunit;

namespace SkeleKit.Tests.Primitives;

public class MotionTests
{
	// advances in display-refresh-sized steps until settled or the safety cap trips
	static double Settle(
		Motion motion,
		double maxSeconds = 10)
	{
		double elapsed = 0;

		while (motion.Step(1.0 / 60) && elapsed < maxSeconds)
			elapsed += 1.0 / 60;

		return elapsed;
	}

	[Fact]
	public void Spring_SettlesOnTheTarget()
	{
		Motion motion = new(Animation.Spring());
		motion.Run(1);

		double elapsed = Settle(motion);

		Assert.Equal(1, motion.Position);
		Assert.Equal(0, motion.Velocity);
		Assert.True(elapsed < 2, $"took {elapsed}s");
	}

	[Fact]
	public void Spring_WithLowDamping_Overshoots()
	{
		Motion motion = new(Animation.Spring(damping: 0.3));
		motion.Run(1);

		double peak = 0;

		while (motion.Step(1.0 / 60))
			peak = Math.Max(peak, motion.Position);

		Assert.True(peak > 1.01, $"peaked at {peak}");
		Assert.Equal(1, motion.Position);
	}

	[Fact]
	public void Spring_FromAScrubbedPosition_RunsBackToTheStart()
	{
		Motion motion = new(Animation.Spring());
		motion.Position = 0.4;
		motion.Run(0);

		Settle(motion);

		Assert.Equal(0, motion.Position);
	}

	[Fact]
	public void Spring_CarriesASeededVelocity()
	{
		Motion motion = new(Animation.Spring(damping: 1));
		motion.Position = 0.5;
		motion.Velocity = 8;
		motion.Run(1);

		// one early step must move faster than the same spring without momentum
		motion.Step(1.0 / 60);
		double thrown = motion.Position;

		Motion still = new(Animation.Spring(damping: 1));
		still.Position = 0.5;
		still.Run(1);
		still.Step(1.0 / 60);

		Assert.True(thrown > still.Position);
	}

	[Fact]
	public void Retarget_MidFlight_KeepsMomentum()
	{
		Motion motion = new(Animation.Spring());
		motion.Run(1);

		for (int i = 0; i < 6; i++)
			motion.Step(1.0 / 60);

		double velocity = motion.Velocity;
		motion.Run(0);

		Assert.Equal(velocity, motion.Velocity);
	}

	[Fact]
	public void Curve_ReachesTheTargetWithinItsDuration()
	{
		Motion motion = new(Animation.Ease(0.3));
		motion.Run(1);

		double elapsed = Settle(motion);

		Assert.Equal(1, motion.Position);
		Assert.True(elapsed <= 0.35, $"took {elapsed}s");
	}

	[Fact]
	public void Curve_FromAScrubbedPosition_TakesTheProportionalSlice()
	{
		Motion motion = new(Animation.Ease(0.4, Easing.Linear));
		motion.Position = 0.75;
		motion.Run(1);

		double elapsed = Settle(motion);

		// a quarter of the travel is a quarter of the duration
		Assert.True(elapsed <= 0.12, $"took {elapsed}s");
	}

	[Fact]
	public void Step_SurvivesADroppedFrame()
	{
		Motion motion = new(Animation.Spring(damping: 0.3));
		motion.Run(1);

		// half a second in one step: the sub-stepping must keep the integrator stable
		motion.Step(0.5);

		Assert.InRange(motion.Position, -1, 2);
	}
}
