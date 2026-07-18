using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates the embeddable <see cref="WebView"/> and the in-app Safari browser (OpenUrlAsync).
/// </summary>
[Page]
public class WebViewDemo : ContentView<WebViewDemoViewModel>
{
	public WebViewDemo(
		WebViewDemoViewModel viewModel) : base(viewModel)
	{
		Title = "WebView";

		WebView web = new()
		{
			AllowsBackGestures = true,
			Url = Bind(vm => vm.Url),
			Navigated = address => Prompt = address
		};

		Content = web;

		ToolbarItems.Add(new() { Icon = "chevron.forward", Command = Command.From(web.GoForward) });
		ToolbarItems.Add(new() { Icon = "chevron.backward", Command = Command.From(web.GoBack) });
		ToolbarItems.Add(new() { Icon = "arrow.clockwise", Command = Command.From(web.Reload) });
		ToolbarItems.Add(new() { Icon = "safari", Command = Command.From(() => _ = Navigator.OpenUrlAsync(viewModel.Url)) });
		ToolbarItems.Add(new() { Icon = "square.and.arrow.up", Command = Command.From(() => _ = Share()) });

		// each choice shares a different item kind, so the sheet's actions visibly change
		async Task Share()
		{
			Uri url = new(viewModel.Url);

			switch (await Navigator.ActionSheetAsync("Share", "Cancel", "Text", "Link", "Image", "Text + link + image"))
			{
				case "Text":
					await Sharer.ShareAsync("Check out this page");
					break;
				case "Link":
					await Sharer.ShareAsync(url);
					break;
				case "Image":
					await Sharer.ShareAsync(ImageSource.Symbol("globe"));
					break;
				case "Text + link + image":
					await Sharer.ShareAsync("Check out this page", url, ImageSource.Symbol("globe"));
					break;
			}
		}
	}
}
