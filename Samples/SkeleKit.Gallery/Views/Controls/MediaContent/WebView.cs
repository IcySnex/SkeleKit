using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.MediaContent;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.MediaContent;

[Page]
internal sealed class WebView : ShowcaseView<WebViewModel>
{
	public WebView(
		WebViewModel viewModel) : base(viewModel, "Web View", Colors.Orange)
	{
		AddWebShowcase(viewModel);
	}


	void AddWebShowcase(
		WebViewModel viewModel)
	{
		SkeleKit.WebView web = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Height = 280,
			Url = Bind(model => model.Url),
			Html = Bind(model => model.Html),
			AllowsBackGestures = viewModel.AllowsBackGestures,
			Navigated = viewModel.RecordNavigation,
			NavigationFailed = viewModel.RecordFailure,
			CornerRadius = 18
		};

		Picker<ShowcaseOption<WebContentKind>> content = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.Contents,
			SelectedItem = viewModel.SelectedContent,
			SelectionChanged = viewModel.SelectContent
		};

		Switch gestures = new()
		{
			IsOn = viewModel.AllowsBackGestures,
			Toggled = value =>
			{
				viewModel.AllowsBackGestures = value;
				web.AllowsBackGestures = value;
			}
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

		AsyncRelayCommand evaluate = new(async () =>
		{
			try
			{
				string? result = await web.EvaluateAsync(
					"document.body.style.fontSize = '18px'; document.title || 'Untitled';");
				viewModel.JavaScriptStatus = $"JavaScript · {result ?? "no result"}";
			}
			catch (Exception exception)
			{
				viewModel.JavaScriptStatus = $"JavaScript failed · {exception.Message}";
			}
		});

		AddShowcase(
			"Content & navigation",
			"Load local HTML or a URL, navigate history, reload, evaluate JavaScript, and inspect callbacks.",
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
							Status(Bind(model => model.NavigationStatus)),
							Status(Bind(model => model.JavaScriptStatus))
						}
					},
					350),
				SettingRow("Content", content),
				SettingRow("Navigation", navigation),
				SettingRow("Back gestures", gestures),
				SettingRow(
					"JavaScript",
					new Button
					{
						Text = "Evaluate",
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Small,
						Command = evaluate
					})),
			ShowcaseBox.Code(Bind(model => model.WebViewCode)));
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
