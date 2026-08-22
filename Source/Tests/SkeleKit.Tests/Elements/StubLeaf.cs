using SkeleKit;

namespace SkeleKit.Tests.Elements;

/// <summary>
/// A leaf view with a fixed content size, standing in for a native control when unit-testing
/// panel layout math in the neutral target framework.
/// </summary>
internal sealed class StubLeaf : View
{
	private readonly Size content;

	public StubLeaf(
		double width,
		double height)
	{
		content = new Size(width, height);
	}

	/// <summary>
	/// How often MeasureOverride actually ran — the measure cache should keep this down.
	/// </summary>
	public int MeasureCount { get; private set; }

	protected override Size MeasureOverride(
		Size availableSize)
	{
		MeasureCount++;

		return content;
	}
}
