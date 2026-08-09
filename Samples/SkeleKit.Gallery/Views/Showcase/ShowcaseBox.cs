namespace SkeleKit.Gallery.Views.Showcase;

internal sealed class ShowcaseBox : Border
{
	static readonly Color ContentBackground = Color.Dynamic(
		Color.FromHex(0xf9f9f9),
		Color.FromHex(0x202020));


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
		Canvas(content, height, 0);

	public static View FittingCanvas(
		View content,
		double minHeight = 156) =>
		Canvas(content, double.NaN, minHeight);


	static View Canvas(
		View content,
		double height,
		double minHeight) =>
		new Border
		{
			Height = height,
			MinHeight = minHeight,
			Background = ContentBackground,

			Child = new Overlay
			{
				Children =
				{
					new NativeView(new DotGridView()),

					new Border
					{
						ClipsToBounds = true,
						Padding = 20,
						Child = content
					}
				}
			}
		};

	public static View Code(
		BindingExpression<IReadOnlyList<Span>?> spans) =>
		new Border
		{
			Background = ContentBackground,

			Child = new ScrollView
			{
				Orientation = Orientation.Horizontal,
				Padding = 16,
				Content = CodeText(spans)
			}
		};

	internal static TextView CodeText(
		BindableList<Span> spans) =>
		new()
		{
			Spans = spans,
			IsSelectable = true,
			FontSize = 13,
			FontDesign = FontDesign.Monospaced,
			TextColor = Colors.Label,
			LineSpacing = 2
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
