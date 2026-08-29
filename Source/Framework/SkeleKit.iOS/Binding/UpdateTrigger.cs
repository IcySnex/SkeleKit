namespace SkeleKit;

/// <summary>
/// When a two-way binding pushes the control's value back to the source.
/// </summary>
public enum UpdateTrigger
{
	/// <summary>
	/// On every change (default).
	/// </summary>
	PropertyChanged,

	/// <summary>
	/// When the control loses focus.
	/// </summary>
	FocusLost
}
