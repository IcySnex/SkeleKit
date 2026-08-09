using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Foundations;

internal sealed partial class ViewViewModel(
	INavigator navigator) : ShowcaseViewModel
{
	static readonly HorizontalAlignment[] LayoutAlignments =
	[
		HorizontalAlignment.Start,
		HorizontalAlignment.Center,
		HorizontalAlignment.End
	];

	static readonly Point[] Anchors =
	[
		new(0.5, 0.5),
		new(0, 0)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(LayoutWidthLabel))]
	[NotifyPropertyChangedFor(nameof(LayoutCode))]
	double layoutWidth = 180;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(LayoutAlignment))]
	[NotifyPropertyChangedFor(nameof(LayoutCode))]
	int layoutAlignmentIndex = 1;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(LeadingMarginLabel))]
	[NotifyPropertyChangedFor(nameof(LayoutCode))]
	double leadingMargin = 12;

	[ObservableProperty]
	bool layoutVisible = true;

	internal HorizontalAlignment LayoutAlignment =>
		LayoutAlignments[Math.Clamp(LayoutAlignmentIndex, 0, LayoutAlignments.Length - 1)];

	public string LayoutWidthLabel =>
		$"{Number(LayoutWidth)} pt";

	public string LeadingMarginLabel =>
		$"{Number(LeadingMargin)} pt";

	public IReadOnlyList<Span> LayoutCode =>
		Code(
			$$"""
			Border card = new()
			{
				Width = {{Number(LayoutWidth)}},
				Height = 96,
				MinWidth = 100,
				MaxWidth = 240,
				Margin = new Thickness({{Number(LeadingMargin)}}, 0, 0, 0),
				HorizontalAlignment = HorizontalAlignment.{{LayoutAlignment}},
				VerticalAlignment = VerticalAlignment.Center,
				IsVisible = Bind(model => model.LayoutVisible),
				Background = Colors.Indigo,
				CornerRadius = 18
			};

			View.LayoutNow();
			Size desired = card.DesiredSize;
			Rect arranged = card.ArrangedBounds;
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(RotationLabel))]
	[NotifyPropertyChangedFor(nameof(VisualCode))]
	double rotation = 12;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ScaleLabel))]
	[NotifyPropertyChangedFor(nameof(VisualCode))]
	double scale = 1;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(OpacityLabel))]
	[NotifyPropertyChangedFor(nameof(VisualCode))]
	double opacity = 1;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(Anchor))]
	[NotifyPropertyChangedFor(nameof(VisualCode))]
	int anchorIndex;

	internal Point Anchor =>
		Anchors[Math.Clamp(AnchorIndex, 0, Anchors.Length - 1)];

	public string RotationLabel =>
		$"{Number(Rotation)}°";

	public string ScaleLabel =>
		$"{Number(Scale)}×";

	public string OpacityLabel =>
		$"{Opacity:P0}";

	public IReadOnlyList<Span> VisualCode =>
		Code(
			$$"""
			new Border
			{
				Width = 180,
				Height = 96,
				Background = Colors.Indigo,
				Opacity = {{Number(Opacity)}},
				CornerRadius = 18,
				Rotation = {{Number(Rotation)}},
				Scale = {{Number(Scale)}},
				AnchorPoint = new({{Number(Anchor.X)}}, {{Number(Anchor.Y)}})
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedBrush))]
	[NotifyPropertyChangedFor(nameof(BrushCode))]
	int brushIndex = 1;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(BrushCode))]
	bool castsShadow = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(BrushCode))]
	bool clipsContent;

	internal Brush SelectedBrush =>
		BrushIndex switch
		{
			0 => Colors.Indigo,
			1 => LinearGradient.Vertical(
				Colors.Indigo,
				Colors.Indigo.WithAlpha(0.7)),
			_ => new Material(MaterialKind.Regular)
		};

	public IReadOnlyList<Span> BrushCode
	{
		get
		{
			string brush = BrushIndex switch
			{
				0 => "Colors.Indigo",
				1 => "LinearGradient.Vertical(Colors.Indigo, Colors.Indigo.WithAlpha(0.7))",
				_ => "new Material(MaterialKind.Regular)"
			};

			return Code(
				$$"""
				Border surface = new()
				{
					Background = {{brush}},
					CornerRadius = 18,
					Shadow = {{(CastsShadow ? "new(opacity: 0.22, radius: 10, offsetY: 5)" : "null")}}
				};

				Grid preview = new()
				{
					Width = 180,
					Height = 96,
					ClipsToBounds = {{Bool(ClipsContent)}},
					Children =
					{
						surface,
						new Border
						{
							Width = 50,
							Height = 26,
							Translation = new(14, -10)
						}
					}
				};
				""");
		}
	}


	public IReadOnlyList<Span> InteractionCode { get; } =
		Code(
			"""
			Label status = new()
			{
				Text = "Tap, double-tap, hold, drag, pinch or rotate."
			};
			void Record(string interaction) =>
				status.Text = interaction;

			Border commandCard = new()
			{
				PointerEffect = PointerEffect.Automatic,
				TapCommand = Command.From(() => Record("Tap")),
				DoubleTapCommand = Command.From(() => Record("Double tap")),
				LongPressCommand = Command.From(() => Record("Long press")),
				LongPressDuration = 0.7
			};

			Border gestureCard = new()
			{
				PointerEffect = PointerEffect.Automatic
			};
			gestureCard.ContextMenu.Add(new()
			{
				Text = "Copy",
				Command = Command.From(() => Record("Copy"))
			});
			gestureCard.ContextMenu.Add(new()
			{
				Text = "Share",
				Command = Command.From(() => Record("Share"))
			});

			gestureCard.Panned = gesture =>
			{
				if (gesture.State is GestureState.Changed)
				{
					gestureCard.Translation = new(
						Math.Clamp(gesture.Translation.X, -70, 70),
						Math.Clamp(gesture.Translation.Y, -36, 36));
				}
				else if (gesture.State is GestureState.Ended or GestureState.Canceled)
				{
					Record("Pan");
					ReturnHome(gestureCard);
				}
			};
			gestureCard.Pinched = gesture =>
			{
				if (gesture.State is GestureState.Changed)
					gestureCard.Scale = Math.Clamp(gesture.Scale, 0.7, 1.45);
				else if (gesture.State is GestureState.Ended or GestureState.Canceled)
				{
					Record("Pinch");
					ReturnHome(gestureCard);
				}
			};
			gestureCard.Rotated = gesture =>
			{
				if (gesture.State is GestureState.Changed)
					gestureCard.Rotation = Math.Clamp(gesture.Degrees, -35, 35);
				else if (gesture.State is GestureState.Ended or GestureState.Canceled)
				{
					Record("Rotation");
					ReturnHome(gestureCard);
				}
			};

			static void ReturnHome(View card) =>
				View.Animate(
					Animation.Spring(0.42, damping: 0.72),
					() =>
					{
						card.Translation = Point.Zero;
						card.Scale = 1;
						card.Rotation = 0;
						card.Opacity = 1;
					});
			""");


	internal Task InspectLayoutAsync(
		View view)
	{
		View.LayoutNow();

		return navigator.AlertAsync(
			"Layout result",
			$"Desired with margin: {Size(view.DesiredSize)}\nArranged: {Size(view.ArrangedBounds.Size)}\nOrigin: {Point(view.ArrangedBounds.Location)}");
	}

	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Number(
		double value) =>
		value.ToString("0.##", CultureInfo.InvariantCulture);

	static string Bool(
		bool value) =>
		value ? "true" : "false";

	static string Point(
		Point value) =>
		$"({Number(value.X)}, {Number(value.Y)})";

	static string Size(
		Size value) =>
		$"{Number(value.Width)}×{Number(value.Height)}";

}
