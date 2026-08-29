using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;

internal sealed partial class PickerViewModel : ShowcaseViewModel
{
	static readonly PickerDestination ExtraDestination =
		new("San Francisco", "United States", "SFO");

	static readonly PickerDestination[] Defaults =
	[
		new("Berlin", "Germany", "BER"),
		new("Copenhagen", "Denmark", "CPH"),
		new("Kyoto", "Japan", "UKY"),
		new("Lisbon", "Portugal", "LIS")
	];


	public PickerViewModel()
	{
		foreach (PickerDestination destination in Defaults)
			Destinations.Add(destination);

		SelectedDestination = Destinations[1];
	}


	public ObservableCollection<PickerDestination> Destinations { get; } = [];

	[ObservableProperty]
	PickerDestination? selectedDestination;

	[ObservableProperty]
	int itemsStateIndex;

	public string ItemsSummary =>
		$"{Destinations.Count} items";

	public IReadOnlyList<Span> PickerCode { get; } =
	[
		new(
			"""
			ObservableCollection<PickerDestination> destinations =
			[
				new("Berlin", "Germany", "BER"),
				new("Copenhagen", "Denmark", "CPH")
			];

			Picker<PickerDestination> picker = new()
			{
				ItemsSource = destinations,
				SelectedItem = Bind(vm => vm.SelectedDestination)
					.TwoWay((vm, val) => vm.SelectedDestination = val),
				Placeholder = "Choose a destination",
				ItemTitle = item => $"{item.City}, {item.Country}"
			};

			destinations.Clear();
			destinations.Add(
				new("San Francisco", "United States", "SFO"));
			""")
	];


	[RelayCommand]
	void ClearSelection() =>
		SelectedDestination = null;

	partial void OnItemsStateIndexChanged(
		int value)
	{
		bool preservesSelection = SelectedDestination is PickerDestination selected
			&& value is not 1
			&& (Defaults.Any(item => ReferenceEquals(item, selected))
				|| value is 2 && ReferenceEquals(ExtraDestination, selected));

		if (!preservesSelection)
			SelectedDestination = null;

		Destinations.Clear();

		if (value is not 1)
			foreach (PickerDestination destination in Defaults)
				Destinations.Add(destination);

		if (value is 2)
			Destinations.Add(ExtraDestination);

		OnPropertyChanged(nameof(ItemsSummary));
	}
}
