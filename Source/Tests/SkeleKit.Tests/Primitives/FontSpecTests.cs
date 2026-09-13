using Xunit;

namespace SkeleKit.Tests.Primitives;

public class FontSpecTests
{
	[Fact]
	public void TextStyle_WithoutSize_Wins()
	{
		Assert.True(FontSpec.UsesTextStyle(TextStyle.Headline, double.NaN));
	}

	// whatever the order they were written in, an explicit size overrides the text style
	[Fact]
	public void ExplicitSize_BeatsTextStyle()
	{
		Assert.False(FontSpec.UsesTextStyle(TextStyle.Headline, 24));
	}

	[Fact]
	public void NoTextStyle_FallsBackToSize()
	{
		Assert.False(FontSpec.UsesTextStyle(null, double.NaN));
	}

	[Fact]
	public void UnsetSize_ResolvesToTheBodyDefault()
	{
		Assert.Equal(17, FontSpec.SizeOf(double.NaN));
	}

	[Fact]
	public void SetSize_ResolvesToItself()
	{
		Assert.Equal(24, FontSpec.SizeOf(24));
	}

	[Fact]
	public void SpanTextStyle_OverridesTheContainingStyle()
	{
		Span span = new("heading") { TextStyle = TextStyle.Headline };
		TextStyle? containingStyle = TextStyle.Body;

		Assert.Equal(TextStyle.Headline, span.TextStyle ?? containingStyle);
		Assert.True(FontSpec.UsesTextStyle(span.TextStyle ?? containingStyle, double.NaN));
	}

	[Fact]
	public void SpanWithoutTextStyle_InheritsTheContainingStyle()
	{
		Span span = new("body");
		TextStyle? containingStyle = TextStyle.Body;

		Assert.Equal(TextStyle.Body, span.TextStyle ?? containingStyle);
	}
}
