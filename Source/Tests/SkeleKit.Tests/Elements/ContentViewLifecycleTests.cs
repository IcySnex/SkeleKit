using Xunit;

namespace SkeleKit.Tests.Elements;

public class ContentViewLifecycleTests
{
	sealed class EmptyView : ContentView
	{ }

	sealed class LifecycleView : ContentView
	{
		public List<string> Events { get; } = [];


		protected override void OnAppearing() =>
			Events.Add("Appearing");

		protected override void OnAppeared() =>
			Events.Add("Appeared");

		protected override void OnDisappearing() =>
			Events.Add("Disappearing");

		protected override void OnDisappeared() =>
			Events.Add("Disappeared");
	}

	[Fact]
	public void TitleStyle_DefaultsToAutomatic()
	{
		EmptyView view = new();

		Assert.Equal(TitleStyle.Automatic, view.TitleStyle);
	}

	[Fact]
	public void Background_DefaultsToSystemBackground()
	{
		EmptyView view = new();

		SolidBrush background = Assert.IsType<SolidBrush>(view.Background);
		Assert.Equal(Colors.Background, background.Color);
	}


	[Fact]
	public void Notifications_InvokeMatchingLifecycleHooks()
	{
		LifecycleView view = new();

		view.NotifyAppearing();
		view.NotifyAppeared();
		view.NotifyDisappearing();
		view.NotifyDisappeared();

		Assert.Equal(
			["Appearing", "Appeared", "Disappearing", "Disappeared"],
			view.Events);
	}
}
