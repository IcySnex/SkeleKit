using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Platform;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Platform;

[Page]
internal sealed class TabsIpadView : ShowcaseView<TabsIpadViewModel>
{
	public TabsIpadView(
		TabsIpadViewModel viewModel) : base(viewModel, "Tabs & iPad", Colors.Green)
	{
		AddAccessoryShowcase(viewModel);
		AddBadgeShowcase(viewModel);
		AddMinimizationShowcase(viewModel);
		AddCodeShowcase(
			"Bottom tabs & search",
			"Declare the app's primary destinations and the system search bubble.",
			Code(vm => vm.TabsCode));
		AddCodeShowcase(
			"iPad sidebar",
			"Set placements, iPad-only destinations, grouped sections and a footer in one configuration.",
			Code(vm => vm.PadCode));
	}

	void AddMinimizationShowcase(
		TabsIpadViewModel viewModel)
	{
		Picker<ShowcaseOption<TabBarMinimize>> behavior = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			MinWidth = 210,
			ItemsSource = viewModel.MinimizeBehaviors,
			SelectedItem = Bind(vm => vm.SelectedMinimizeBehavior)
				.TwoWay((vm, val) => vm.SelectedMinimizeBehavior = val!),
			ItemTitle = static option => option.Title
		};

		AddShowcase(
			"Tab bar minimization",
			"Choose a live scroll direction, then move the page up or down to see the native iOS 26 behavior.",
			ShowcaseBox.Canvas(behavior, 140),
			Code(vm => vm.MinimizeCode));
	}


	void AddAccessoryShowcase(
		TabsIpadViewModel viewModel)
	{
		Button visible = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Text = Bind(vm => vm.ShowsAccessory)
				.ConvertTo(static shown => shown ? "Hide accessory" : "Show accessory"),
			Icon = ImageSource.Symbol("rectangle.bottomthird.inset.filled"),
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Medium,
			Command = viewModel.ToggleAccessoryCommand
		};

		AddShowcase(
			"Tab accessory",
			"Show or hide an app-level view in the native iOS 26 slot above the tab bar.",
			ShowcaseBox.Canvas(visible, 140),
			Code(vm => vm.AccessoryCode));
	}

	void AddBadgeShowcase(
		TabsIpadViewModel viewModel)
	{
		Stepper count = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Value = Bind(vm => vm.BadgeCount)
				.TwoWay((vm, val) => vm.BadgeCount = val),
			Minimum = 0,
			Maximum = 12,
			Step = 1
		};

		AddShowcase(
			"Live badge",
			"Update or clear the badge on the Gallery's Platform tab through a bound page property.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 12,

						Children =
						{
							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(vm => vm.BadgeLabel),
								TextStyle = TextStyle.Title3,
								FontWeight = FontWeight.Semibold,
								TextAlignment = TextAlignment.Center
							},

							count
						}
					},
					160),
				SettingRow(
					"Badge",
					new Button
					{
						Text = "Clear",
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Small,
						Command = viewModel.ClearBadgeCommand
					})),
			Code(vm => vm.BadgeCode));
	}


	protected override void OnAppearing()
	{
		base.OnAppearing();
		ViewModel.Enter();
	}

	protected override void OnDisappearing()
	{
		ViewModel.Leave();
		base.OnDisappearing();
	}
}
