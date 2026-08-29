using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.TextInput;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.TextInput;

[Page]
internal sealed class SecureFieldView : ShowcaseView<SecureFieldViewModel>
{
	public SecureFieldView(
		SecureFieldViewModel viewModel) : base(viewModel, "Secure Field", Colors.Purple)
	{
		AddEntryShowcase(viewModel);
		AddIntentShowcase(viewModel);
	}


	void AddEntryShowcase(
		SecureFieldViewModel viewModel)
	{
		SecureField field = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Text = Bind(vm => vm.Text)
				.TwoWay((vm, val) => vm.Text = val),
			Placeholder = "Create a password",
			LeadingIcon = ImageSource.Symbol("lock.fill"),
			RevealButton = viewModel.RevealsEntry,
			ContentKind = ContentKind.NewPassword,
			ReturnKey = ReturnKeyType.Done,
			RequiresText = true,
			SubmitCommand = viewModel.SubmitCommand
		};

		Switch reveal = new()
		{
			IsOn = Bind(vm => vm.RevealsEntry)
				.TwoWay((vm, val) => vm.RevealsEntry = val),
			Toggled = value =>
			{
				field.RevealButton = value;
			}
		};

		AddShowcase(
			"Secure entry",
			"Edit a two-way password, inspect masked and revealed states, and submit without exposing the value.",
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

							new ProgressBar
							{
								HorizontalAlignment = HorizontalAlignment.Stretch,
								Progress = Bind(vm => vm.Strength),
								FillColor = Colors.Pink
							},

							Status(Bind(vm => vm.StrengthLabel), FontWeight.Medium),

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

							Status(Bind(vm => vm.SubmitStatus))
						}
					},
					280),
				SettingRow("Reveal button", reveal)),
			Code(vm => vm.EntryCode));
	}

	void AddIntentShowcase(
		SecureFieldViewModel viewModel)
	{
		SecureField field = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Text = "Gallery password",
			ContentKind = viewModel.SelectedIntent.Value,
			RevealButton = viewModel.ShowsReveal,
			TrailingIcon = ImageSource.Symbol("checkmark.circle.fill"),
			ClearButton = ClearButton.WhileEditing
		};

		Picker<ShowcaseOption<ContentKind>> intent = new()
		{
			MinWidth = 160,
			ItemsSource = viewModel.PasswordIntents,
			SelectedItem = Bind(vm => vm.SelectedIntent)
				.TwoWay((vm, val) => vm.SelectedIntent = val!),
			SelectionChanged = option => field.ContentKind = option.Value
		};

		Switch reveal = new()
		{
			IsOn = Bind(vm => vm.ShowsReveal)
				.TwoWay((vm, val) => vm.ShowsReveal = val),
			Toggled = value =>
			{
				field.RevealButton = value;
			}
		};

		Switch trailing = new()
		{
			IsOn = Bind(vm => vm.ShowsTrailingIcon)
				.TwoWay((vm, val) => vm.ShowsTrailingIcon = val),
			Toggled = value =>
			{
				field.TrailingIcon = value
					? ImageSource.Symbol("checkmark.circle.fill")
					: (ImageSource?)null;
			}
		};

		Switch clear = new()
		{
			IsOn = Bind(vm => vm.ShowsClearButton)
				.TwoWay((vm, val) => vm.ShowsClearButton = val),
			Toggled = value =>
			{
				field.ClearButton = value
					? ClearButton.WhileEditing
					: ClearButton.Never;
			}
		};

		AddShowcase(
			"Password intent & trailing slot",
			"Choose the native autofill intent and verify which control owns the shared trailing position.",
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
							Status(Bind(vm => vm.TrailingOwner), FontWeight.Medium)
						}
					},
					190),
				SettingRow("Autofill intent", intent),
				SettingRow("Reveal button", reveal),
				SettingRow("Trailing icon", trailing),
				SettingRow("Clear button", clear)),
			Code(vm => vm.IntentCode));
	}


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
