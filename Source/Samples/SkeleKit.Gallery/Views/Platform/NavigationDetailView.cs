using SkeleKit.Gallery.ViewModels.Platform;

namespace SkeleKit.Gallery.Views.Platform;

[Page]
internal sealed class NavigationDetailView : ContentView<NavigationDetailViewModel>
{
	public NavigationDetailView(
		NavigationDetailViewModel viewModel) : base(viewModel)
	{
		Title = Bind(model => model.Title);
		TitleStyle = TitleStyle.Inline;
		BackButtonStyle = BackButtonStyle.Generic;
		BackgroundStyle = PageBackground.Grouped;

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
							Spacing = 6,

							Children =
							{
								new Label
								{
									Text = Bind(model => model.Title),
									TextStyle = TextStyle.Title2,
									FontWeight = FontWeight.Bold
								},

								new Label
								{
									Text = Bind(model => model.Summary),
									TextStyle = TextStyle.Subheadline,
									TextColor = Colors.SecondaryLabel,
									MaxLines = 3
								}
							}
						}
					},

					new Button
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Text = "Push next detail",
			Icon = ImageSource.Symbol("arrow.right"),
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Medium,
						Command = viewModel.PushNextCommand
					},

					new Button
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Text = "Pop this page",
			Icon = ImageSource.Symbol("chevron.left"),
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Medium,
						Command = viewModel.PopCommand
					},

					new Button
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Text = "Pop to root",
			Icon = ImageSource.Symbol("arrow.uturn.backward"),
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Medium,
						Command = viewModel.PopToRootCommand
					}
				}
			}
		};
	}
}
