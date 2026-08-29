using System.Windows.Input;
using SkeleKit.Gallery.ViewModels.Platform;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Platform;

[Page]
internal sealed class NavigationView : ShowcaseView<NavigationViewModel>
{
	public NavigationView(
		NavigationViewModel viewModel) : base(viewModel, "Navigation", Colors.Green)
	{
		AddStackShowcase(viewModel);
		AddPresentationShowcase(viewModel);
		AddTabsShowcase(viewModel);
		AddUrlShowcase(viewModel);
	}


	void AddStackShowcase(
		NavigationViewModel viewModel)
	{
		Button push = ActionButton(
			"Push detail page",
			"arrow.right",
			viewModel.PushDetailCommand);

		AddShowcase(
			"Stack transitions",
			"Push real detail pages, use the native back gesture, and pop one or every page from the stack.",
			ShowcaseBox.Canvas(push, 140),
			Code(vm => vm.StackCode));
	}

	void AddPresentationShowcase(
		NavigationViewModel viewModel)
	{
		Picker<NavigationModalOption> style = new()
		{
			ItemsSource = viewModel.ModalStyles,
			SelectedItem = Bind(vm => vm.SelectedModalStyle)
				.TwoWay((vm, val) => vm.SelectedModalStyle = val!),
			ItemTitle = static option => option.Title
		};

		Picker<NavigationDetentOption> detents = new()
		{
			ItemsSource = viewModel.Detents,
			SelectedItem = Bind(vm => vm.SelectedDetents)
				.TwoWay((vm, val) => vm.SelectedDetents = val!),
			ItemTitle = static option => option.Title
		};

		Button present = ActionButton(
			"Present modal",
			"rectangle.portrait.bottomhalf.filled");
		present.Command = Command.From(() => _ = viewModel.PresentModalAsync(present));

		View detentSetting = SettingRow("Sheet detents", detents);
		style.SelectionChanged = option =>
			detentSetting.IsVisible = option.Kind is NavigationModalKind.Sheet;

		AddShowcase(
			"Modal presentations",
			"Try the presentation styles and sheet detents that shape a real page transition.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(present, 140),
				SettingRow("Presentation", style),
				detentSetting),
			Code(vm => vm.ModalCode));
	}

	void AddTabsShowcase(
		NavigationViewModel viewModel)
	{
		Picker<string> tabs = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			MinWidth = 220,
			ItemsSource = viewModel.Tabs,
			SelectedItem = "Platform",
			Placeholder = "Choose a tab",
			SelectionChanged = tab => viewModel.SelectTabCommand.Execute(tab)
		};

		AddShowcase(
			"Tab selection",
			"Select one of the app's declared tabs through the navigator and keep each tab's own stack intact.",
			ShowcaseBox.Canvas(tabs, 140),
			Code(vm => vm.TabsCode));
	}

	void AddUrlShowcase(
		NavigationViewModel viewModel)
	{
		Picker<NavigationModalOption> style = new()
		{
			ItemsSource = viewModel.UrlModalStyles,
			SelectedItem = Bind(vm => vm.SelectedUrlModalStyle)
				.TwoWay((vm, val) => vm.SelectedUrlModalStyle = val!),
			ItemTitle = static option => option.Title
		};

		Picker<NavigationDetentOption> detents = new()
		{
			ItemsSource = viewModel.UrlDetents,
			SelectedItem = Bind(vm => vm.SelectedUrlDetents)
				.TwoWay((vm, val) => vm.SelectedUrlDetents = val!),
			ItemTitle = static option => option.Title
		};

		Picker<NavigationSafariDismissButtonOption> dismiss = new()
		{
			ItemsSource = viewModel.SafariDismissButtons,
			SelectedItem = Bind(vm => vm.SelectedSafariDismissButton)
				.TwoWay((vm, val) => vm.SelectedSafariDismissButton = val!),
			ItemTitle = static option => option.Title
		};

		Switch reader = new()
		{
			IsOn = Bind(vm => vm.EntersReaderIfAvailable)
				.TwoWay((vm, val) => vm.EntersReaderIfAvailable = val)
		};

		Switch bars = new()
		{
			IsOn = Bind(vm => vm.BarCollapsingEnabled)
				.TwoWay((vm, val) => vm.BarCollapsingEnabled = val)
		};

		Button open = ActionButton(
			"Open repository",
			"safari");
		open.Command = Command.From(() => _ = viewModel.OpenUrlAsync(open));

		View detentSetting = SettingRow("Sheet detents", detents);
		style.SelectionChanged = option =>
			detentSetting.IsVisible = option.Kind is NavigationModalKind.Sheet;

		AddShowcase(
			"In-app browser",
			"Open an http address in Safari with its presentation and browser options. Content detents use Safari's full height.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(open, 140),
				SettingRow("Presentation", style),
				detentSetting,
				SettingRow("Reader mode", reader),
				SettingRow("Collapsing bars", bars),
				SettingRow("Dismiss button", dismiss)),
			Code(vm => vm.UrlCode));
	}


	static Button ActionButton(
		string text,
		ImageSource icon,
		ICommand? command = null) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Text = text,
			Icon = icon,
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Medium,
			Command = command
		};
}
