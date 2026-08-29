using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.TextInput;

internal sealed partial class TextFieldViewModel : ShowcaseViewModel
{
	public TextFieldViewModel()
	{
		SelectedKeyboard = Keyboards[4];
		SelectedReturnKey = ReturnKeys[4];
		SelectedContentKind = ContentKinds[5];
		SelectedClearButton = ClearButtons[1];
		SelectedWeight = FontWeights[3];
	}


	public List<ShowcaseOption<KeyboardType>> Keyboards { get; } =
	[
		new("Default", KeyboardType.Default),
		new("Numeric", KeyboardType.Numeric),
		new("Decimal", KeyboardType.Decimal),
		new("Phone", KeyboardType.Phone),
		new("Email", KeyboardType.Email),
		new("URL", KeyboardType.Url)
	];

	public List<ShowcaseOption<ReturnKeyType>> ReturnKeys { get; } =
	[
		new("Default", ReturnKeyType.Default),
		new("Go", ReturnKeyType.Go),
		new("Next", ReturnKeyType.Next),
		new("Search", ReturnKeyType.Search),
		new("Send", ReturnKeyType.Send),
		new("Done", ReturnKeyType.Done)
	];

	public List<ShowcaseOption<ContentKind>> ContentKinds { get; } =
	[
		new("None", ContentKind.None),
		new("Username", ContentKind.Username),
		new("Password", ContentKind.Password),
		new("New Password", ContentKind.NewPassword),
		new("One-Time Code", ContentKind.OneTimeCode),
		new("Email", ContentKind.Email),
		new("Name", ContentKind.Name),
		new("Phone Number", ContentKind.PhoneNumber),
		new("Street Address", ContentKind.StreetAddress),
		new("URL", ContentKind.Url)
	];

	public List<ShowcaseOption<Capitalization>> Capitalizations { get; } =
	[
		new("Sentences", Capitalization.Sentences),
		new("None", Capitalization.None),
		new("Words", Capitalization.Words),
		new("Characters", Capitalization.Characters)
	];

	public List<ShowcaseOption<KeyboardLook>> KeyboardLooks { get; } =
	[
		new("System", KeyboardLook.Default),
		new("Light", KeyboardLook.Light),
		new("Dark", KeyboardLook.Dark)
	];

	public List<ShowcaseOption<ClearButton>> ClearButtons { get; } =
	[
		new("Never", ClearButton.Never),
		new("While Editing", ClearButton.WhileEditing),
		new("Unless Editing", ClearButton.UnlessEditing),
		new("Always", ClearButton.Always)
	];

	public List<ShowcaseOption<FontWeight>> FontWeights { get; } =
	[
		new("Ultra Light", FontWeight.UltraLight),
		new("Thin", FontWeight.Thin),
		new("Light", FontWeight.Light),
		new("Regular", FontWeight.Regular),
		new("Medium", FontWeight.Medium),
		new("Semibold", FontWeight.Semibold),
		new("Bold", FontWeight.Bold),
		new("Heavy", FontWeight.Heavy),
		new("Black", FontWeight.Black)
	];

