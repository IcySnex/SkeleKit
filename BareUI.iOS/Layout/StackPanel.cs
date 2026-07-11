
namespace BareUI;

/// <summary>
/// Stacks its children in a single line, vertically (default) or horizontally, with optional spacing.
/// </summary>
public class StackPanel : Panel
{
	/// <summary>
	/// The stacking axis.
	/// </summary>
	public Orientation Orientation { get; set; } = Orientation.Vertical;

	/// <summary>
	/// The gap in points inserted between consecutive visible children.
	/// </summary>
	public double Spacing { get; set; } = 0;


	protected override Size MeasureOverride(
		Size availableSize)
	{
		bool vertical = Orientation == Orientation.Vertical;

		double along = 0;
		double across = 0;
		int visible = 0;

		foreach (View child in Children)
		{
			Size childAvailable = vertical
				? new(availableSize.Width, double.PositiveInfinity)
				: new(double.PositiveInfinity, availableSize.Height);

			child.Measure(childAvailable);

			if (!child.IsVisible)
				continue;

			Size desired = child.DesiredSize;
			if (vertical)
			{
				along += desired.Height;
				across = Math.Max(across, desired.Width);
			}
			else
			{
				along += desired.Width;
				across = Math.Max(across, desired.Height);
			}

			visible++;
		}

		along += Spacing * Math.Max(0, visible - 1);

		return vertical
			? new(across, along)
			: new(along, across);
	}

	protected override Size ArrangeOverride(
		Size finalSize)
	{
		bool vertical = Orientation == Orientation.Vertical;

		double offset = 0;
		bool first = true;

		foreach (View child in Children)
		{
			if (!child.IsVisible)
			{
				child.Arrange(new(Point.Zero, Size.Zero));
				continue;
			}

			if (!first)
				offset += Spacing;
			first = false;

			Size desired = child.DesiredSize;
			Rect slot = vertical
				? new(0, offset, finalSize.Width, desired.Height)
				: new(offset, 0, desired.Width, finalSize.Height);

			child.Arrange(slot);
			offset += vertical ? desired.Height : desired.Width;
		}

		return finalSize;
	}
}

/// <summary>
/// A <see cref="StackPanel"/> preset to <see cref="Orientation.Vertical"/>.
/// </summary>
public sealed class VStack : StackPanel
{
	public VStack()
	{
		Orientation = Orientation.Vertical;
	}
}

/// <summary>
/// A <see cref="StackPanel"/> preset to <see cref="Orientation.Horizontal"/>.
/// </summary>
public sealed class HStack : StackPanel
{
	public HStack()
	{
		Orientation = Orientation.Horizontal;
	}
}
