namespace BareUI;

/// <summary>
/// Declares the app's tabs. Each tab hosts its own navigation stack.
/// </summary>
public sealed class TabsBuilder(
	BareApp app)
{
	internal List<TabDefinition> Definitions { get; } = [];

	internal bool UseLargeTitles { get; private set; }

	/// <summary>
	/// Adds a tab rooted at <typeparamref name="TView"/>. Mapping it is implied.
	/// </summary>
	public TabsBuilder Tab<TView>(
		string title,
		string icon)
		where TView : ContentView, new()
	{
		if (app.Register<TView>() is not { } viewModel)
			throw new InvalidOperationException(
				$"'{typeof(TView).Name}' is a tab root, so it needs a ViewModel: derive it from ContentView<TViewModel>.");

		Definitions.Add(new(viewModel, title, icon));

		return this;
	}

	/// <summary>
	/// Uses large navigation-bar titles.
	/// </summary>
	public TabsBuilder LargeTitles()
	{
		UseLargeTitles = true;

		return this;
	}
}

sealed record TabDefinition(
	Type ViewModel,
	string Title,
	string Icon);
