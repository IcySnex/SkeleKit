using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Platform;

internal sealed partial class AccessibilityViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ValueLabel))]
	[NotifyPropertyChangedFor(nameof(AccessibilityValueText))]
	double value = 50;

	public string ValueLabel =>
		$"{Number(Value)}%";

	public string AccessibilityValueText =>
		$"{Number(Value)} percent";

	public IReadOnlyList<Span> LabelHintCode { get; } =
	[
		new(
			"""
			new Button
			{
				Icon = "info.circle",
				AccessibilityLabel = "More information",
				AccessibilityHint = "Shows additional information."
			};
			""")
	];

	public IReadOnlyList<Span> ValueCode { get; } =
	[
		new(
			"""
			new Slider
			{
				Value = Bind(
					model => model.Value,
					(model, value) => model.Value = value),
				Minimum = 0,
				Maximum = 100,
				AccessibilityLabel = "Value",
				AccessibilityValue = Bind(
					model => model.AccessibilityValueText)
			};
			""")
	];

	public IReadOnlyList<Span> TraitsCode { get; } =
	[
		new(
			"""
			new Label
			{
				Text = "Section heading",
				AccessibilityTraits = AccessibilityTraits.Header
			};

			new Image
			{
				Source = ImageSource.Symbol("photo"),
				AccessibilityLabel = "Sample image",
				AccessibilityTraits = AccessibilityTraits.Image,
				IsAccessibilityElement = true
			};

			new Button
			{
				Text = "Selected option",
				AccessibilityTraits = AccessibilityTraits.Selected
			};
			""")
	];

	public IReadOnlyList<Span> GroupingCode { get; } =
	[
		new(
			"""
			new Border
			{
				AccessibilityLabel = "Sample item",
				AccessibilityValue = "Secondary text",
				AccessibilityIdentifier = "sample-item",
				IsAccessibilityElement = true,
				Child = itemContent
			};
			""")
	];

	public IReadOnlyList<Span> FocusCode { get; } =
	[
		new(
			"""
			TextField field = new()
			{
				Placeholder = "Value",
				AccessibilityLabel = "Value"
			};

			field.Focus();
			field.Unfocus();
			""")
	];


	static string Number(
		double current) =>
		current.ToString("0", CultureInfo.InvariantCulture);
}
