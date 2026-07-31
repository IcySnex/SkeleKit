using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.MediaContent;

internal enum WebContentKind
{
	Local,
	Website,
	Failure
}

internal sealed partial class WebViewModel : ShowcaseViewModel
{
	const string LocalHtml = """
		<!doctype html>
		<html>
		<head>
			<meta name="viewport" content="width=device-width, initial-scale=1">
			<style>
				:root { color-scheme: light dark; font-family: -apple-system; }
				body { margin: 0; padding: 20px; background: transparent; }
				.card { padding: 20px; border-radius: 18px; background: color-mix(in srgb, #ff9500 16%, transparent); }
				h1 { margin: 0 0 8px; font-size: 24px; }
				p { margin: 0 0 16px; color: #8e8e93; line-height: 1.4; }
				button { border: 0; border-radius: 999px; padding: 10px 16px; color: white; background: #ff9500; font: inherit; font-weight: 600; }
			</style>
		</head>
		<body>
			<div class="card">
				<h1 id="title">Native web content</h1>
				<p id="copy">This page is bundled directly in the gallery.</p>
				<button onclick="openDetails()">Open details</button>
			</div>
			<script>
				function openDetails() {
					history.pushState({}, '', '#details');
					document.querySelector('#title').textContent = 'History entry';
					document.querySelector('#copy').textContent = 'Use the native back and forward controls below.';
				}
			</script>
		</body>
		</html>
		""";

	public WebViewModel()
	{
		SelectedContent = Contents[0];
		Html = LocalHtml;
	}


	public List<ShowcaseOption<WebContentKind>> Contents { get; } =
	[
		new("Local HTML", WebContentKind.Local),
		new("Website", WebContentKind.Website),
		new("Failure", WebContentKind.Failure)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(WebViewCode))]
	ShowcaseOption<WebContentKind> selectedContent = null!;

	[ObservableProperty]
	string? url;

	[ObservableProperty]
	string? html;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(WebViewCode))]
	bool allowsBackGestures = true;

	[ObservableProperty]
	string navigationStatus = "Waiting for the first navigation.";

	[ObservableProperty]
	string javaScriptStatus = "JavaScript has not run yet.";

	public IReadOnlyList<Span> WebViewCode =>
	[
		new(SelectedContent.Value is WebContentKind.Local
			? $$"""
				new WebView
				{
					Height = 280,
					Html = "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><h1>SkeleKit</h1>",
					AllowsBackGestures = {{Boolean(AllowsBackGestures)}},
					Navigated = viewModel.RecordNavigation,
					NavigationFailed = viewModel.RecordFailure
				};
				"""
			: $$"""
				new WebView
				{
					Height = 280,
					Url = "{{SelectedUrl}}",
					AllowsBackGestures = {{Boolean(AllowsBackGestures)}},
					Navigated = viewModel.RecordNavigation,
					NavigationFailed = viewModel.RecordFailure
				};
				""")
	];

	string SelectedUrl =>
		SelectedContent.Value is WebContentKind.Website
			? "https://example.com"
			: "https://example.invalid";


	internal void SelectContent(
		ShowcaseOption<WebContentKind> option)
	{
		SelectedContent = option;

		if (option.Value is WebContentKind.Local)
		{
			Url = null;
			Html = LocalHtml;
		}
		else
		{
			Html = null;
			Url = SelectedUrl;
		}
	}

	internal void RecordNavigation(
		string address) =>
		NavigationStatus = string.IsNullOrEmpty(address)
			? "Navigated · local HTML"
			: $"Navigated · {address}";

	internal void RecordFailure(
		string message) =>
		NavigationStatus = $"Failed · {message}";


	static string Boolean(
		bool value) =>
		value ? "true" : "false";
}
