using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.StylingMotion;

internal sealed partial class StylesThemesViewModel : ShowcaseViewModel
{
	const string Definitions =
		"""
		static class GalleryStyles
		{
			public static readonly Style<Border> Card = new(card =>
			{
				card.Background = Colors.SecondaryGroupedBackground;
				card.CornerRadius = 18;
				card.Stroke = Colors.Separator;
				card.StrokeThickness = 0.5;
			});

			public static readonly Style<Border> ElevatedCard = new(Card, card =>
			{
				card.Stroke = Colors.Cyan.WithAlpha(0.5);
				card.StrokeThickness = 1;
				card.Shadow = new(opacity: 0.22, radius: 12, offsetY: 6);
			});

			public static readonly Style<ThemedCard> ImplicitCard = new(Card, card =>
			{
				card.Background = Colors.Cyan.WithAlpha(0.12);
				card.Stroke = Colors.Cyan;
				card.StrokeThickness = 1;
			});
		}

		sealed class ThemedCard : Border;

		SkeleApplication.CreateBuilder()
			.UseTheme(theme => theme.Style(GalleryStyles.ImplicitCard));

		static StackPanel CardContent(string title, string detail) =>
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
		""";

	static readonly string[] Titles =
	[
		"Implicit theme",
		"Explicit style",
		"BasedOn style",
		"Local override"
	];

	static readonly string[] Details =
	[
		"Applied during construction",
		"Style = GalleryStyles.Card",
		"Card plus elevation",
		"CornerRadius = 6"
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ModeTitle))]
	[NotifyPropertyChangedFor(nameof(ModeDetail))]
	[NotifyPropertyChangedFor(nameof(StyleCode))]
	int modeIndex;

	int SafeMode =>
		Math.Clamp(ModeIndex, 0, Titles.Length - 1);

	public string ModeTitle =>
		Titles[SafeMode];

	public string ModeDetail =>
		Details[SafeMode];

	public IReadOnlyList<Span> StyleCode =>
		[new($"{Definitions}\n\n{UsageCode()}")];


	string UsageCode() =>
		SafeMode switch
		{
			0 =>
				"""
				ThemedCard preview = new()
				{
					Width = 240,
					Height = 120,
					Child = CardContent("Implicit theme", "Applied during construction")
				};
				""",

			1 =>
				"""
				Border preview = new()
				{
					Style = GalleryStyles.Card,
					Width = 240,
					Height = 120,
					Child = CardContent("Explicit style", "Style = GalleryStyles.Card")
				};
				""",

			2 =>
				"""
				Border preview = new()
				{
					Style = GalleryStyles.ElevatedCard,
					Width = 240,
					Height = 120,
					Child = CardContent("BasedOn style", "Card plus elevation")
				};
				""",

			_ =>
				"""
				Border preview = new()
				{
					Style = GalleryStyles.ElevatedCard,
					CornerRadius = 6,
					Width = 240,
					Height = 120,
					Child = CardContent("Local override", "CornerRadius = 6")
				};
				"""
		};
}
