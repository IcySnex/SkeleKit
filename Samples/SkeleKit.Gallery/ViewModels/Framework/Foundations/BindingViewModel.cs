using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Foundations;

internal sealed record BindingSampleItem(
	string Title);

internal sealed partial class BindingViewModel : ShowcaseViewModel
{
	int nextItem = 4;


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(OneWayValueLabel))]
	double oneWayValue = 24;

	[ObservableProperty]
	string? twoWayText = "SkeleKit";

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedItemLabel))]
	BindingSampleItem? selectedItem;

	public ObservableCollection<BindingSampleItem> Items { get; } =
	[
		new("Item 1"),
		new("Item 2"),
		new("Item 3")
	];


	public string OneWayValueLabel =>
		$"{OneWayValue:0}";

	public string ItemCountLabel =>
		$"{Items.Count} {(Items.Count == 1 ? "item" : "items")}";

	public bool CanAddItem =>
		Items.Count < 5;

	public bool CanRemoveItem =>
		Items.Count > 1;

	public string SelectedItemLabel =>
		SelectedItem?.Title ?? "No selection";

	public IReadOnlyList<Span> OneWayCode { get; } =
		Code(
			"""
			Label target = new()
			{
				Text = Bind(
					model => model.OneWayValue,
					value => $"{value:0}")
			};

			Slider source = new()
			{
				Minimum = 12,
				Maximum = 48,
				Value = viewModel.OneWayValue,
				ValueChanged = value => viewModel.OneWayValue = value
			};
			""");

	public IReadOnlyList<Span> TwoWayCode { get; } =
		Code(
			"""
			TextField field = new()
			{
				Text = Bind(
					model => model.TwoWayText,
					static (model, value) => model.TwoWayText = value)
			};

			Label sourceValue = new()
			{
				Text = Bind(model => model.TwoWayText)
			};

			Button updateSource = new()
			{
				Text = "Set example",
				Command = Command.From(() =>
					viewModel.TwoWayText = "Updated by source")
			};
			""");

	public IReadOnlyList<Span> ListCode { get; } =
		Code(
			"""
			ObservableCollection<BindingSampleItem> Items { get; } =
			[
				new("Item 1"),
				new("Item 2"),
				new("Item 3")
			];

			Picker<BindingSampleItem> picker = new()
			{
				ItemsSource = Bind(model => model.Items),
				SelectedItem = Bind(
					model => model.SelectedItem,
					static (model, item) => model.SelectedItem = item),
				ItemTitle = item => item.Title
			};

			Items.Add(new("Item 4"));
			Items.RemoveAt(Items.Count - 1);
			""");

	public BindingViewModel()
	{
		selectedItem = Items[0];
	}


	internal void SetTwoWayExample() =>
		TwoWayText = "Updated by source";

	internal void AddItem()
	{
		if (!CanAddItem)
			return;

		Items.Add(new($"Item {nextItem++}"));
		NotifyItemState();
	}

	internal void RemoveItem()
	{
		if (!CanRemoveItem)
			return;

		BindingSampleItem removed = Items[^1];
		Items.RemoveAt(Items.Count - 1);

		if (ReferenceEquals(SelectedItem, removed))
			SelectedItem = Items[0];

		NotifyItemState();
	}


	void NotifyItemState()
	{
		OnPropertyChanged(nameof(ItemCountLabel));
		OnPropertyChanged(nameof(CanAddItem));
		OnPropertyChanged(nameof(CanRemoveItem));
	}

	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
