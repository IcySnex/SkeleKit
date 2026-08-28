using CommunityToolkit.Mvvm.Input;
using PencilKit;
using SkeleKit.Gallery.ViewModels.Controls.MediaContent;
using SkeleKit.Gallery.Views.Showcase;
using UIKit;

namespace SkeleKit.Gallery.Views.Controls.MediaContent;

[Page]
internal sealed class NativeView : ShowcaseView<NativeViewModel>
{
	public NativeView(
		NativeViewModel viewModel) : base(viewModel, "Native View", Colors.Orange)
	{
		AddCanvasShowcase();
		AddDirectAccessShowcase();
	}


	void AddCanvasShowcase()
	{
		PKCanvasView canvas = new()
		{
			BackgroundColor = UIColor.SystemBackground,
			DrawingPolicy = PKCanvasViewDrawingPolicy.AnyInput,
			Tool = new PKInkingTool(PKInkType.Pen, UIColor.SystemOrange, 5)
		};

		Button clear = new()
		{
			Text = "Clear",
			Icon = ImageSource.Symbol("trash"),
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Small,
			Command = new RelayCommand(() => canvas.Drawing = new())
		};

		AddShowcase(
			"PencilKit canvas",
			"Host an unsupported native framework view and control it from the surrounding SkeleKit tree.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new SkeleKit.NativeView(canvas)
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						Height = 260,
						CornerRadius = 18
					},
					300),
				SettingRow("Drawing", clear)),
			Code(model => model.CanvasCode));
	}

	void AddDirectAccessShowcase()
	{
		AddCodeShowcase(
			"Direct native access",
			"Inspect a realized UIKit peer or hosted controller only when no SkeleKit API covers the requirement. Accessing Native realizes the view immediately, and direct mutations bypass framework state.",
			Code(model => model.DirectAccessCode));
	}
}
