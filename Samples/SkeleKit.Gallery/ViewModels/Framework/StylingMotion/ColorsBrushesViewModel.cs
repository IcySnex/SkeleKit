using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.StylingMotion;

internal sealed class ColorsBrushesViewModel : ShowcaseViewModel
{
	public IReadOnlyList<Span> SemanticCode { get; } =
		Code(
			"""
			Color primaryText = Colors.Label;
			Color secondaryText = Colors.SecondaryLabel;
			Color page = Colors.Background;
			Color raisedSurface = Colors.SecondaryBackground;
			Color separator = Colors.Separator;

			Color custom = Color.Dynamic(
				Color.FromHex(0xDCECEF),
				Color.FromHex(0x24464D));
			""");

	public IReadOnlyList<Span> GradientCode { get; } =
		Code(
			"""
			static readonly LinearGradient TealGradient = new()
			{
				Stops =
				[
					new(Color.FromHex(0x1D5D67), 0),
					new(Color.FromHex(0x497889), 0.55),
					new(Color.FromHex(0x7C8B91), 1)
				],
				Start = new(0, 0),
				End = new(1, 1)
			};

			static readonly LinearGradient SlateGradient = new()
			{
				Stops =
				[
					new(Color.FromHex(0x43566B), 0),
					new(Color.FromHex(0x687989), 0.55),
					new(Color.FromHex(0x8A8580), 1)
				],
				Start = new(0, 0),
				End = new(1, 1)
			};

			bool alternate;

			Border surface = new()
			{
				Background = TealGradient,
				CornerRadius = 20
			};

			void Transition()
			{
				alternate = !alternate;

				View.Animate(
					Animation.Ease(0.8),
					() => surface.Background = alternate
						? SlateGradient
						: TealGradient);
			}
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
