namespace SkeleKit;

/// <summary>
/// Marks a view for automatic page registration.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class PageAttribute : Attribute
{
	/// <summary>
	/// Whether one instance is kept for the app's lifetime.
	/// </summary>
	public bool Singleton { get; set; }
}
