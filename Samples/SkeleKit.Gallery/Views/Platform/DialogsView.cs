using System.Windows.Input;
using SkeleKit.Gallery.ViewModels.Platform;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Platform;

[Page]
internal sealed class DialogsView : ShowcaseView<DialogsViewModel>
{
	public DialogsView(
		DialogsViewModel viewModel) : base(viewModel, "Dialogs", Colors.Mint)
	{
		AddAlertShowcase(viewModel);
		AddConfirmationShowcase(viewModel);
		AddPromptShowcase(viewModel);
		AddSelectionShowcase(viewModel);
	}


	void AddAlertShowcase(
		DialogsViewModel viewModel)
	{
		AddShowcase(
			"Alert",
			"Show a message with one dismiss action and await its completion.",
			DialogCanvas(
				"Show alert",
				"exclamationmark.bubble",
				viewModel.ShowAlertCommand,
				Bind(model => model.AlertResult)),
			Code(model => model.AlertCode));
	}

	void AddConfirmationShowcase(
		DialogsViewModel viewModel)
	{
		Button show = DialogButton(
			"Show confirmation",
			"questionmark.circle",
			viewModel.ShowConfirmationCommand);
		show.IsDestructive = viewModel.DestructiveConfirmation;

		Switch destructive = new()
		{
			IsOn = Bind(
				model => model.DestructiveConfirmation,
				static (model, value) => model.DestructiveConfirmation = value),
			Toggled = value => show.IsDestructive = value
		};

		AddShowcase(
			"Confirmation",
			"Compare standard and destructive confirmation actions and observe the awaited Boolean result.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					DialogContent(
						show,
						Bind(model => model.ConfirmationResult)),
					170),
				SettingRow("Destructive action", destructive)),
			Code(model => model.ConfirmationCode));
	}

	void AddPromptShowcase(
		DialogsViewModel viewModel)
	{
		Button show = DialogButton(
			"Show text prompt",
			"character.cursor.ibeam",
			viewModel.ShowPromptCommand);
		show.IsDestructive = viewModel.DestructivePrompt;

		Switch destructive = new()
		{
			IsOn = Bind(
				model => model.DestructivePrompt,
				static (model, value) => model.DestructivePrompt = value),
			Toggled = value => show.IsDestructive = value
		};

		AddShowcase(
			"Text prompt",
			"Enter a value, submit an empty field, or compare standard and destructive accept actions.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					DialogContent(
						show,
						Bind(model => model.PromptResult)),
					170),
				SettingRow("Destructive action", destructive)),
			Code(model => model.PromptCode));
	}

	void AddSelectionShowcase(
		DialogsViewModel viewModel)
	{
		AddShowcase(
			"Action selection",
			"Choose a standard, long, or destructive option, or cancel, and observe the awaited result.",
			DialogCanvas(
				"Show selection",
				"list.bullet",
				viewModel.ShowSelectionCommand,
				Bind(model => model.SelectionResult)),
			Code(model => model.SelectionCode));
	}


	static View DialogCanvas(
		string title,
		string icon,
		ICommand command,
		BindingExpression<string?> result) =>
		ShowcaseBox.Canvas(
			DialogContent(
				DialogButton(title, icon, command),
				result),
			170);

	static StackPanel DialogContent(
		Button button,
		BindingExpression<string?> result) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Spacing = 10,

			Children =
			{
				button,

				new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					Text = result,
					TextStyle = TextStyle.Footnote,
					TextColor = Colors.SecondaryLabel,
					TextAlignment = TextAlignment.Center,
					MaxLines = 2
				}
			}
		};

	static Button DialogButton(
		string title,
		string icon,
		ICommand command) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = title,
			Icon = icon,
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Medium,
			Command = command
		};
}
