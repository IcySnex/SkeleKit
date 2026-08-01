using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Controls.MediaContent;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.MediaContent;

[Page]
internal sealed class WebView : ShowcaseView<WebViewModel>
{
	public WebView(
		WebViewModel viewModel) : base(viewModel, "Web View", Colors.Orange)
	{
		AddHtmlShowcase(viewModel);
		AddWebsiteShowcase(viewModel);
	}


	void AddHtmlShowcase(
		WebViewModel viewModel)
	{
		SkeleKit.WebView web = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Height = 240,
			Html = WebViewModel.LocalHtml,
			Navigated = viewModel.RecordLocalNavigation,
			NavigationFailed = viewModel.RecordLocalFailure,
			CornerRadius = 18
		};

		AsyncRelayCommand evaluate = new(async () =>
		{
			try
			{
				string? result = await web.EvaluateAsync(
					"""
					(() => {
						const card = document.querySelector('.card');
						const button = document.querySelector('button');
						const blue = card.dataset.color === 'blue';
						card.dataset.color = blue ? 'orange' : 'blue';
						card.style.background = blue ? 'rgba(255, 149, 0, 0.16)' : 'rgba(10, 132, 255, 0.16)';
						button.style.background = blue ? '#ff9500' : '#0a84ff';
						return blue ? 'Orange' : 'Blue';
					})()
					""");
				viewModel.JavaScriptStatus = $"JavaScript · {result ?? "no result"}";
			}
			catch (Exception exception)
			{
				viewModel.JavaScriptStatus = $"JavaScript failed · {exception.Message}";
			}
		});

		AddShowcase(
			"HTML & JavaScript",
			"Load a bundled document, interact with its own script, and update its colors from native code.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 8,

						Children =
						{
							web,
							Status(Bind(model => model.LocalStatus)),
							Status(Bind(model => model.JavaScriptStatus))
						}
					},
					320),
				SettingRow(
					"JavaScript",
					new Button
					{
						Text = "Change color",
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Small,
						Command = evaluate
					})),
			ShowcaseBox.Code(Bind(model => model.HtmlCode)));
	}

	void AddWebsiteShowcase(
		WebViewModel viewModel)
	{
		SkeleKit.WebView web = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Height = 300,
			Url = WebViewModel.WebsiteUrl,
			Navigated = viewModel.RecordWebsiteNavigation,
			NavigationFailed = viewModel.RecordWebsiteFailure,
			CornerRadius = 18
		};

		StackPanel navigation = new()
		{
			Orientation = Orientation.Horizontal,
			Spacing = 4,

			Children =
			{
				ActionButton("chevron.backward", new RelayCommand(web.GoBack)),
				ActionButton("chevron.forward", new RelayCommand(web.GoForward)),
				ActionButton("arrow.clockwise", new RelayCommand(web.Reload))
			}
		};

		AddShowcase(
			"Website & navigation",
			"Browse the SkeleKit repository with native history controls.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 8,

						Children =
						{
							web,
							Status(Bind(model => model.WebsiteStatus))
						}
					},
					350),
				SettingRow("Navigation", navigation)),
			ShowcaseBox.Code(Bind(model => model.WebsiteCode)));
	}


	static Button ActionButton(
		string icon,
		ICommand command) =>
		new()
		{
			Icon = icon,
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Small,
			Command = command
		};

	static Label Status(
		BindingExpression<string?> text) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = text,
			TextStyle = TextStyle.Caption1,
			TextColor = Colors.SecondaryLabel,
			MaxLines = 2,
			TextAlignment = TextAlignment.Center
		};
}
