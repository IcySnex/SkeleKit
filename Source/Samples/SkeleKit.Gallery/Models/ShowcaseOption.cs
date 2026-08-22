namespace SkeleKit.Gallery.Models;

internal sealed record ShowcaseOption<T>(
	string Title,
	T Value)
{
	public override string ToString() =>
		Title;
}
