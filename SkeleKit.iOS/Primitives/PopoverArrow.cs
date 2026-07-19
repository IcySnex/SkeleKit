namespace SkeleKit;

/// <summary>
/// The directions a popover's arrow may point.
/// </summary>
[Flags]
public enum PopoverArrow
{
	/// <summary>
	/// Points up, from a popover below its anchor.
	/// </summary>
	Up = 1,

	/// <summary>
	/// Points down, from a popover above its anchor.
	/// </summary>
	Down = 2,

	/// <summary>
	/// Points left.
	/// </summary>
	Left = 4,

	/// <summary>
	/// Points right.
	/// </summary>
	Right = 8,

	/// <summary>
	/// Any direction the system prefers.
	/// </summary>
	Any = Up | Down | Left | Right
}
