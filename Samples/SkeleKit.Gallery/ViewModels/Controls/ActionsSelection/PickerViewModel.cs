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

	static readonly PickerDestination[] LiveDefaults =
	[
		new("Oslo", "Norway", "OSL"),
		new("Paris", "France", "CDG"),
		new("Rome", "Italy", "FCO")
	];


	public PickerViewModel()
	{
		foreach (PickerDestination destination in LiveDefaults)
			LiveDestinations.Add(destination);

		SelectedDestination = Destinations[1];
		LiveSelectedDestination = LiveDestinations[0];
	}


	public List<PickerDestination> Destinations { get; } =
	[
		new("Berlin", "Germany", "BER"),
		new("Copenhagen", "Denmark", "CPH"),
		new("Kyoto", "Japan", "UKY"),
		new("Lisbon", "Portugal", "LIS")
	];

	public ObservableCollection<PickerDestination> LiveDestinations { get; } = [];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedSummary))]
	PickerDestination? selectedDestination;

	[ObservableProperty]
	string selectionStatus = "SelectionChanged has not fired yet.";

	int selectionCount;

	public string SelectedSummary =>
		SelectedDestination is PickerDestination destination
			? $"{destination.Code} · {destination.City}, {destination.Country}"
			: "No destination selected";

	public IReadOnlyList<Span> SelectionCode { get; } =
	[
		new(
			"""
			Picker<PickerDestination> picker = new()
			{
				ItemsSource = Bind(model => model.Destinations),
				SelectedItem = Bind(
					model => model.SelectedDestination,
					(model, value) => model.SelectedDestination = value),
				Placeholder = "Choose a destination",
				ItemTitle = item => $"{item.City}, {item.Country}",
				SelectionChanged = viewModel.RecordSelection
			};
			""")
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(LiveSelectionSummary))]
	PickerDestination? liveSelectedDestination;

	public string LiveSelectionSummary =>
		LiveSelectedDestination is PickerDestination destination
			? $"{destination.Code} selected"
			: "No live item selected";

	public string ItemsSummary =>
		$"{LiveDestinations.Count} items · ObservableCollection";

	public IReadOnlyList<Span> LiveItemsCode { get; } =
	[
		new(
			"""
			ObservableCollection<PickerDestination> destinations =
			[
				new("Oslo", "Norway", "OSL"),
				new("Paris", "France", "CDG")
			];

			Picker<PickerDestination> picker = new()
			{
				ItemsSource = destinations,
				Placeholder = "Select an item",
				ItemTitle = item => $"{item.City}, {item.Country}"
			};

			destinations.Clear();
			destinations.Add(
				new("San Francisco", "United States", "SFO"));
			""")
	];


	[RelayCommand]
	void ClearSelection()
	{
		SelectedDestination = null;
		SelectionStatus = "Selection cleared from the ViewModel.";
	}

	internal void SetLiveItemsState(
		int state)
	{
		LiveSelectedDestination = null;
		LiveDestinations.Clear();

		if (state is not 1)
			foreach (PickerDestination destination in LiveDefaults)
				LiveDestinations.Add(destination);

		if (state is 2)
			LiveDestinations.Add(ExtraDestination);

		OnPropertyChanged(nameof(ItemsSummary));
	}

	internal void RecordSelection(
		PickerDestination destination)
	{
		selectionCount++;
		SelectionStatus = $"SelectionChanged · {destination.Code} · {selectionCount}";
	}
}
