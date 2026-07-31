using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.MediaContent;

internal sealed partial class WebViewModel : ShowcaseViewModel
{
	internal const string WebsiteUrl = "https://github.com/IcySnex/SkeleKit";

	internal const string LocalHtml = """
		<!doctype html>
		<html>
		<head>
			<meta name="viewport" content="width=device-width, initial-scale=1">
			<style>
				:root { color-scheme: light dark; font-family: -apple-system; }
				body { margin: 0; padding: 20px; background: transparent; }
				.card { padding: 20px; background: rgba(255, 149, 0, 0.16); }
				p { margin: 0 0 16px; color: #8e8e93; line-height: 1.4; }
				button { border: 0; padding: 10px 16px; color: white; background: #ff9500; font: inherit; font-weight: 600; }
			</style>
		</head>
		<body>
			<div class="card" data-color="orange">
				<h1 id="title">Native web content</h1>
				<p id="copy">This page is bundled directly in the gallery.</p>
				<button onclick="showDetails()">Show details</button>
			</div>
			<script>
				function showDetails() {
					document.querySelector('#title').textContent = 'JavaScript is active';
					document.querySelector('#copy').textContent = 'wuhuuu yipeee.';
				}
			</script>
		</body>
		</html>
		""";


	[ObservableProperty]
	string localStatus = "Waiting for the local document.";

	[ObservableProperty]
	string websiteStatus = "Opening the SkeleKit repository.";

	[ObservableProperty]
	string javaScriptStatus = "JavaScript has not run yet.";

	public IReadOnlyList<Span> HtmlCode { get; } =
	[
		new(
			""""
			WebView web = new()
			{
				Height = 240,
				Html = """
					<meta name="viewport" content="width=device-width, initial-scale=1">
					<div class="card">Bundled HTML</div>
					""",
				Navigated = viewModel.RecordLocalNavigation,
				NavigationFailed = viewModel.RecordLocalFailure
			};

			string? result = await web.EvaluateAsync(
				"document.querySelector('.card').style.background = '#0a84ff'; 'Blue';");
			"""")
	];

	public IReadOnlyList<Span> WebsiteCode { get; } =
	[
		new(
			"""
			WebView web = new()
			{
				Height = 300,
				Url = "https://github.com/IcySnex/SkeleKit",
				Navigated = viewModel.RecordWebsiteNavigation,
				NavigationFailed = viewModel.RecordWebsiteFailure
			};

			web.GoBack();
			web.GoForward();
			web.Reload();
			""")
	];


	internal void RecordLocalNavigation(
		string address) =>
		LocalStatus = string.IsNullOrEmpty(address)
			? "Loaded · bundled HTML"
			: $"Loaded · {address}";

	internal void RecordLocalFailure(
		string message) =>
		LocalStatus = $"Failed · {message}";

	internal void RecordWebsiteNavigation(
		string address) =>
		WebsiteStatus = $"Loaded · {address}";

	internal void RecordWebsiteFailure(
		string message) =>
		WebsiteStatus = $"Failed · {message}";
}
