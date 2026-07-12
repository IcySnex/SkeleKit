namespace BareUI;

/// <summary>
	///
/// Which way values flow between the binding source and the control.
/// </summary>
public enum BindingMode
{
	/// <summary>
	/// Read once when the context is attached, then never again.
	/// </summary>
	OneTime,

	/// <summary>
	/// Source to control (default).
	/// </summary>
	OneWay,

	/// <summary>
	/// Both ways; needs an explicit setter.
	/// </summary>
	TwoWay,

	/// <summary>
	/// Control to source only; needs an explicit setter.
	/// </summary>
	OneWayToSource
}
