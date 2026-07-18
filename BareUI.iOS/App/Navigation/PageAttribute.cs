namespace BareUI;

/// <summary>
/// Marks a view for generated page registration.
/// </summary>
/// <remarks>
/// The generator emits the UsePages() extension.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class PageAttribute : Attribute
{
	/// <summary>
	/// Whether one instance is kept for the app's lifetime.
	/// </summary>
	public bool Singleton { get; set; }
}
