using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.MediaContent;

internal sealed class NativeViewModel : ShowcaseViewModel
{
	public IReadOnlyList<Span> CanvasCode { get; } =
	[
		new(
			"""
			PKCanvasView canvas = new()
			{
				BackgroundColor = UIColor.SystemBackground,
				DrawingPolicy = PKCanvasViewDrawingPolicy.AnyInput,
				Tool = new PKInkingTool(
					PKInkType.Pen,
					UIColor.SystemOrange,
					5)
			};

			new NativeView(canvas)
			{
				Height = 260,
				CornerRadius = 18
			};

			canvas.Drawing = new PKDrawing();
			""")
	];
}
