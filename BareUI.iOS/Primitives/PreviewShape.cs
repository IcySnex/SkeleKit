namespace BareUI;

/// <summary>
/// The shape of a row lifted as its own context-menu platter.
/// </summary>
/// <param name="Padding">Uniform padding between the row's content and the platter's edge.</param>
/// <param name="CornerRadius">The platter's corner radius.</param>
/// <param name="Background">The platter's fill, or null for the system default. A transparent color draws none.</param>
public sealed record PreviewShape(
	double Padding = 0,
	double CornerRadius = 0,
	Color? Background = null);
