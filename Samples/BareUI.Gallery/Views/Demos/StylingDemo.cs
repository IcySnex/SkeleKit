using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates the native type hierarchy, shared styles, BasedOn, and style precedence.
/// </summary>
public class StylingDemo : ContentView<StylingDemoViewModel>
{
	public StylingDemo()
	{
		Title = "Styling";

		Content = new ScrollView
		{
			Content = new VStack
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

					new Label { Style = Styles.Caption, Text = "Whatever the initializer writes after the style wins" },
					new Border
					{
						Style = Styles.Card,
						CornerRadius = 0,
						Child = new Label { Style = Styles.Title, Text = "Styles.Card, square corners" }
					}
				}
			}
		};
	}
}
