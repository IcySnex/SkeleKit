using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.ViewModels.Demos;

public class ContactsDemoViewModel
{
	static readonly string[] Names =
	[
		"Aaron Diaz", "Adam Cole", "Alice Newton", "Amir Khan", "Anna Brooks",
		"Ben Carter", "Bianca Rossi", "Brian Lee",
		"Carla Mendes", "Chloe Turner", "Colin Ward",
		"Dana Foster", "Derek Shaw", "Diana Cruz",
		"Elena Petrova", "Ethan Moore", "Evan Price",
		"Fiona Walsh", "Frank Obi",
		"Grace Kim", "Greg Nolan",
		"Hannah Reed", "Hugo Marin",
		"Ian Frost", "Isla Bennett",
		"Jack Owens", "Jamal Wright", "Julia Sato",
		"Karen Blake", "Kevin Ross",
		"Laura Diaz", "Leo Marsh", "Lily Chen",
		"Marco Bruno", "Maya Singh", "Mia Lawson",
		"Nadia Haddad", "Noah Park",
		"Olivia Grant", "Omar Faris",
		"Paula Reyes", "Peter Vance",
		"Rachel Stone", "Ravi Menon", "Rosa Iglesias",
		"Sam Porter", "Sofia Costa", "Simon Webb",
		"Tara Nolan", "Theo Ellis",
		"Uma Nair",
		"Victor Sole", "Vera Lang",
		"Wendy Cross", "Will Hobbs",
		"Yara Aziz",
		"Zoe Marsh", "Zane Kelly"
	];


	public List<ContactGroup> Groups { get; } =
		[.. Names
			.GroupBy(name => char.ToUpperInvariant(name[0]).ToString())
			.OrderBy(group => group.Key, StringComparer.Ordinal)
			.Select(group => new ContactGroup(group.Key, [.. group.OrderBy(name => name, StringComparer.Ordinal).Select(name => new Contact(name))]))];

	public string[] Alphabet { get; } =
		[.. Enumerable.Range('A', 26).Select(letter => ((char)letter).ToString())];
}
