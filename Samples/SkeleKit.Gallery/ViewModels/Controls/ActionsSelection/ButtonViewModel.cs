using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;

internal sealed partial class ButtonViewModel : ShowcaseViewModel
{
	static readonly ButtonSize[] Sizes =
	[
		ButtonSize.Mini,
		ButtonSize.Small,
		ButtonSize.Medium,
		ButtonSize.Large
	];


	public ButtonViewModel()
	{
		SelectedStyle = Styles[3];
		SelectedPlacement = Placements[0];
	}


	public List<ShowcaseOption<ButtonStyle>> Styles { get; } =
	[
		new("Plain", ButtonStyle.Plain),
		new("Gray", ButtonStyle.Gray),
		new("Tinted", ButtonStyle.Tinted),
		new("Filled", ButtonStyle.Filled),
		new("Filled Capsule", ButtonStyle.FilledCapsule),
		new("Glass", ButtonStyle.Glass),
		new("Prominent Glass", ButtonStyle.ProminentGlass),
		new("Clear Glass", ButtonStyle.ClearGlass)
	];

	public List<ShowcaseOption<IconPlacement>> Placements { get; } =
	[
		new("Leading", IconPlacement.Leading),
		new("Trailing", IconPlacement.Trailing),
		new("Top", IconPlacement.Top),
		new("Bottom", IconPlacement.Bottom)
	];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ConfigurationCode))]
	ShowcaseOption<ButtonStyle> selectedStyle = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedSize))]
	[NotifyPropertyChangedFor(nameof(ConfigurationCode))]
	int selectedSizeIndex = 3;

	public ButtonSize SelectedSize => Sizes[Math.Clamp(SelectedSizeIndex, 0, Sizes.Length - 1)];

	public IReadOnlyList<Span> ConfigurationCode =>
		Code(
			$$"""
			new Button
			{
				Text = "Continue",
				Icon = "arrow.right",
				Kind = ButtonStyle.{{SelectedStyle.Value}},
				Size = ButtonSize.{{SelectedSize}}
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ContentCode))]
	ShowcaseOption<IconPlacement> selectedPlacement = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ContentCode))]
	bool showsSubtitle = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IconSizeLabel))]
	[NotifyPropertyChangedFor(nameof(ContentCode))]
	double iconSize = 18;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IconSpacingLabel))]
	[NotifyPropertyChangedFor(nameof(ContentCode))]
	double iconSpacing = 8;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PaddingLabel))]
	[NotifyPropertyChangedFor(nameof(ContentCode))]
	double horizontalPadding = 20;

	public string IconSizeLabel => $"{Number(IconSize)} pt";
	public string IconSpacingLabel => $"{Number(IconSpacing)} pt";
	public string PaddingLabel => $"{Number(HorizontalPadding)} pt";

	public IReadOnlyList<Span> ContentCode =>
		Code(
			$$"""
			new Button
			{
				Text = "Save",
				Icon = "square.and.arrow.down",
				Subtitle = {{NullableString(ShowsSubtitle, "Updated moments ago")}},
				Kind = ButtonStyle.Tinted,
				Size = ButtonSize.Large,
				IconPlacement = IconPlacement.{{SelectedPlacement.Value}},
				IconSize = {{Number(IconSize)}},
				IconSpacing = {{Number(IconSpacing)}},
				Padding = new Thickness({{Number(HorizontalPadding)}}, 12)
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StateCode))]
	bool isLoading;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StateCode))]
	bool isDestructive;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(TapCommand))]
	[NotifyPropertyChangedFor(nameof(StateCode))]
	bool isButtonEnabled = true;

	[ObservableProperty]
	string stateStatus = "Tap the button to run its command.";

	int tapCount;

	public IReadOnlyList<Span> StateCode =>
		Code(
			$$"""
			new Button
			{
				Text = "Run command",
				Icon = "play.fill",
				Kind = ButtonStyle.Filled,
				IsLoading = {{Boolean(IsLoading)}},
				IsDestructive = {{Boolean(IsDestructive)}},
				Command = viewModel.TapCommand,
				CommandParameter = "Button showcase"
			};
			""");


	[ObservableProperty]
	string menuStatus = "Choose an action or a density.";

	public IReadOnlyList<Span> MenuCode =>
		Code(
			"""
			Button actions = new()
			{
				Text = "Actions",
				Icon = "ellipsis.circle"
			};
			actions.Menu.Add(new()
			{
				Text = "Share",
				Icon = "square.and.arrow.up",
				Command = viewModel.SelectMenuCommand,
				CommandParameter = "Share"
			});

			Button density = new()
			{
				Text = "Density",
				SelectsFromMenu = true
			};
			density.Menu.Add(new()
			{
				Text = "Comfortable",
				Command = viewModel.SelectMenuCommand,
				CommandParameter = "Comfortable"
			});
			""");


	[RelayCommand(CanExecute = nameof(CanTap))]
	void Tap(
		string? source)
	{
		tapCount++;
		StateStatus = $"Received “{source}” · {tapCount} tap{(tapCount == 1 ? "" : "s")}";
	}

	bool CanTap(
		string? source) =>
		IsButtonEnabled;

	[RelayCommand]
	void SelectMenu(
		string? value) =>
		MenuStatus = $"{value} selected";


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Boolean(
		bool value) =>
		value ? "true" : "false";

	static string NullableString(
		bool included,
		string value) =>
		included ? $"\"{value}\"" : "null";

	static string Number(
		double value) =>
		value.ToString("0.#", CultureInfo.InvariantCulture);
}
