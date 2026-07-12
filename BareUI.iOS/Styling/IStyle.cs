namespace BareUI;

/// <summary>
/// A reusable block of property setters for one view type.
/// </summary>
public interface IStyle
{
	/// <summary>
	/// The view type the style configures.
	/// </summary>
	Type TargetType { get; }

	/// <summary>
	/// Runs the style's setters against <paramref name="view"/>.
	/// </summary>
	void Apply(
		View view);
}
