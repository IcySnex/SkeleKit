namespace SkeleKit;

/// <summary>
/// Corner radii resolved from the current operating system.
/// </summary>
public static partial class SystemCornerRadius
{
	/// <summary>
	/// The corner radius of an inset-grouped list section.
	/// </summary>
	/// <remarks>
	/// Pair with <see cref="CornerCurve.Continuous"/> to match the system surface.
	/// </remarks>
	public static double GroupedList
	{
		get
		{
			double radius = 0;
			ResolveGroupedList(ref radius);
			return radius;
		}
	}


	static partial void ResolveGroupedList(
		ref double radius);
}
