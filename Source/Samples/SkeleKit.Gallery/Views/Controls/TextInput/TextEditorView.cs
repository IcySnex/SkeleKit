using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.TextInput;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.TextInput;

[Page]
internal sealed class TextEditorView : ShowcaseView<TextEditorViewModel>
{
	public TextEditorView(
		TextEditorViewModel viewModel) : base(viewModel, "Text Editor", Colors.Purple)
	{
		AddBindingShowcase(viewModel);
		AddKeyboardShowcase(viewModel);
		AddTypographyShowcase(viewModel);
		AddAccessoryShowcase(viewModel);
	}


	void AddBindingShowcase(
		TextEditorViewModel viewModel)
	{
		TextEditor editor = Editor();
		editor.Text = Bind(vm => vm.Text)
			.TwoWay((vm, val) => vm.Text = val);

		AddShowcase(
			"Binding & live growth",
			"Edit a two-way value and watch the native multi-line editor remeasure as lines are added or removed.",
			PreviewWithSettings(
				ShowcaseBox.FittingCanvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						MaxWidth = 320,
						Spacing = 10,

						Children =
						{
							editor,

							new StackPanel
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Orientation = Orientation.Horizontal,
								Spacing = 8,

								Children =
								{
									new Button
									{
										Text = "Set example",
										Kind = ButtonStyle.Tinted,
										Size = ButtonSize.Small,
										Command = viewModel.SetExampleCommand
									},

									new Button
									{
										Text = "Clear",
										Kind = ButtonStyle.Gray,
										Size = ButtonSize.Small,
										Command = viewModel.ClearTextCommand
									}
								}
							},

							Status(Bind(vm => vm.EditSummary), FontWeight.Medium)
						}
					},
					250)),
			Code(vm => vm.BindingCode));
	}

	void AddKeyboardShowcase(
		TextEditorViewModel viewModel)
	{
		TextEditor editor = Editor("Tap to edit this note.");

		editor.ContentKind = viewModel.SelectedContentKind.Value;
		editor.Capitalization = viewModel.SelectedCapitalization.Value;
		editor.Autocorrection = viewModel.Autocorrection;
		editor.KeyboardLook = viewModel.SelectedKeyboardLook.Value;

		Picker<ShowcaseOption<ContentKind>> contentKind = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.ContentKinds,
			SelectedItem = Bind(vm => vm.SelectedContentKind)
				.TwoWay((vm, val) => vm.SelectedContentKind = val!),
			SelectionChanged = option => editor.ContentKind = option.Value
		};

		SegmentedControl capitalization = new()
		{
			SelectedIndex = Bind(vm => vm.SelectedCapitalizationIndex)
				.TwoWay((vm, val) => vm.SelectedCapitalizationIndex = val),
			SelectionChanged = index =>
			{
				editor.Capitalization = viewModel.SelectedCapitalization.Value;
			}
		};
		capitalization.Items.Add("Sentences");
		capitalization.Items.Add("None");
		capitalization.Items.Add("Words");
		capitalization.Items.Add("All");

		Switch autocorrection = new()
		{
			IsOn = Bind(vm => vm.Autocorrection)
				.TwoWay((vm, val) => vm.Autocorrection = val),
			Toggled = value =>
			{
				editor.Autocorrection = value;
			}
		};

		SegmentedControl look = new()
		{
			SelectedIndex = Bind(vm => vm.SelectedKeyboardLookIndex)
				.TwoWay((vm, val) => vm.SelectedKeyboardLookIndex = val),
			SelectionChanged = index =>
			{
				editor.KeyboardLook = viewModel.SelectedKeyboardLook.Value;
			}
		};
		look.Items.Add("System");
		look.Items.Add("Light");
		look.Items.Add("Dark");

		AddShowcase(
			"Keyboard behavior",
			"Change native text traits while editing and inspect capitalization, correction, autofill intent and keyboard appearance.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						MaxWidth = 320,
						Spacing = 8,

						Children =
						{
							editor
						}
					},
					210),
				SettingRow("Autofill kind", contentKind),
				LabeledControl("Capitalization", capitalization),
				SettingRow("Autocorrection", autocorrection),
				LabeledControl("Keyboard appearance", look)),
			Code(vm => vm.KeyboardCode));
	}

	void AddTypographyShowcase(
		TextEditorViewModel viewModel)
	{
		TextEditor editor = Editor("Editable typography\nacross multiple lines.");
		editor.FontSize = Bind(vm => vm.FontSize);
		editor.FontWeight = viewModel.SelectedWeight.Value;
		editor.FontDesign = viewModel.SelectedDesign.Value;

		Slider size = new()
		{
			Minimum = 12,
			Maximum = 32,
			Step = 1,
			Value = Bind(vm => vm.FontSize)
				.TwoWay((vm, val) => vm.FontSize = val)
		};

		Picker<ShowcaseOption<FontWeight>> weight = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.FontWeights,
			SelectedItem = Bind(vm => vm.SelectedWeight)
				.TwoWay((vm, val) => vm.SelectedWeight = val!),
			SelectionChanged = option => editor.FontWeight = option.Value
		};

		SegmentedControl design = new()
		{
			SelectedIndex = Bind(vm => vm.SelectedDesignIndex)
				.TwoWay((vm, val) => vm.SelectedDesignIndex = val),
			SelectionChanged = index =>
			{
				editor.FontDesign = viewModel.SelectedDesign.Value;
			}
		};
		design.Items.Add("Default");
		design.Items.Add("Rounded");
		design.Items.Add("Serif");
		design.Items.Add("Mono");

		AddShowcase(
			"Typography",
			"Adjust explicit size, every native weight and all four system font designs while the editor remains interactive.",
			PreviewWithSettings(
				ShowcaseBox.FittingCanvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						MaxWidth = 320,

						Children =
						{
							editor
						}
					},
					170),
				LabeledSlider("Font size", Bind(vm => vm.FontSizeLabel), size),
				SettingRow("Weight", weight),
				LabeledControl("Design", design)),
			Code(vm => vm.TypographyCode));
	}

	void AddAccessoryShowcase(
		TextEditorViewModel viewModel)
	{
		TextEditor[] editors =
		[
			Editor("First note"),
			Editor("Second note")
		];

		foreach (TextEditor editor in editors)
			editor.KeyboardToolbar = KeyboardToolbar.Done;

		SegmentedControl mode = new()
		{
			SelectedIndex = Bind(vm => vm.AccessoryModeIndex)
				.TwoWay((vm, val) => vm.AccessoryModeIndex = val),
			SelectionChanged = index =>
			{
				foreach (TextEditor editor in editors)
					ApplyAccessory(editor, index);
			}
		};
		mode.Items.Add("None");
		mode.Items.Add("Done");
		mode.Items.Add("Navigate");
		mode.Items.Add("Custom");

		AddShowcase(
			"Keyboard accessories",
			"Compare no accessory, native Done and navigation toolbars, or a fully custom SkeleKit view.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						MaxWidth = 320,
						Spacing = 10,

						Children =
						{
							editors[0],
							editors[1]
						}
					},
					280),
				LabeledControl("Accessory", mode)),
			Code(vm => vm.AccessoryCode));
	}


	static TextEditor Editor(
		string? text = null) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			MinHeight = 80,
			Text = text,
			Background = Colors.SecondaryBackground,
			CornerRadius = 10,
			ClipsToBounds = true
		};

	static void ApplyAccessory(
		TextEditor editor,
		int mode)
	{
		editor.KeyboardAccessory = mode is 3 ? CustomAccessory(editor) : null;
		editor.KeyboardToolbar = mode switch
		{
			1 => KeyboardToolbar.Done,
			2 => KeyboardToolbar.Navigation,
			_ => KeyboardToolbar.None
		};
	}

	static View CustomAccessory(
		TextEditor editor) =>
		new Grid
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
					Size = ButtonSize.Small,
					Command = Command.From(editor.Unfocus)
				}.Column(1)
			}
		};

	static Label Status(
		BindingExpression<string?> text,
		FontWeight weight = FontWeight.Regular) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = text,
			TextStyle = TextStyle.Footnote,
			FontWeight = weight,
			TextColor = weight is FontWeight.Regular ? Colors.SecondaryLabel : (Color?)null,
			MaxLines = 2,
			TextAlignment = TextAlignment.Center
		};

}
