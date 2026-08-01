using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.TextInput;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.TextInput;

[Page]
internal sealed class TextFieldView : ShowcaseView<TextFieldViewModel>
{
	public TextFieldView(
		TextFieldViewModel viewModel) : base(viewModel, "Text Field", Colors.Pink)
	{
		AddBindingShowcase(viewModel);
		AddKeyboardShowcase(viewModel);
		AddChromeShowcase(viewModel);
		AddAccessoryShowcase(viewModel);
	}


	void AddBindingShowcase(
		TextFieldViewModel viewModel)
	{
		TextField field = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Text = Bind(
				model => model.Text,
				static (model, value) => model.Text = value),
			Placeholder = "name@example.com",
			LeadingIcon = ImageSource.Symbol("envelope"),
			ClearButton = ClearButton.WhileEditing,
			Keyboard = KeyboardType.Email,
			ReturnKey = ReturnKeyType.Send,
			ContentKind = ContentKind.Email,
			Capitalization = Capitalization.None,
			Autocorrection = false,
			RequiresText = viewModel.RequiresText,
			SubmitCommand = viewModel.SubmitCommand
		};

		Switch requiresText = new()
		{
			IsOn = viewModel.RequiresText,
			Toggled = value =>
			{
				viewModel.RequiresText = value;
				field.RequiresText = value;
			}
		};

