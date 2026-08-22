namespace SkeleKit.Gallery.Views;

internal static class GalleryStyles
{
	internal static readonly Style<Border> Card = new(card =>
	{
		card.Background = Colors.SecondaryGroupedBackground;
		card.CornerRadius = 18;
		card.Stroke = Colors.Separator;
		card.StrokeThickness = 0.5;
	});

	internal static readonly Style<Border> ElevatedCard = new(Card, card =>
	{
		card.Stroke = Colors.Cyan.WithAlpha(0.5);
		card.StrokeThickness = 1;
		card.Shadow = new(opacity: 0.22, radius: 12, offsetY: 6);
	});

	internal static readonly Style<ThemedCard> ImplicitCard = new(Card, card =>
	{
		card.Background = Colors.Cyan.WithAlpha(0.12);
		card.Stroke = Colors.Cyan;
		card.StrokeThickness = 1;
	});
}

internal sealed class ThemedCard : Border;
