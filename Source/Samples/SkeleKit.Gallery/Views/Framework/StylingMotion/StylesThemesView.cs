using SkeleKit.Gallery.ViewModels.Framework.StylingMotion;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.StylingMotion;

[Page]
internal sealed class StylesThemesView : ShowcaseView<StylesThemesViewModel>
{
	public StylesThemesView(
		StylesThemesViewModel viewModel) : base(viewModel, "Styles & Themes", Colors.Cyan)
	{
		AddStyleShowcase(viewModel);
	}


	void AddStyleShowcase(
		StylesThemesViewModel viewModel)
	{
		Overlay preview = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 300,
			Height = 180,
			Children =
			{
				CreateCard(viewModel.ModeIndex, viewModel.ModeTitle, viewModel.ModeDetail)
			}
		};

		SegmentedControl mode = new()
		{
			SelectedIndex = Bind(vm => vm.ModeIndex)
				.TwoWay((vm, val) => vm.ModeIndex = val),
			SelectionChanged = index =>
			{
				preview.Children.Clear();
				preview.Children.Add(CreateCard(index, viewModel.ModeTitle, viewModel.ModeDetail));
			}
		};
		mode.Items.Add("Theme");
		mode.Items.Add("Style");
		mode.Items.Add("BasedOn");
		mode.Items.Add("Override");

		AddShowcase(
			"Style precedence",
			"Compare an implicit app theme, an explicit style, inheritance and a later local value.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(preview, 224),
				LabeledControl("Applied source", mode)),
			Code(vm => vm.StyleCode));
	}


	static Border CreateCard(
		int mode,
		string title,
		string detail)
	{
		Border card = mode switch
		{
			0 => new ThemedCard(),
			1 => new() { Style = GalleryStyles.Card },
			2 => new() { Style = GalleryStyles.ElevatedCard },
			_ => new()
			{
				Style = GalleryStyles.ElevatedCard,
				CornerRadius = 6
			}
		};

		card.HorizontalAlignment = HorizontalAlignment.Center;
		card.VerticalAlignment = VerticalAlignment.Center;
		card.Width = 240;
		card.Height = 120;
		card.Child = CardContent(title, detail);

		return card;
	}

	static StackPanel CardContent(
		string title,
		string detail) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Spacing = 4,

			Children =
			{
				new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					Text = title,
					TextStyle = TextStyle.Title3,
					FontWeight = FontWeight.Semibold
				},

				new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					Text = detail,
					TextStyle = TextStyle.Footnote,
					TextColor = Colors.SecondaryLabel
				}
			}
		};
}
