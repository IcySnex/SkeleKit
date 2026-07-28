using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.Views.Pages;

internal sealed class TopicView : ContentView
{
	public TopicView(
		GalleryTopic topic)
	{
		Title = topic.Title;
		BackgroundStyle = PageBackground.Grouped;
		BackButtonStyle = BackButtonStyle.Generic;

		Content = new StackPanel
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Padding = 24,
			Spacing = 14,

			Children =
			{
				new Image
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					Source = ImageSource.Symbol(topic.Symbol),
					SymbolSize = 54,
					Tint = topic.Accent
				},

				new Label
				{
					Text = topic.Title,
					TextStyle = TextStyle.Title1,
					FontWeight = FontWeight.Bold,
					TextAlignment = TextAlignment.Center
				},

				new Label
				{
					Text = topic.Summary,
					TextStyle = TextStyle.Body,
					TextColor = Colors.SecondaryLabel,
					TextAlignment = TextAlignment.Center,
					MaxLines = 3
				},

				new Label
				{
					Text = "Showcase coming next",
					TextStyle = TextStyle.Footnote,
					TextColor = topic.Accent
				}
			}
		};
	}
}