	public List<ShowcaseOption<FontDesign>> FontDesigns { get; } =
	[
		new("Default", FontDesign.Default),
		new("Rounded", FontDesign.Rounded),
		new("Serif", FontDesign.Serif),
		new("Mono", FontDesign.Monospaced)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(BindingCode))]
	string? text;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(BindingCode))]
	bool requiresText = true;

	[ObservableProperty]
	string submitStatus = "Press Send to submit.";

	public IReadOnlyList<Span> BindingCode =>
		Code(
			$$"""
			new TextField
			{
				Text = Bind(vm => vm.Text)
					.TwoWay((vm, val) => vm.Text = val),
				Placeholder = "name@example.com",
				LeadingIcon = ImageSource.Symbol("envelope"),
				ClearButton = ClearButton.WhileEditing,
				ReturnKey = ReturnKeyType.Send,
				RequiresText = {{Boolean(RequiresText)}},
				SubmitCommand = viewModel.SubmitCommand
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(KeyboardCode))]
	ShowcaseOption<KeyboardType> selectedKeyboard = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(KeyboardCode))]
	ShowcaseOption<ReturnKeyType> selectedReturnKey = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(KeyboardCode))]
	ShowcaseOption<ContentKind> selectedContentKind = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedCapitalization))]
	[NotifyPropertyChangedFor(nameof(KeyboardCode))]
	int selectedCapitalizationIndex = 1;

	public ShowcaseOption<Capitalization> SelectedCapitalization =>
		Capitalizations[Math.Clamp(SelectedCapitalizationIndex, 0, Capitalizations.Count - 1)];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(KeyboardCode))]
	bool autocorrection;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedKeyboardLook))]
	[NotifyPropertyChangedFor(nameof(KeyboardCode))]
	int selectedKeyboardLookIndex;

	public ShowcaseOption<KeyboardLook> SelectedKeyboardLook =>
		KeyboardLooks[Math.Clamp(SelectedKeyboardLookIndex, 0, KeyboardLooks.Count - 1)];

	public IReadOnlyList<Span> KeyboardCode =>
		Code(
			$$"""
			new TextField
			{
				Placeholder = "Tap to inspect the keyboard",
				Keyboard = KeyboardType.{{SelectedKeyboard.Value}},
				ReturnKey = ReturnKeyType.{{SelectedReturnKey.Value}},
				ContentKind = ContentKind.{{SelectedContentKind.Value}},
				Capitalization = Capitalization.{{SelectedCapitalization.Value}},
				Autocorrection = {{Boolean(Autocorrection)}},
				KeyboardLook = KeyboardLook.{{SelectedKeyboardLook.Value}}
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ChromeCode))]
	bool showsLeadingIcon = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ChromeCode))]
	int trailingModeIndex;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ChromeCode))]
	ShowcaseOption<ClearButton> selectedClearButton = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(FontSizeLabel))]
	[NotifyPropertyChangedFor(nameof(ChromeCode))]
	double fontSize = 20;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ChromeCode))]
	ShowcaseOption<FontWeight> selectedWeight = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedDesign))]
	[NotifyPropertyChangedFor(nameof(ChromeCode))]
	int selectedDesignIndex = 1;

	public ShowcaseOption<FontDesign> SelectedDesign =>
		FontDesigns[Math.Clamp(SelectedDesignIndex, 0, FontDesigns.Count - 1)];

	public string FontSizeLabel => $"{Number(FontSize)} pt";

	public IReadOnlyList<Span> ChromeCode =>
		Code(
			$$"""
			new TextField
			{
				Text = "SkeleKit",
				LeadingIcon = {{(ShowsLeadingIcon ? "ImageSource.Symbol(\"character.cursor.ibeam\")" : "null")}},
				TrailingIcon = {{(TrailingModeIndex is 1 ? "ImageSource.Symbol(\"checkmark.circle.fill\")" : "null")}},
				ClearButton = ClearButton.{{(TrailingModeIndex is 0 ? SelectedClearButton.Value : ClearButton.Never)}},
				FontSize = {{Number(FontSize)}},
				FontWeight = FontWeight.{{SelectedWeight.Value}},
				FontDesign = FontDesign.{{SelectedDesign.Value}}
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(AccessoryCode))]
	int accessoryModeIndex = 1;

	public IReadOnlyList<Span> AccessoryCode =>
		Code(
			AccessoryModeIndex switch
			{
				0 =>
					"""
					new TextField
					{
						KeyboardToolbar = KeyboardToolbar.None
					};
					""",
				1 =>
					"""
					new TextField
					{
						KeyboardToolbar = KeyboardToolbar.Done
					};
					""",
				2 =>
					"""
					new TextField
					{
						KeyboardToolbar = KeyboardToolbar.Navigation
					};
					""",
				_ =>
					"""
					TextField field = new();
					field.KeyboardAccessory = new Grid
					{
						Padding = new(8, 6),
						Columns =
						{
							GridLength.Star,
							GridLength.Auto
						},
						Children =
						{
							new Button
							{
								Text = "Done",
								Icon = ImageSource.Symbol("keyboard.chevron.compact.down"),
								Kind = ButtonStyle.Glass,
								Command = Command.From(field.Unfocus)
							}.Column(1)
						}
					};
					"""
			});


	[RelayCommand]
	void SetExample() =>
		Text = "hello@skelekit.dev";

	[RelayCommand]
	void ClearText() =>
		Text = null;

	[RelayCommand]
	void Submit() =>
		SubmitStatus = "Submitted";


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Boolean(
		bool value) =>
		value ? "true" : "false";

	static string Number(
		double value) =>
		value.ToString("0.##", CultureInfo.InvariantCulture);
}
