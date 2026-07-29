namespace SkeleKit.Gallery.Views.Showcase;

internal sealed class ShowcaseBox : Border
{
	readonly View code;
	readonly Overlay content;
	readonly View preview;

	int transition;


	public ShowcaseBox(
		View preview,
		View code)
	{
		this.preview = preview;
		this.code = code;

		code.IsEnabled = false;
		code.IsVisible = false;
		code.Opacity = 0;

		content = new()
		{
			Children =
			{
				preview,
				code
			}
		};

		SegmentedControl mode = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Width = 220,
			SelectionChanged = SetMode
		};
		mode.Items.Add("Preview");
		mode.Items.Add("Code");

		Background = Colors.SecondaryGroupedBackground;
		CornerRadius = 16;
		ClipsToBounds = true;

		Child = new StackPanel
		{
			Children =
			{
				new Border
				{
					Padding = 12,
					Child = mode
				},

				new Divider(),

				content
			}
		};
	}


	public static View Canvas(
		View content,
		double height = 156) =>
		new Border
		{
			Height = height,
			Background = Colors.TertiaryGroupedBackground,

			Child = new Overlay
			{
				Children =
				{
					new NativeView(new DotGridView()),

					new Border
					{
						Padding = 20,
						Child = content
					}
				}
			}
		};


	void SetMode(
		int index)
	{
		bool showsCode = index == 1;
		View incoming = showsCode ? code : preview;
		View outgoing = showsCode ? preview : code;
		int currentTransition = ++transition;

		outgoing.IsEnabled = false;
		incoming.IsEnabled = true;
		incoming.IsVisible = true;
		incoming.Opacity = 0;

		content.Height = outgoing.DesiredSize.Height;

		View.Animate(
			Animation.Ease(0.2, Easing.EaseInOut),
			() =>
			{
				outgoing.Opacity = 0;
				incoming.Opacity = 1;
				content.Height = incoming.DesiredSize.Height;
			},
			_ =>
			{
				if (transition != currentTransition)
					return;

				outgoing.IsVisible = false;
				content.Height = double.NaN;
			});
	}
}
