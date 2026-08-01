using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.TextInput;

internal sealed partial class TextEditorViewModel : ShowcaseViewModel
{
	public TextEditorViewModel()
	{
		SelectedContentKind = ContentKinds[0];
		SelectedCapitalization = Capitalizations[0];
		SelectedKeyboardLook = KeyboardLooks[0];
		SelectedWeight = FontWeights[3];
		SelectedDesign = FontDesigns[0];
	}


	public List<ShowcaseOption<ContentKind>> ContentKinds { get; } =
	[
		new("None", ContentKind.None),
		new("Name", ContentKind.Name),
		new("Email", ContentKind.Email),
		new("Street address", ContentKind.StreetAddress)
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
	[NotifyPropertyChangedFor(nameof(EditSummary))]
	[NotifyPropertyChangedFor(nameof(BindingCode))]
	string? text = "Build native iOS interfaces\nwith clean C# composition.";

	public string EditSummary
	{
		get
		{
			if (string.IsNullOrEmpty(Text))
				return "ViewModel value is empty";

			int lines = Text.Count(character => character is '\n') + 1;
			return $"{Text.Length} characters · {lines} lines";
		}
	}

	public IReadOnlyList<Span> BindingCode =>
		Code(
			"""
			new TextEditor
			{
				Text = Bind(
					model => model.Text,
					(model, value) => model.Text = value)
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(KeyboardCode))]
	ShowcaseOption<ContentKind> selectedContentKind = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(KeyboardCode))]
	ShowcaseOption<Capitalization> selectedCapitalization = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(KeyboardCode))]
	bool autocorrection = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(KeyboardCode))]
	ShowcaseOption<KeyboardLook> selectedKeyboardLook = null!;

	public IReadOnlyList<Span> KeyboardCode =>
		Code(
			$$"""
			new TextEditor
			{
				Text = "Tap to edit this note.",
				ContentKind = ContentKind.{{SelectedContentKind.Value}},
				Capitalization = Capitalization.{{SelectedCapitalization.Value}},
				Autocorrection = {{Boolean(Autocorrection)}},
				KeyboardLook = KeyboardLook.{{SelectedKeyboardLook.Value}}
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(FontSizeLabel))]
	[NotifyPropertyChangedFor(nameof(TypographyCode))]
	double fontSize = 17;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TypographyCode))]
	ShowcaseOption<FontWeight> selectedWeight = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TypographyCode))]
	ShowcaseOption<FontDesign> selectedDesign = null!;

	public string FontSizeLabel => $"{Number(FontSize)} pt";

	public IReadOnlyList<Span> TypographyCode =>
		Code(
			$$"""
			new TextEditor
			{
				Text = "Editable typography\nacross multiple lines.",
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
					new TextEditor
					{
						KeyboardToolbar = KeyboardToolbar.None
					};
					""",
				1 =>
					"""
					new TextEditor
					{
						KeyboardToolbar = KeyboardToolbar.Done
					};
					""",
				2 =>
					"""
					new TextEditor
					{
						KeyboardToolbar = KeyboardToolbar.Navigation
					};
					""",
				_ =>
					"""
					TextEditor editor = new();
					editor.KeyboardAccessory = new Grid
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
								Icon = "keyboard.chevron.compact.down",
								Kind = ButtonStyle.Glass,
								Command = Command.From(editor.Unfocus)
							}.Column(1)
						}
					};
					"""
			});


	[RelayCommand]
	void SetExample() =>
		Text =
			"""
			SkeleKit keeps native controls at the center.

			Compose the interface in C# and keep UIKit behavior.
			""";

	[RelayCommand]
	void ClearText() =>
		Text = null;

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
