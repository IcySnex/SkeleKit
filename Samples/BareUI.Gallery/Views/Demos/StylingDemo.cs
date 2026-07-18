using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates the native type hierarchy, shared styles, BasedOn, and style precedence.
/// </summary>
[Page]
public class StylingDemo : ContentView<StylingDemoViewModel>
{
	public StylingDemo(
		StylingDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Styling";

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 16,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "TextStyle — the native type hierarchy" },
					new Label { TextStyle = TextStyle.LargeTitle, Text = "LargeTitle" },
					new Label { TextStyle = TextStyle.Title2, Text = "Title2" },
					new Label { TextStyle = TextStyle.Headline, Text = "Headline" },
					new Label { TextStyle = TextStyle.Body, Text = "Body" },
					new Label { TextStyle = TextStyle.Caption1, Text = "Caption1" },

					new Label { Style = Styles.Caption, Text = "Weight and design compose on top of a text style" },
					new Label { TextStyle = TextStyle.Body, Text = "Body, rounded, bold", FontDesign = FontDesign.Rounded, Bold = true },

					new Label { Style = Styles.Caption, Text = "Typography — spacing, decoration, auto-shrink" },
					new Label { Text = "S P A C E D  O U T", LetterSpacing = 3, Bold = true },
					new Label { Text = "Two lines with extra\nbreathing room between them", LineSpacing = 8, MaxLines = 0 },
					new Label { Text = "Underlined", Underline = true },
					new Label { Text = "Struck through", Strikethrough = true },
					new Label { Text = "This long line shrinks down instead of truncating away", AutoShrink = 0.5, MaxLines = 1 },

					new Label { Style = Styles.Caption, Text = "Spans — styled runs in one Label (read-only)" },
					new Label
					{
						MaxLines = 0,
						Spans =
						[
							"A label composes ",
							new Span("bold") { Bold = true },
							", ",
							new Span("colored") { TextColor = Colors.Pink },
							", ",
							new Span("serif") { FontDesign = FontDesign.Serif },
							" and ",
							new Span("struck") { Strikethrough = true },
							" runs."
						]
					},

					new Label { Style = Styles.Caption, Text = "TextView — selectable, with tappable links" },
					new TextView
					{
						MaxLines = 0,
						IsSelectable = true,
						Spans =
						[
							"Long-press to select. Read the ",
							new Link("terms")
							{
								Command = Command.From(() => _ = Navigator.AlertAsync("Terms", "You tapped the terms link."))
							},
							", or hold ",
							new Link("this one")
							{
								Command = Command.From(() => _ = Navigator.AlertAsync("Tapped", "Hold instead for the menu.")),
								ContextMenu =
								{
									new MenuAction { Text = "Copy", Icon = "doc.on.doc" },
									new MenuAction { Text = "Share", Icon = "square.and.arrow.up" }
								}
							},
							" for a peek menu."
						]
					},

					new Label { Style = Styles.Caption, Text = "Style — one shared block of setters" },
					new Border
					{
						Style = Styles.Card,
						Child = new Label { Style = Styles.Title, Text = "Styles.Card" }
					},
					new Border
					{
						Style = Styles.ProminentCard,
						Child = new Label { Style = Styles.Title, Text = "Styles.ProminentCard, BasedOn Card" }
					},

					new Label { Style = Styles.Caption, Text = "Brush — a gradient fill, with a material blurring it" },
					new Overlay
					{
						Height = 160,
						CornerRadius = 12,
						ClipsToBounds = true,
						Children =
						{
							new Border { Background = LinearGradient.Vertical(Colors.Indigo, Colors.Pink) },
							new Border
							{
								Background = new Material(MaterialKind.Thin),
								CornerRadius = 10,
								Padding = new Thickness(12, 8),
								Margin = new Thickness(12),
								HorizontalAlignment = HorizontalAlignment.Start,
								VerticalAlignment = VerticalAlignment.End,
								Child = new Label { Style = Styles.Title, Text = "Material.Thin" }
							}
						}
					},

					new Label { Style = Styles.Caption, Text = "A shadow goes outside, the rounding inside" },
					new Border
					{
						Shadow = new(opacity: 0.4, radius: 10, offsetY: 4),
						HorizontalAlignment = HorizontalAlignment.Start,
						Child = new Image
						{
							Source = ImageSource.Symbol("swift"),
							Width = 80,
							Height = 80,
							Background = Palette.Card,
							CornerRadius = 16
						}
					},

					new Label { Style = Styles.Caption, Text = "Whatever the initializer writes after the style wins" },
					new Border
					{
						Style = Styles.Card,
						CornerRadius = 0,
						Child = new Label { Style = Styles.Title, Text = "Styles.Card, square corners" }
					},

					new Label { Style = Styles.Caption, Text = "PointerEffect — hover with an iPad trackpad/mouse" },
					new StackPanel
					{
						Orientation = Orientation.Horizontal,
						Spacing = 12,
						Children =
						{
							PointerTile("No effect", PointerEffect.None),
							PointerTile("Automatic", PointerEffect.Automatic)
						}
					},

					new Label { Style = Styles.Caption, Text = "AnchorPoint — same 45° rotation, overlaid: center vs corner pivot" },
					new Overlay
					{
						Height = 180,
						Children =
						{
							RotatedTile("None", 0, new Point(0, 0), Colors.Gray),
							RotatedTile("Corner", 45, new Point(0, 0), Colors.Pink),
							RotatedTile("Center", 45, new Point(0.5, 0.5), Colors.Indigo)
						}
					}
				}
			}
		};
	}


	static Border PointerTile(
		string text,
		PointerEffect effect) =>
		new()
		{
			PointerEffect = effect,
			Background = Palette.Card,
			CornerRadius = 12,
			Padding = new Thickness(16, 12),
			Child = new Label { Text = text }
		};

	static Border RotatedTile(
		string text,
		double roatation,
		Point anchor,
		Color color) =>
		new()
		{
			Rotation = roatation,
			AnchorPoint = anchor,
			Width = 120,
			Height = 76,
			Opacity = 0.5,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Background = color,
			Child = new Label { Text = text, TextAlignment = TextAlignment.Center }
		};
}
