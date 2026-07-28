using Xunit;

namespace SkeleKit.Tests.Elements;

public class ContentViewLifecycleTests
{
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
