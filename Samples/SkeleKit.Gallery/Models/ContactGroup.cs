namespace SkeleKit.Gallery.Models;

/// <summary>
/// A single contact.
/// </summary>
public record Contact(
	string Name);

/// <summary>
/// A letter group of contacts. The section model is the app's own: the library only asks for <c>Items</c>.
/// </summary>
public record ContactGroup(
	string Letter,
	IReadOnlyList<Contact> Items) : ISection<Contact>;
