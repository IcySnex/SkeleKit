using SkeleKit.Tests.Binding;
using Xunit;

namespace SkeleKit.Tests.Styling;

public class StyleTests
{
	[Fact]
	public void Apply_RunsSetters()
	{
		Style<StubStyled> style = new(view => view.Tag = "caption");
		StubStyled view = new();

		style.Apply(view);

		Assert.Equal("caption", view.Tag);
	}

	[Fact]
	public void Style_AppliesOnAssignment()
	{
		Style<StubStyled> style = new(view => view.Opacity = 0.5);

		StubStyled view = new() { Style = style };

		Assert.Equal(0.5, view.Opacity);
	}

	// the whole precedence model: whatever the initializer writes after the style wins
	[Fact]
	public void LocalValue_AfterStyle_Wins()
	{
		Style<StubStyled> style = new(view => view.Opacity = 0.5);

		StubStyled view = new()
		{
			Style = style,
			Opacity = 0.25
		};

		Assert.Equal(0.25, view.Opacity);
	}

	[Fact]
	public void BasedOn_AppliesBaseFirst()
	{
		Style<StubStyled> card = new(view =>
		{
			view.CornerRadius = 12;
			view.Opacity = 0.5;
		});
		Style<StubStyled> prominent = new(card, view => view.Opacity = 1);

		StubStyled view = new() { Style = prominent };

		Assert.Equal(12, view.CornerRadius);
		Assert.Equal(1, view.Opacity);
	}

	[Fact]
	public void BasedOn_AcceptsStyleOfBaseType()
	{
		Style<StubStyled> baseStyle = new(view => view.CornerRadius = 8);
		Style<StubStyledLeaf> derived = new(baseStyle, view => view.Tag = "leaf");

		StubStyledLeaf view = new() { Style = derived };

		Assert.Equal(8, view.CornerRadius);
		Assert.Equal("leaf", view.Tag);
	}

	[Fact]
	public void BasedOn_RejectsUnrelatedTargetType()
	{
		Style<StubOther> other = new(view => view.CornerRadius = 8);

		ArgumentException exception = Assert.Throws<ArgumentException>(() => new Style<StubStyled>(other, view => view.Tag = "x"));

		Assert.Contains(nameof(StubOther), exception.Message);
		Assert.Contains(nameof(StubStyled), exception.Message);
	}

	[Fact]
	public void Apply_ToWrongType_Throws()
	{
		IStyle style = new Style<StubStyled>(view => view.Tag = "x");

		InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => style.Apply(new StubOther()));

		Assert.Contains(nameof(StubStyled), exception.Message);
		Assert.Contains(nameof(StubOther), exception.Message);
	}

	// a style holding a Bind expression is shared: each view must end up with its own live binding
	[Fact]
	public void Apply_RegistersAFreshBindingPerView()
	{
		Style<StubStyled> style = new(view => view.Label = BindingFactory.Bind((MovieViewModel vm) => vm.Title));

		StubStyled first = new() { Style = style, BindingContext = new MovieViewModel { Title = "Interstellar" } };
		StubStyled second = new() { Style = style, BindingContext = new MovieViewModel { Title = "Dune" } };

		Assert.Equal("Interstellar", first.CurrentLabel);
		Assert.Equal("Dune", second.CurrentLabel);
	}
}
