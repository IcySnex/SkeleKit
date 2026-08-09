using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Foundations;

internal sealed partial class ContentViewViewModel : ShowcaseViewModel
{
	static readonly TitleStyle[] TitleStyles =
	[
		TitleStyle.Large,
		TitleStyle.Inline
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedTitleStyle))]
	[NotifyPropertyChangedFor(nameof(ChromeCode))]
	int titleStyleIndex;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ChromeCode))]
	bool showsPrompt;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ChromeCode))]
	bool hidesTabBar = true;

	internal TitleStyle SelectedTitleStyle =>
		TitleStyles[Math.Clamp(TitleStyleIndex, 0, TitleStyles.Length - 1)];

	public IReadOnlyList<Span> ChromeCode =>
		Code(
			$$"""
			ContentView page = new()
			{
				Title = "Page chrome",
				TitleStyle = TitleStyle.{{SelectedTitleStyle}},
				Prompt = {{(ShowsPrompt ? "\"ContentView\"" : "null")}},
				HidesTabBar = {{Bool(HidesTabBar)}},
				BackgroundStyle = PageBackground.Grouped,
				BackButtonStyle = BackButtonStyle.Generic,
				ScrollsUnderBars = true,

				Content = new ScrollView
				{
					Content = new StackPanel { Padding = 16 }
				}
			};
			""");

	public IReadOnlyList<Span> SearchCode { get; } =
		Code(
			"""
			Label status = new();
			ContentView page = new()
			{
				Title = "Search",
				SearchPlaceholder = "Search gallery",
				SearchChanged = query => status.Text = $"Typing: {query}",
				SearchCommand = Command.From<string>(query =>
					status.Text = $"Submitted: {query}"),
				SearchCanceled = () => status.Text = "Search cancelled",
				Content = status
			};

			page.SearchScopes.Add("All");
			page.SearchScopes.Add("Recent");
			page.SearchScopes.Add("Saved");
			page.SearchScopeChanged = index =>
				status.Text = $"Scope: {page.SearchScopes[index]}";
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

	static string Bool(
		bool value) =>
		value ? "true" : "false";
}
