using BareUI;
using Xunit;

namespace BareUI.Tests.Elements;

/// <summary>
/// A leaf view with a fixed content size, standing in for a native control during
/// neutral-TFM layout tests.
/// </summary>
file sealed class FixedView : View
{
	private readonly Size content;

	public FixedView(
		double width,
		double height)
	{
		content = new Size(width, height);
	}

	protected override Size MeasureOverride(
		Size availableSize) =>
		content;
}

public class ViewLayoutTests
{
	[Fact]
	public void Measure_AddsMarginToDesiredSize()
	{
		FixedView view = new(100, 40) { Margin = new Thickness(10) };

		view.Measure(Size.Infinity);

		Assert.Equal(new Size(120, 60), view.DesiredSize);
	}

	[Fact]
	public void Measure_ExplicitSize_OverridesContent()
	{
		FixedView view = new(100, 40) { Width = 200, Height = 80 };

		view.Measure(Size.Infinity);

		Assert.Equal(new Size(200, 80), view.DesiredSize);
	}

	[Fact]
	public void Measure_MaxConstraint_Clamps()
	{
		FixedView view = new(500, 40) { MaxWidth = 300 };

		view.Measure(Size.Infinity);

		Assert.Equal(300, view.DesiredSize.Width);
	}

	[Fact]
	public void Measure_MinConstraint_Clamps()
	{
		FixedView view = new(10, 40) { MinWidth = 120 };

		view.Measure(Size.Infinity);

		Assert.Equal(120, view.DesiredSize.Width);
	}

	[Fact]
	public void Measure_NotVisible_IsZero()
	{
		FixedView view = new(100, 40) { IsVisible = false };

		view.Measure(Size.Infinity);

		Assert.Equal(Size.Zero, view.DesiredSize);
	}

	[Fact]
	public void Arrange_Stretch_FillsSlot()
	{
		FixedView view = new(100, 40);
		view.Measure(new Size(300, 200));

		view.Arrange(new Rect(0, 0, 300, 200));

		Assert.Equal(new Rect(0, 0, 300, 200), view.ArrangedBounds);
	}

	[Fact]
	public void Arrange_CenterCenter_CentersDesiredSize()
	{
		FixedView view = new(100, 40)
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center
		};
		view.Measure(new Size(300, 200));

		view.Arrange(new Rect(0, 0, 300, 200));

		Assert.Equal(new Rect(100, 80, 100, 40), view.ArrangedBounds);
	}

	[Fact]
	public void Arrange_EndAlignment_PinsToTrailingBottom()
	{
		FixedView view = new(100, 40)
		{
			HorizontalAlignment = HorizontalAlignment.End,
			VerticalAlignment = VerticalAlignment.End
		};
		view.Measure(new Size(300, 200));

		view.Arrange(new Rect(0, 0, 300, 200));

		Assert.Equal(new Rect(200, 160, 100, 40), view.ArrangedBounds);
	}

	[Fact]
	public void Arrange_Margin_InsetsSlotAndStretches()
	{
		FixedView view = new(100, 40) { Margin = new Thickness(10, 20, 30, 40) };
		view.Measure(new Size(300, 200));

		view.Arrange(new Rect(0, 0, 300, 200));

		// slot 300x200 minus margin (l10,t20,r30,b40) → x10,y20,w260,h140
		Assert.Equal(new Rect(10, 20, 260, 140), view.ArrangedBounds);
	}

	[Fact]
	public void Arrange_StartWithMargin_UsesDesiredSizeAtLeadingEdge()
	{
		FixedView view = new(100, 40)
		{
			Margin = new Thickness(10),
			HorizontalAlignment = HorizontalAlignment.Start,
			VerticalAlignment = VerticalAlignment.Start
		};
		view.Measure(new Size(300, 200));

		view.Arrange(new Rect(0, 0, 300, 200));

		Assert.Equal(new Rect(10, 10, 100, 40), view.ArrangedBounds);
	}
}
