namespace SkeleKit;

/// <summary>
/// Base for a view that decorates one child.
/// </summary>
public abstract class Decorator : Container
{
	/// <summary>
	/// The single decorated child.
	/// </summary>
	public View? Child
	{
		get => LogicalChildren.Count > 0 ? LogicalChildren[0] : null;
		set
		{
			LogicalChildren.Clear();

			if (value is not null)
				LogicalChildren.Add(value);
		}
	}
}
