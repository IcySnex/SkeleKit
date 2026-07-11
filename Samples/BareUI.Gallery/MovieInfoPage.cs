using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Reproduces the top section of Velura's MovieInfo screen (backdrop, poster, title, metadata)
/// in pure BareUI — the M1 layout-engine exit criteria. Poster/backdrop are colored boxes since
/// image controls arrive in M2.
/// </summary>
public static class MovieInfoPage
{
	static readonly Color Secondary = Color.FromHex(0x8E8E93);

	public static View Build() =>
		new ScrollView
		{
			Content = new VStack
			{
				Spacing = 16,
				Margin = new Thickness(16),
				Children =
				{
					// Backdrop
					new Border
					{
						Height = 200,
						CornerRadius = 16,
						Background = Color.FromHex(0x2C2C2E)
					},

					// Poster + info
					new Grid
					{
						Columns = { GridLength.Auto, GridLength.Star },
						Rows = { GridLength.Auto },
						ColumnSpacing = 16,
						Children =
						{
							new Border
							{
								Width = 120,
								Height = 180,
								CornerRadius = 12,
								Background = Color.FromHex(0x48484A)
							}.Column(0),

							new VStack
							{
								Spacing = 6,
								Children =
								{
									new Label { Text = "Interstellar", FontSize = 28, Bold = true },
									new Label { Text = "2014 · 2h 49m · PG-13", FontSize = 15, TextColor = Secondary },
									new Label { Text = "Adventure · Drama · Science Fiction", FontSize = 15, TextColor = Secondary },
									new Label
									{
										Text = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
										FontSize = 15
									}
								}
							}.Column(1)
						}
					}
				}
			}
		};
}
