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
			sealed class GalleryPage : ContentView
			{
				public GalleryPage()
				{
					Title = "Page chrome";
					TitleStyle = TitleStyle.{{SelectedTitleStyle}};
					Prompt = {{(ShowsPrompt ? "\"ContentView\"" : "null")}};
					HidesTabBar = {{Bool(HidesTabBar)}};
					BackgroundStyle = PageBackground.Grouped;
					BackButtonStyle = BackButtonStyle.Generic;
					ScrollsUnderBars = true;

					Content = new ScrollView
					{
						Content = new StackPanel { Padding = 16 }
					};
				}
			}
			""");

	public IReadOnlyList<Span> SearchCode { get; } =
		Code(
			"""
			sealed class SearchPage : ContentView
			{
				readonly Label status = new();

				public SearchPage()
				{
					Title = "Search";
					TitleStyle = TitleStyle.Large;
					SearchPlaceholder = "Search gallery";
					SearchScopes.Add("All");
					SearchScopes.Add("Recent");
					SearchScopes.Add("Saved");

					SearchChanged = query =>
						status.Text = $"Typing: {query}";
					SearchScopeChanged = index =>
						status.Text = $"Scope: {SearchScopes[index]}";
					SearchCommand = Command.From<string>(query =>
						status.Text = $"Submitted: {query}");
					SearchCanceled = () =>
						status.Text = "Search cancelled";

					Content = status;
				}
			}
			""");

	public IReadOnlyList<Span> LifecycleCode { get; } =
		Code(
			"""
			sealed class LifecyclePage : ContentView
			{
				protected override void OnLoaded() =>
					Record("Loaded");

				protected override void OnUnloaded() =>
					Record("Unloaded");

				protected override void OnAppearing() =>
					Record("Appearing");

				protected override void OnAppeared() =>
					Record("Appeared");

				protected override void OnDisappearing() =>
					Record("Disappearing");

				protected override void OnDisappeared() =>
					Record("Disappeared");

				void GuardLeaving(bool enabled) =>
					ConfirmLeave = enabled
						? () => Navigator.ConfirmAsync(
							"Leave page?",
							"Leave confirmation is enabled.",
							"Leave",
							"Stay")
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
