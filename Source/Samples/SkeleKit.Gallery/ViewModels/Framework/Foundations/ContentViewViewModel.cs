using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Foundations;

internal sealed class ContentViewViewModel : ShowcaseViewModel
{
	public IReadOnlyList<Span> CompositionCode { get; } =
		Code(
			"""
			ContentView page = new()
			{
				Title = "Content",
				BackgroundStyle = PageBackground.Grouped,
				Content = new StackPanel
				{
					Padding = 16,
					Children =
					{
						new Label { Text = "Compose the page tree here." }
					}
				}
			};
			""");

	public IReadOnlyList<Span> LifecycleCode { get; } =
		Code(
			"""
			sealed class GalleryPage : ContentView
			{
				protected override void OnLoaded() => Record("Loaded");
				protected override void OnUnloaded() => Record("Unloaded");
				protected override void OnAppearing() => Record("Appearing");
				protected override void OnAppeared() => Record("Appeared");
				protected override void OnDisappearing() => Record("Disappearing");
				protected override void OnDisappeared() => Record("Disappeared");

				public void RequireConfirmation(bool enabled) =>
					ConfirmLeave = enabled
						? () => Navigator.ConfirmAsync(
							"Leave page?",
							"Unsaved changes will be lost.")
						: null;
			}
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