		AddShowcase(
			"Binding & submission",
			"Edit a two-way value and submit through the configured return key.",
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
							field,

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

							Status(Bind(model => model.SubmitStatus))
						}
					},
					220),
				SettingRow("Require text", requiresText)),
			ShowcaseBox.Code(Bind(model => model.BindingCode)));
	}

	void AddKeyboardShowcase(
		TextFieldViewModel viewModel)
	{
		TextField field = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Placeholder = "Tap to inspect the keyboard",
			Keyboard = viewModel.SelectedKeyboard.Value,
			ReturnKey = viewModel.SelectedReturnKey.Value,
			ContentKind = viewModel.SelectedContentKind.Value,
			Capitalization = viewModel.SelectedCapitalization.Value,
			Autocorrection = viewModel.Autocorrection,
			KeyboardLook = viewModel.SelectedKeyboardLook.Value
		};

		Picker<ShowcaseOption<KeyboardType>> keyboard = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.Keyboards,
			SelectedItem = viewModel.SelectedKeyboard,
			SelectionChanged = option =>
			{
				viewModel.SelectedKeyboard = option;
				field.Keyboard = option.Value;
			}
		};

		Picker<ShowcaseOption<ReturnKeyType>> returnKey = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.ReturnKeys,
			SelectedItem = viewModel.SelectedReturnKey,
			SelectionChanged = option =>
			{
				viewModel.SelectedReturnKey = option;
				field.ReturnKey = option.Value;
			}
		};

		Picker<ShowcaseOption<ContentKind>> contentKind = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.ContentKinds,
			SelectedItem = viewModel.SelectedContentKind,
			SelectionChanged = option =>
			{
				viewModel.SelectedContentKind = option;
				field.ContentKind = option.Value;
			}
		};

		SegmentedControl capitalization = new()
		{
			SelectedIndex = 1,
			SelectionChanged = index =>
			{
				viewModel.SelectedCapitalization = viewModel.Capitalizations[index];
				field.Capitalization = viewModel.SelectedCapitalization.Value;
			}
		};
		capitalization.Items.Add("Sentences");
		capitalization.Items.Add("None");
		capitalization.Items.Add("Words");
		capitalization.Items.Add("All");

		Switch autocorrection = new()
		{
			IsOn = viewModel.Autocorrection,
			Toggled = value =>
			{
				viewModel.Autocorrection = value;
				field.Autocorrection = value;
			}
		};

		SegmentedControl look = new()
		{
			SelectionChanged = index =>
			{
				viewModel.SelectedKeyboardLook = viewModel.KeyboardLooks[index];
				field.KeyboardLook = viewModel.SelectedKeyboardLook.Value;
			}
		};
		look.Items.Add("System");
		look.Items.Add("Light");
		look.Items.Add("Dark");

		AddShowcase(
			"Keyboard behavior",
			"Change native input traits while the field is focused and inspect the keyboard, return key and autofill hints.",
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
							field
						}
					},
					170),
				SettingRow("Keyboard", keyboard),
				SettingRow("Return key", returnKey),
				SettingRow("Autofill kind", contentKind),
				LabeledControl("Capitalization", capitalization),
				SettingRow("Autocorrection", autocorrection),
				LabeledControl("Keyboard appearance", look)),
			ShowcaseBox.Code(Bind(model => model.KeyboardCode)));
	}

	void AddChromeShowcase(
		TextFieldViewModel viewModel)
	{
		TextField field = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Text = "SkeleKit",
			LeadingIcon = ImageSource.Symbol("character.cursor.ibeam"),
			ClearButton = viewModel.SelectedClearButton.Value,
			FontSize = Bind(model => model.FontSize),
			FontWeight = viewModel.SelectedWeight.Value,
			FontDesign = viewModel.SelectedDesign.Value
		};

		Switch leading = new()
		{
			IsOn = viewModel.ShowsLeadingIcon,
			Toggled = value =>
			{
				viewModel.ShowsLeadingIcon = value;
				field.LeadingIcon = value
					? ImageSource.Symbol("character.cursor.ibeam")
					: (ImageSource?)null;
			}
		};

		Picker<ShowcaseOption<ClearButton>> clearButton = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.ClearButtons,
			SelectedItem = viewModel.SelectedClearButton,
			SelectionChanged = option =>
			{
				viewModel.SelectedClearButton = option;

				if (viewModel.TrailingModeIndex is 0)
					field.ClearButton = option.Value;
			}
		};

		View clearSetting = SettingRow("Clear button", clearButton);

		SegmentedControl trailing = new()
		{
			SelectionChanged = index =>
			{
				viewModel.TrailingModeIndex = index;
				field.TrailingIcon = index is 1
					? ImageSource.Symbol("checkmark.circle.fill")
					: (ImageSource?)null;
				field.ClearButton = index is 0
					? viewModel.SelectedClearButton.Value
					: ClearButton.Never;
				clearSetting.IsVisible = index is 0;
			}
		};
		trailing.Items.Add("Clear");
		trailing.Items.Add("Icon");
		trailing.Items.Add("None");

		Slider size = new()
		{
			Minimum = 12,
			Maximum = 32,
			Step = 1,
			Value = viewModel.FontSize,
			ValueChanged = value => viewModel.FontSize = value
		};

		Picker<ShowcaseOption<FontWeight>> weight = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.FontWeights,
			SelectedItem = viewModel.SelectedWeight,
			SelectionChanged = option =>
			{
				viewModel.SelectedWeight = option;
				field.FontWeight = option.Value;
			}
		};

		SegmentedControl design = new()
		{
			SelectedIndex = 1,
			SelectionChanged = index =>
			{
				viewModel.SelectedDesign = viewModel.FontDesigns[index];
				field.FontDesign = viewModel.SelectedDesign.Value;
			}
		};
		design.Items.Add("Default");
		design.Items.Add("Rounded");
		design.Items.Add("Serif");
		design.Items.Add("Mono");

		AddShowcase(
			"Chrome & typography",
			"Compose decorative icons, every native clear-button mode, and explicit system typography.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						MaxWidth = 320,

						Children =
						{
							field
						}
					},
					170),
				SettingRow("Leading icon", leading),
				LabeledControl("Trailing slot", trailing),
				clearSetting,
				LabeledSlider("Font size", Bind(model => model.FontSizeLabel), size),
				SettingRow("Weight", weight),
				LabeledControl("Design", design)),
			ShowcaseBox.Code(Bind(model => model.ChromeCode)));
	}

	void AddAccessoryShowcase(
		TextFieldViewModel viewModel)
	{
		TextField[] fields =
		[
			AccessoryField("First field"),
			AccessoryField("Second field"),
			AccessoryField("Third field")
		];

		foreach (TextField field in fields)
			field.KeyboardToolbar = KeyboardToolbar.Done;

		SegmentedControl mode = new()
		{
			SelectedIndex = viewModel.AccessoryModeIndex,
			SelectionChanged = index =>
			{
				viewModel.AccessoryModeIndex = index;

				foreach (TextField field in fields)
					ApplyAccessory(field, index);
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
							fields[0],
							fields[1],
							fields[2]
						}
					},
					240),
				LabeledControl("Accessory", mode)),
			ShowcaseBox.Code(Bind(model => model.AccessoryCode)));
	}


	static TextField AccessoryField(
		string placeholder) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Placeholder = placeholder
		};

	static void ApplyAccessory(
		TextField field,
		int mode)
	{
		field.KeyboardAccessory = mode is 3 ? CustomAccessory(field) : null;
		field.KeyboardToolbar = mode switch
		{
			1 => KeyboardToolbar.Done,
			2 => KeyboardToolbar.Navigation,
			_ => KeyboardToolbar.None
		};
	}

	static View CustomAccessory(
		TextField field) =>
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
					Icon = "keyboard.chevron.compact.down",
					Kind = ButtonStyle.Glass,
					Size = ButtonSize.Small,
					Command = Command.From(field.Unfocus)
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
