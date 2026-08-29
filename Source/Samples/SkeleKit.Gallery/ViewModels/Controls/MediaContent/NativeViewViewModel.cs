using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.MediaContent;

internal sealed class NativeViewModel : ShowcaseViewModel
{
	public IReadOnlyList<Span> CanvasCode { get; } =
	[
		new(
			"""
			NativeView canvas = new(() => new PKCanvasView
			{
				BackgroundColor = UIColor.SystemBackground,
				DrawingPolicy = PKCanvasViewDrawingPolicy.AnyInput,
				Tool = new PKInkingTool(
					PKInkType.Pen,
					UIColor.SystemOrange,
					5)
			})
			{
				Height = 260,
				CornerRadius = 18
			};

			((PKCanvasView)canvas.Native).Drawing = new PKDrawing();
			""")
	];

	public IReadOnlyList<Span> DirectAccessCode { get; } =
	[
		new(
			"""
			Button button = new() { Text = "Action" };

			bool wasRealized = button.IsRealized;
			UIButton native = (UIButton)button.Native;
			bool isRealized = button.IsRealized;

			// Direct UIKit mutations bypass SkeleKit property replay,
			// binding, styling, layout, and appearance updates.
			native.Layer.BorderWidth = 1;

			// A page controller is available only after the page is hosted.
			UIViewController? controller = Controller;
			""")
	];
}
