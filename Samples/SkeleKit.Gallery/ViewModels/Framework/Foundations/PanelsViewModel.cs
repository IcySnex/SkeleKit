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
				Spacing = 8
			};
			SetChildCount(row, 3);

			static void SetChildCount(StackPanel panel, int count)
			{
				count = Math.Clamp(count, 1, 5);

				while (panel.Children.Count < count)
					panel.Children.Add(ChildCard(panel.Children.Count + 1));

				while (panel.Children.Count > count)
				{
					View last = panel.Children[panel.Children.Count - 1];
					panel.Children.Remove(last);
				}
			}

			static Border ChildCard(int number) =>
				new()
				{
					Width = 44,
					Height = 56,
					Background = Colors.Indigo,
					CornerRadius = 12,
					Child = new Label { Text = number.ToString() }
				};
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
				Width = 300,

				Children =
				{
					new Border
					{
						Child = new Label
						{
							Text = BindingFactory.Bind(
								(PanelsViewModel model) => model.InheritedText,
								text => $"Child reads: {text}")
						}
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
