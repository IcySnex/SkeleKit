namespace SkeleKit;

/// <summary>
/// Base for layouts that measure and arrange multiple children.
/// </summary>
public abstract class Panel : Container
{
	/// <summary>
	/// The panel's children. Collection-initializer friendly.
	/// </summary>
	public ViewCollection Children => LogicalChildren;
}
