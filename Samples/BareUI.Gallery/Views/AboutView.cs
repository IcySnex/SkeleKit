using BareUI;

namespace BareUI.Gallery.Views;

/// <summary>
/// A slim page (ADR-013): no ViewModel, no registration, payload by constructor.
/// </summary>
public class AboutView : ContentView
{
	public AboutView(
		string appName,
		string version)
	{
		Title = "About";

		Label status = new() { Style = Styles.Caption, Text = "Not tapped yet" };

		Content = new StackPanel
		{
			Spacing = 12,
			Margin = new Thickness(16),
			Children =
			{
				new Label { Text = appName, TextStyle = TextStyle.Title1, Bold = true },
				new Label { Style = Styles.Caption, Text = $"Version {version}" },

				new Label { Text = "This page has no ViewModel: state is plain fields, updates are direct assignments." },
				status,
				new Button
				{
					Text = "Tap me",
					Command = Command.From(() => status.Text = $"Tapped at {DateTime.Now:HH:mm:ss}")
				},

				new Button
				{
					Text = "Push another",
					Kind = ButtonStyle.Gray,
					Command = Command.From(() => Navigator.PushAsync(new AboutView(appName, version)))
				}
			}
		};
	}
}
