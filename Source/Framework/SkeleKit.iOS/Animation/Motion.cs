namespace SkeleKit;

internal sealed class Motion(
	Animation animation)
{
	double origin;
	double elapsed;


	public double Position { get; set; }

	public double Velocity { get; set; }

	public double Target { get; private set; } = 1;


	public void Run(
		double target)
	{
		Target = target;
		origin = Position;
		elapsed = 0;
	}

	public bool Step(
		double dt)
	{
		if (animation.SpringDamping is double damping)
			return StepSpring(dt, damping);

		elapsed += dt;

		// the curve covers the remaining distance in a proportional slice of the duration
		double duration = Math.Max(animation.Duration * Math.Abs(Target - origin), 0.01);
		double t = Math.Min(elapsed / duration, 1);

		Position = origin + (Target - origin) * Ease(t);
		Velocity = 0;

		return t < 1;
	}

	bool StepSpring(
		double dt,
		double damping)
	{
		// unit-mass spring, stiffness from the settle duration: ω = 2π / duration
		double omega = 2 * Math.PI / Math.Max(animation.Duration, 0.05);
		double stiffness = omega * omega;
		double drag = 2 * damping * omega;

		// semi-implicit Euler, sub-stepped so a dropped frame cannot destabilize it
		int steps = Math.Max(1, (int)Math.Ceiling(dt / 0.008));
		double h = dt / steps;

		for (int i = 0; i < steps; i++)
		{
			Velocity += (-stiffness * (Position - Target) - drag * Velocity) * h;
			Position += Velocity * h;
		}

		if (Math.Abs(Position - Target) > 0.001 || Math.Abs(Velocity) > 0.01)
			return true;

		Position = Target;
		Velocity = 0;

		return false;
	}

	double Ease(
		double t) =>
		animation.Easing switch
		{
			Easing.Linear => t,
			Easing.EaseIn => t * t,
			Easing.EaseOut => t * (2 - t),
			_ => t * t * (3 - 2 * t)
		};
}
