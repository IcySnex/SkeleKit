namespace SkeleKit;

/// <summary>
/// A built-in animation an SF Symbol can perform.
/// </summary>
public enum SymbolEffect
{
	/// <summary>
	/// No effect.
	/// </summary>
	None,

	/// <summary>
	/// Scales the symbol up and back, like a tap acknowledgement.
	/// </summary>
	Bounce,

	/// <summary>
	/// Fades the symbol's opacity in and out.
	/// </summary>
	Pulse,

	/// <summary>
	/// Steps through the symbol's variable layers, like an ongoing transfer.
	/// </summary>
	VariableColor,

	/// <summary>
	/// Smoothly scales the symbol up and down, like a calm breath.
	/// </summary>
	Breathe,

	/// <summary>
	/// Rocks the symbol side to side, drawing attention.
	/// </summary>
	Wiggle,

	/// <summary>
	/// Spins the symbol's rotatable parts.
	/// </summary>
	Rotate
}

/// <summary>
/// The relative size an SF Symbol is drawn at within its font metrics.
/// </summary>
public enum SymbolScale
{
	/// <summary>
	/// The symbol's own default.
	/// </summary>
	Default,

	/// <summary>
	/// Small.
	/// </summary>
	Small,

	/// <summary>
	/// Medium.
	/// </summary>
	Medium,

	/// <summary>
	/// Large.
	/// </summary>
	Large
}
