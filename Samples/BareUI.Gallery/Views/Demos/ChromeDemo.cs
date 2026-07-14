using BareUI;
using BareUI.Gallery.Views;
using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates page chrome: bar colors, leave confirmation, bottom toolbar, and search scopes.
/// </summary>
public class ChromeDemo : ContentView<ChromeDemoViewModel>
{
	public ChromeDemo(
		ChromeDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Page chrome";
		TitleStyle = TitleStyle.Large;

		BarAccent = Colors.Orange;
		TitleColor = Colors.Indigo;
		LargeTitleColor = Colors.Indigo;

		ConfirmLeave = ViewModel.ConfirmLeaveAsync;

		SearchPlaceholder = "Search chrome";
		SearchScopes.Add("All");
		SearchScopes.Add("Recent");
		SearchScopes.Add("Starred");
		SearchChanged = text => ViewModel.SearchStatus = $"Typing: {text}";
		SearchScopeChanged = index => ViewModel.SearchStatus = $"Scope {index} selected";
		SearchCancelled = () => ViewModel.SearchStatus = "Search cancelled";

		BottomToolbarItems.Add(new() { Icon = "square.and.arrow.up", Command = Command.From(() => ViewModel.SearchStatus = "Share tapped") });
		BottomToolbarItems.Add(new() { Icon = "star", Command = Command.From(() => ViewModel.SearchStatus = "Star tapped") });
		BottomToolbarItems.Add(new() { Icon = "trash", Command = Command.From(() => ViewModel.SearchStatus = "Trash tapped") });

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "Bar: orange accent, indigo title (collapse the large title)" },

					new Label { Style = Styles.Caption, Text = "Leave guard: back button and sheet swipe ask first" },
					new StackPanel
					{
						Orientation = Orientation.Horizontal,
						Spacing = 12,
						Children =
						{
							new Switch { IsOn = Bind(vm => vm.GuardLeave, (vm, value) => vm.GuardLeave = value) },
							new Label
							{
								VerticalAlignment = VerticalAlignment.Center,
								Text = Bind(vm => vm.GuardLeave, guarded => guarded ? "Leaving asks for confirmation" : "Leaving is free")
							}
						}
					},

					new Label { Style = Styles.Caption, Text = "Search: type, switch scopes, cancel" },
					new Label { Text = Bind(vm => vm.SearchStatus) },

					new Label { Style = Styles.Caption, Text = "Modals: this page presented each way (guard blocks the swipe-down)" },
					new StackPanel
					{
						Orientation = Orientation.Horizontal,
						Spacing = 8,
						Children =
						{
							new Button { Text = "Sheet", Kind = ButtonStyle.Tinted, Command = ViewModel.PresentCommand, CommandParameter = "large" },
							new Button { Text = "Medium", Kind = ButtonStyle.Tinted, Command = ViewModel.PresentCommand, CommandParameter = "medium" },
							new Button { Text = "Half → full", Kind = ButtonStyle.Tinted, Command = ViewModel.PresentCommand, CommandParameter = "resizable" },
							new Button { Text = "Form", Kind = ButtonStyle.Tinted, Command = ViewModel.PresentCommand, CommandParameter = "form" },
							new Button { Text = "Full", Kind = ButtonStyle.Tinted, Command = ViewModel.PresentCommand, CommandParameter = "full" }
						}
					},
					new Button { Text = "Dismiss this modal", Kind = ButtonStyle.Gray, Command = ViewModel.DismissCommand },

					new Label { Style = Styles.Caption, Text = "Bottom toolbar: share / star / trash" }
				}
			}
		};
	}
}
