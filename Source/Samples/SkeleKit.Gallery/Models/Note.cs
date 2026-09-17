namespace SkeleKit.Gallery.Models;

internal sealed record Note(
	string Title,
	string Summary,
	string Body);

internal sealed record Notebook(
	string Title,
	string Symbol,
	IReadOnlyList<Note> Notes)
{
	public string Summary =>
		Notes.Count == 1 ? "1 note" : $"{Notes.Count} notes";
}
