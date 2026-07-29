namespace SkeleKit;

/// <summary>
/// A height where a modal sheet may rest.
/// </summary>
public readonly record struct Detent
{
	/// <summary>
	/// The system medium height.
	/// </summary>
	public static readonly Detent Medium = new(DetentKind.Medium);

	/// <summary>
	/// The system full height.
	/// </summary>
	public static readonly Detent Large = new(DetentKind.Large);


	/// <summary>
	/// A fixed height in points, clamped to the sheet's available height.
	/// </summary>
	/// <param name="height">The height in points.</param>
	/// <returns>The fixed-height detent.</returns>
	public static Detent Height(
		double height)
	{
		if (!double.IsFinite(height) || height <= 0)
			throw new ArgumentOutOfRangeException(nameof(height));

		return new(DetentKind.Height, height);
	}

	/// <summary>
	/// A fraction of the sheet's available height.
	/// </summary>
	/// <param name="fraction">A value greater than 0 and no greater than 1.</param>
	/// <returns>The proportional detent.</returns>
	public static Detent Fraction(
		double fraction)
	{
		if (!double.IsFinite(fraction) || fraction <= 0 || fraction > 1)
			throw new ArgumentOutOfRangeException(nameof(fraction));

		return new(DetentKind.Fraction, fraction);
	}


	Detent(
		DetentKind kind,
		double value = 0)
	{
		Kind = kind;
		Value = value;
	}


	internal DetentKind Kind { get; }

	internal double Value { get; }
}

internal enum DetentKind
{
	Medium,
	Large,
	Height,
	Fraction
}
