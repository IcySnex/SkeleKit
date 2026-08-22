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
	/// The page's measured content and navigation chrome, updated as layout changes and capped at the available height.
	/// </summary>
	public static readonly Detent Content = new(DetentKind.Content);


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

	internal double Resolve(
		double maximum,
		double content = 0) =>
		Kind switch
		{
			DetentKind.Height => Math.Min(Value, maximum),
			DetentKind.Fraction => Value * maximum,
			DetentKind.Content => Math.Clamp(content, 0, maximum),
			_ => maximum
		};
}

internal enum DetentKind
{
	Medium,
	Large,
	Content,
	Height,
	Fraction
}
