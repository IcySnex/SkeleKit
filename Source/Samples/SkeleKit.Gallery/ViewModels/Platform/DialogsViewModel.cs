using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Platform;

internal sealed partial class DialogsViewModel(
	INavigator navigator) : ShowcaseViewModel
{
	[ObservableProperty]
	string alertResult = "Not shown";

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ConfirmationCode))]
	bool destructiveConfirmation;

	[ObservableProperty]
	string confirmationResult = "No result";

	[ObservableProperty]
	string promptResult = "No result";

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PromptCode))]
	bool destructivePrompt;

	[ObservableProperty]
	string selectionResult = "No result";

	public IReadOnlyList<Span> AlertCode { get; } =
	[
		new(
			"""
			await navigator.AlertAsync(
				"Alert",
				"This is an alert message.",
				"OK");
			""")
	];

	public IReadOnlyList<Span> ConfirmationCode =>
	[
		new(
			$$"""
			bool accepted = await navigator.ConfirmAsync(
				"Confirmation",
				"Choose whether to confirm or cancel.",
				"Confirm",
				"Cancel",
				destructive: {{Boolean(DestructiveConfirmation)}});
			""")
	];

	public IReadOnlyList<Span> PromptCode =>
	[
		new(
			$$"""
			string? value = await navigator.PromptAsync(
				"Text prompt",
				"Enter a value.",
				placeholder: "Value",
				destructive: {{Boolean(DestructivePrompt)}});
			""")
	];

	public IReadOnlyList<Span> SelectionCode { get; } =
	[
		new(
			"""
			string? option = await navigator.SelectAsync(
				"Select an option",
				"Cancel",
				[
					"First option",
					"Second option",
					"A longer option title that demonstrates shrinking",
					new("Destructive option", true)
				]);
			""")
	];


	[RelayCommand]
	async Task ShowAlertAsync()
	{
		AlertResult = "Shown";

		await navigator.AlertAsync(
			"Alert",
			"This is an alert message.",
			"OK");

		AlertResult = "Dismissed";
	}

	[RelayCommand]
	async Task ShowConfirmationAsync()
	{
		bool accepted = await navigator.ConfirmAsync(
			"Confirmation",
			"Choose whether to confirm or cancel.",
			"Confirm",
			"Cancel",
			DestructiveConfirmation);

		ConfirmationResult = accepted
			? "Confirmed"
			: "Canceled";
	}

	[RelayCommand]
	async Task ShowPromptAsync()
	{
		string? response = await navigator.PromptAsync(
			"Text prompt",
			"Enter a value.",
			placeholder: "Value",
			destructive: DestructivePrompt);

		PromptResult = response switch
		{
			null => "Canceled",
			"" => "Empty value",
			_ => $"Value: {response}"
		};
	}

	[RelayCommand]
	async Task ShowSelectionAsync()
	{
		string? selection = await navigator.SelectAsync(
			"Select an option",
			"Cancel",
			[
				"First option",
				"Second option",
				"A longer option title that demonstrates shrinking",
				new("Destructive option", true)
			]);

		SelectionResult = selection ?? "Canceled";
	}


	static string Boolean(
		bool value) =>
		value ? "true" : "false";
}
