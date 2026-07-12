using Xunit;

namespace BareUI.Tests.Primitives;

public class AnimationTests
{
	[Fact]
	public void Default_IsAShortEasedCurve()
	{
		Animation animation = Animation.Default;

		Assert.Equal(0.3, animation.Duration);
		Assert.Equal(Easing.EaseInOut, animation.Easing);
		Assert.Null(animation.SpringDamping);
		Assert.Equal(0, animation.Delay);
	}

	// a spring is the absence of a curve: the damping is what the native side switches on
	[Fact]
	public void Spring_CarriesDampingInsteadOfACurve()
	{
		Animation animation = Animation.Spring(duration: 0.6, damping: 0.5);

		Assert.Equal(0.6, animation.Duration);
		Assert.Equal(0.5, animation.SpringDamping);
	}

	[Fact]
	public void Ease_KeepsItsCurveAndHasNoSpring()
	{
		Animation animation = Animation.Ease(1.2, Easing.EaseOut);

		Assert.Equal(1.2, animation.Duration);
		Assert.Equal(Easing.EaseOut, animation.Easing);
		Assert.Null(animation.SpringDamping);
	}

	[Fact]
	public void After_OnlyDelaysTheAnimation()
	{
		Animation animation = Animation.Spring().After(0.25);

		Assert.Equal(0.25, animation.Delay);
		Assert.Equal(Animation.Spring().SpringDamping, animation.SpringDamping);
		Assert.Equal(Animation.Spring().Duration, animation.Duration);
	}
}
