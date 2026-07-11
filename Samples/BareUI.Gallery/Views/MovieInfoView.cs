using BareUI.Gallery.ViewModels;

namespace BareUI.Gallery.Views;

/// <summary>
/// Velura's MovieInfo top section, bound to a ViewModel that loads asynchronously.
/// </summary>
public class MovieInfoView : ContentView<MovieInfoViewModel>
{
	protected override void OnAppearing() =>
		_ = ViewModel!.LoadAsync();

	public MovieInfoView()
	{
		Title = "MovieInfo";

		Content = new ScrollView
		{
			Content = new VStack
			{
				Spacing = 16,
				Margin = new Thickness(16),
				Children =
				{
					new ActivityIndicator
					{
						IsLarge = true,
						IsAnimating = Bind(vm => vm.IsLoading),
						IsVisible = Bind(vm => vm.IsLoading),
						Margin = new Thickness(0, 32)
					},

					new Image
					{
						Source = Bind(vm => vm.Backdrop),
						Height = 200,
						CornerRadius = 16,
						Stretch = Stretch.UniformToFill
					},

					new Grid
					{
						Columns = { GridLength.Auto, GridLength.Star },
						Rows = { GridLength.Auto },
						ColumnSpacing = 16,
						Children =
						{
							new Image
							{
								Source = Bind(vm => vm.Poster),
								Width = 120,
								Height = 180,
								CornerRadius = 12,
								Stretch = Stretch.UniformToFill
							}.Column(0),

							new VStack
							{
								Spacing = 6,
								Children =
								{
									new Label { Text = Bind(vm => vm.Title), FontSize = 28, Bold = true },
									new Label { Text = Bind(vm => vm.Metadata), FontSize = 15, TextColor = Theme.Secondary },
									new Label { Text = Bind(vm => vm.GenreLine), FontSize = 15, TextColor = Theme.Secondary },
									new Label { Text = Bind(vm => vm.Overview), FontSize = 15 }
								}
							}.Column(1)
						}
					}
				}
			}
		};
	}
}
