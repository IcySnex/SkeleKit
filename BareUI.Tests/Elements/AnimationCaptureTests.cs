using Xunit;

namespace BareUI.Tests.Elements;

public class AnimationCaptureTests
{
	[Fact]
	public void Run_RecordsTheStateBeforeTheChange()
	{
		StubLeaf view = new(10, 10) { Opacity = 1 };

		Dictionary<View, ViewState> captured = AnimationCapture.Run(() => view.Opacity = 0);

		Assert.Equal(1, captured[view].Opacity);
		Assert.Equal(0, view.Opacity);
	}

	// the animation's end values land in the model; a revert has to undo all of them at once
	[Fact]
	public void Restore_PutsEveryTouchedViewBack()
	{
		StubLeaf first = new(10, 10);
		StubLeaf second = new(10, 10);

		Dictionary<View, ViewState> captured = AnimationCapture.Run(() =>
		{
			first.Translation = new(240, 0);
			first.Rotation = 6;
			second.Opacity = 0.5;
		});

		foreach ((View view, ViewState state) in captured)
			view.Restore(state);

		Assert.Equal(Point.Zero, first.Translation);
		Assert.Equal(0, first.Rotation);
		Assert.Equal(1, second.Opacity);
	}

	// only the pre-animation state may be kept: the second write is already part of the animation
	[Fact]
	public void Run_RecordsAViewOnlyOnce()
	{
		StubLeaf view = new(10, 10);

		Dictionary<View, ViewState> captured = AnimationCapture.Run(() =>
		{
			view.Scale = 2;
			view.Scale = 3;
		});

		Assert.Equal(1, captured[view].Scale);
	}

	[Fact]
	public void Run_LeavesUntouchedViewsAlone()
	{
		StubLeaf touched = new(10, 10);
		StubLeaf untouched = new(10, 10);

		Dictionary<View, ViewState> captured = AnimationCapture.Run(() => touched.Opacity = 0);

		Assert.DoesNotContain(untouched, captured.Keys);
	}

	// nothing may leak between animations: a set outside a scope is not part of one
	[Fact]
	public void Set_OutsideAScope_RecordsNothing()
	{
		StubLeaf view = new(10, 10);
		view.Opacity = 0.25;

		Dictionary<View, ViewState> captured = AnimationCapture.Run(() => { });

		Assert.Empty(captured);
	}

	// the whole point: an unchanged value never reaches Set's body, so it must not be captured either
	[Fact]
	public void Run_IgnoresAWriteThatChangesNothing()
	{
		StubLeaf view = new(10, 10) { Scale = 2 };

		Dictionary<View, ViewState> captured = AnimationCapture.Run(() => view.Scale = 2);

		Assert.Empty(captured);
	}
}
