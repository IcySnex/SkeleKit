namespace SkeleKit;

/// <summary>
/// Base for a view that hosts one piece of visual content.
/// </summary>
public abstract class ContentHost : Container
{
	/// <summary>
	/// The single hosted view.
	/// </summary>
	public View? Content
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
