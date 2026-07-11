using System.Diagnostics.CodeAnalysis;
using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Root menu listing every control demo. Tapping an entry invokes <paramref name="push"/> to show
/// its page — the actual navigation controller lives in the bootstrap layer (<c>SceneDelegate</c>),
/// keeping this page tree pure BareUI.
/// </summary>
public static class MenuPage
{
	static readonly (string Title, Func<View> Build)[] entries =
	[
		("Binding", () => new BindingPage { ViewModel = new() }),
		("MovieInfo", MovieInfoPage.Build),
		("Button", ButtonPage.Build),
		("TextField", TextFieldPage.Build),
		("TextEditor", TextEditorPage.Build),
		("Switch", SwitchPage.Build),
		("Slider", SliderPage.Build),
		("Stepper", StepperPage.Build),
		("ProgressBar", ProgressBarPage.Build),
		("ActivityIndicator", ActivityIndicatorPage.Build),
		("Divider", DividerPage.Build),
		("Picker", PickerPage.Build),
		("Image", ImagePage.Build),
		("NativeView", NativeViewPage.Build)
	];

	public static View Build(
		Action<string, View> push)
	{
		VStack list = new()
		{
			Spacing = 12,
			Margin = new Thickness(16)
		};

		foreach ((string title, Func<View> build) in entries)
			list.Children.Add(new Button
			{
				Text = title,
				Style = ButtonStyle.Gray,
				Clicked = () => push(title, build())
			});

		return new ScrollView { Content = list };
	}

	/// <summary>Builds the page whose title matches <paramref name="title"/>, if any.</summary>
	public static bool TryBuild(
		string title,
		[NotNullWhen(true)] out View? page)
	{
		foreach ((string entryTitle, Func<View> build) in entries)
			if (entryTitle == title)
			{
				page = build();
				return true;
			}

		page = null;
		return false;
	}
}
