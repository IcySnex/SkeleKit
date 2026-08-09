using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Foundations;

internal sealed partial class PanelsViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ChildCountLabel))]
	double childCount = 3;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PaddingLabel))]
	[NotifyPropertyChangedFor(nameof(PaddingCode))]
	double panelPadding = 16;

	[ObservableProperty]
	string? inheritedText = "Gallery context";


	public string ChildCountLabel
	{
		get
		{
			int count = (int)Math.Round(ChildCount);
			return $"{count} {(count == 1 ? "child" : "children")}";
		}
	}

	public string PaddingLabel =>
		$"{Number(PanelPadding)} pt";

	public IReadOnlyList<Span> ChildrenCode { get; } =
		Code(
			"""
			StackPanel row = new()
			{
				Orientation = Orientation.Horizontal,
				Spacing = 8,
				Children =
				{
					ChildCard(1),
					ChildCard(2),
					ChildCard(3)
				}
			};

			static Border ChildCard(int number) =>
				new()
				{
					Width = 44,
					Height = 56,
					Child = new Label { Text = number.ToString() }
				};

			row.Children.Add(ChildCard(4));
			row.Children.Remove(row.Children[^1]);
			""");

	public IReadOnlyList<Span> PaddingCode =>
		Code(
			$$"""
			Overlay panel = new()
			{
				Width = 240,
				Height = 124,
				Padding = new Thickness({{Number(PanelPadding)}}),
				Background = Colors.Indigo.WithAlpha(0.18),
				CornerRadius = 18,

				Children =
				{
					new Border
					{
						Background = Colors.Indigo,
						CornerRadius = 12,
						Child = new Label { Text = "Content" }
					}
				}
			};
			""");

	public IReadOnlyList<Span> BindingCode { get; } =
		Code(
			"""
			StackPanel panel = new()
			{
				BindingContext = viewModel,
				Children =
				{
					new Label
					{
						Text = Bind(
							(PanelsViewModel model) => model.InheritedText,
							text => $"Child reads: {text}")
					}
				}
			};
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Number(
		double value) =>
		value.ToString("0.##", CultureInfo.InvariantCulture);
}
