using SkeleKit.Gallery.ViewModels.Notes;

namespace SkeleKit.Gallery.Views.Notes;

[Page]
internal sealed class NoteDetailView : ContentView<NoteDetailViewModel>
{
	public NoteDetailView(NoteDetailViewModel viewModel) : base(viewModel)
	{
		Title = viewModel.Title;
		NavigationTitleStyle = TitleStyle.Inline;
		BackButtonStyle = BackButtonStyle.Generic;
		Background = Colors.GroupedBackground;

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Padding = new(20, 20, 20, 32),
				Spacing = 14,

				Children =
				{
					new Border
					{
						Padding = 16,
						Style = GalleryStyles.Card,

						Child = new StackPanel
						{
							Spacing = 8,

							Children =
							{
								new Label
								{
									Text = viewModel.Title,
									TextStyle = TextStyle.Title2,
									FontWeight = FontWeight.Bold
								},

								new Label
								{
									Text = viewModel.Summary,
									TextStyle = TextStyle.Subheadline,
									TextColor = Colors.SecondaryLabel
								},

								new Label
								{
									Text = viewModel.Body,
									TextStyle = TextStyle.Body
								}
							}
						}
					},

					new Button
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Text = "Pop this column",
						Icon = ImageSource.Symbol("chevron.left"),
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Medium,
						Command = viewModel.PopCommand
					}
				}
			}
		};
	}
}
