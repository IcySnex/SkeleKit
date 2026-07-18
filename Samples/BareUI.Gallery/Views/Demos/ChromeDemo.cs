using BareUI;
using BareUI.Gallery.Views;
using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates page chrome: bar colors, leave confirmation, bottom toolbar, and search scopes.
/// </summary>
[Page]
public class ChromeDemo : ContentView<ChromeDemoViewModel>
{
	public ChromeDemo(
		ChromeDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Page chrome";
		TitleStyle = TitleStyle.Large;

		// the bottom toolbar and the tab bar share the screen edge: this page trades the tab bar away
		HidesTabBar = true;

		BarAccent = Colors.Orange;
		TitleColor = Colors.Indigo;
		LargeTitleColor = Colors.Indigo;

		// only guarded while the switch is on: a set ConfirmLeave also disables the pop swipe
		if (ViewModel.GuardLeave)
			ConfirmLeave = ViewModel.ConfirmLeaveAsync;

		Button popoverButton = new() { Text = "Popover", Kind = ButtonStyle.Tinted, Command = ViewModel.PresentPopoverCommand };
		popoverButton.CommandParameter = ModalStyle.Popover(popoverButton, PopoverArrow.Up);


		SearchPlaceholder = "Search chrome";
		SearchScopes.Add("All");
		SearchScopes.Add("Recent");
		SearchScopes.Add("Starred");
		SearchChanged = text => ViewModel.SearchStatus = $"Typing: {text}";
		SearchScopeChanged = index => ViewModel.SearchStatus = $"Scope {index} selected";
		SearchCanceled = () => ViewModel.SearchStatus = "Search cancelled";

		BottomToolbarItems.Add(new() { Icon = "square.and.arrow.up", Command = Command.From(() => ViewModel.SearchStatus = "Share tapped") });
		BottomToolbarItems.Add(new() { Icon = "star", Command = Command.From(() => ViewModel.SearchStatus = "Star tapped") });
		BottomToolbarItems.Add(new() { Icon = "trash", Command = Command.From(() => ViewModel.SearchStatus = "Trash tapped") });

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 20,
				Margin = new Thickness(0, 16),
				Children =
				{
					new Label { Margin = new Thickness(16, 0), Style = Styles.Caption, Text = "Bar: orange accent, indigo title (collapse the large title)" },

					new Label { Margin = new Thickness(16, 0), Style = Styles.Caption, Text = "Leave guard: back button and sheet swipe ask first" },
					new StackPanel
					{
						Margin = new Thickness(16, 0),
						Orientation = Orientation.Horizontal,
						Spacing = 12,
						Children =
						{
							new Switch
							{
								IsOn = Bind(vm => vm.GuardLeave, (vm, value) => vm.GuardLeave = value),
								Toggled = value => ConfirmLeave = value ? ViewModel.ConfirmLeaveAsync : null
							},
							new Label
							{
								VerticalAlignment = VerticalAlignment.Center,
								Text = Bind(vm => vm.GuardLeave, guarded => guarded ? "Leaving asks for confirmation" : "Leaving is free")
							}
						}
					},

					new Label { Margin = new Thickness(16, 0), Style = Styles.Caption, Text = "Search: type, switch scopes, cancel" },
					new Label { Margin = new Thickness(16, 0), Text = Bind(vm => vm.SearchStatus) },

					new Label { Margin = new Thickness(16, 0), Style = Styles.Caption, Text = "Modals: this page presented each way (guard blocks the swipe-down)" },
					new ScrollView
					{
						Orientation = Orientation.Horizontal,
						ShowsIndicator = false,
						Content = new StackPanel
						{
							Margin = new Thickness(16, 0),
							Orientation = Orientation.Horizontal,
							Spacing = 8,
							Children =
							{
								new Button { Text = "Sheet", Kind = ButtonStyle.Tinted, Command = ViewModel.PresentCommand, CommandParameter = "large" },
								new Button { Text = "Medium", Kind = ButtonStyle.Tinted, Command = ViewModel.PresentCommand, CommandParameter = "medium" },
								new Button { Text = "Half → full", Kind = ButtonStyle.Tinted, Command = ViewModel.PresentCommand, CommandParameter = "resizable" },
								new Button { Text = "Form", Kind = ButtonStyle.Tinted, Command = ViewModel.PresentCommand, CommandParameter = "form" },
								new Button { Text = "Full", Kind = ButtonStyle.Tinted, Command = ViewModel.PresentCommand, CommandParameter = "full" },
								popoverButton
							}
						},
					},
					new Button { Margin = new Thickness(16, 0), Text = "Dismiss this modal", Kind = ButtonStyle.Gray, Command = ViewModel.DismissCommand },

					new Label { Margin = new Thickness(16, 0), Style = Styles.Caption, Text = "Bottom toolbar: share / star / trash" }
				}
			}
		};
	}
}
