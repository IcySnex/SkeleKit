namespace SkeleKit.Gallery.Views.Showcase;

internal sealed class ShowcaseBox : Border
{
	static readonly Color CanvasBackground = Color.Dynamic(
		Colors.White,
		Color.FromHex(0x2C2C2E));


	readonly View code;
	readonly ShowcaseContent content;
	readonly View preview;

	int transition;


	public ShowcaseBox(
		View preview,
		View code)
	{
		this.preview = preview;
		this.code = code;

		code.IsEnabled = false;
		code.Opacity = 0;

		content = new(preview, code);

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
			Background = CanvasBackground,

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
		double currentHeight = content.ArrangedBounds.Height > 0
			? content.ArrangedBounds.Height
			: outgoing.DesiredSize.Height;
		double incomingHeight = incoming.DesiredSize.Height;

		outgoing.IsEnabled = false;
		incoming.IsEnabled = true;
		incoming.Opacity = 0;

		content.Height = currentHeight;
		content.Select(incoming);

		View.Animate(
			Animation.Ease(0.2, Easing.EaseInOut),
			() =>
			{
				outgoing.Opacity = 0;
				incoming.Opacity = 1;
				content.Height = incomingHeight;
			},
			_ =>
			{
				if (transition != currentTransition)
					return;

				content.Height = double.NaN;
			});
	}
}
