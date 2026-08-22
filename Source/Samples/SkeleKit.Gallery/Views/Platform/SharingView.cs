using System.Windows.Input;
using SkeleKit.Gallery.ViewModels.Platform;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Platform;

[Page]
internal sealed class SharingView : ShowcaseView<SharingViewModel>
{
	public SharingView(
		SharingViewModel viewModel) : base(viewModel, "Sharing", Colors.Mint)
	{
		AddTextShowcase(viewModel);
		AddLinkShowcase(viewModel);
		AddImageShowcase(viewModel);
		AddCombinedShowcase(viewModel);
	}


	void AddTextShowcase(
		SharingViewModel viewModel)
	{
		AddShowcase(
			"Text",
			"Share plain text through the system activity sheet.",
			ShareCanvas(
				"Share text",
				"text.quote",
				viewModel.ShareTextCommand,
				Bind(model => model.TextResult)),
			Code(model => model.TextCode));
	}

	void AddLinkShowcase(
		SharingViewModel viewModel)
	{
		AddShowcase(
			"Link",
			"Share a URL so the system can offer link-specific activities.",
			ShareCanvas(
				"Share link",
				"link",
				viewModel.ShareLinkCommand,
				Bind(model => model.LinkResult)),
			Code(model => model.LinkCode));
	}

	void AddImageShowcase(
		SharingViewModel viewModel)
	{
		AddShowcase(
			"Image",
			"Share an image source with a native preview and image activities.",
			ShareCanvas(
				"Share image",
				"photo",
				viewModel.ShareImageCommand,
				Bind(model => model.ImageResult)),
			Code(model => model.ImageCode));
	}

	void AddCombinedShowcase(
		SharingViewModel viewModel)
	{
		AddShowcase(
			"Combined content",
			"Share text, a URL, and an image as one coherent payload.",
			ShareCanvas(
				"Share combined",
				"square.stack.3d.up",
				viewModel.ShareCombinedCommand,
				Bind(model => model.CombinedResult)),
			Code(model => model.CombinedCode));
	}


	static View ShareCanvas(
		string title,
		string icon,
		ICommand command,
		BindingExpression<string?> result) =>
		ShowcaseBox.Canvas(
			new StackPanel
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Spacing = 10,

				Children =
				{
					new Button
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						Text = title,
						Icon = icon,
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Medium,
						Command = command
					},

					new Label
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						Text = result,
						TextStyle = TextStyle.Footnote,
						TextColor = Colors.SecondaryLabel,
						TextAlignment = TextAlignment.Center
					}
				}
			},
			170);
}
