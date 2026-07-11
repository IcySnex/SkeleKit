namespace BareUI;

/// <summary>
/// Declares the app's tabs. Each tab hosts its own navigation stack.
/// </summary>
public sealed class TabsBuilder
{
	internal List<TabDefinition> Definitions { get; } = [];

	internal bool UseLargeTitles { get; private set; }

	/// <summary>
	/// Adds a tab rooted at the page mapped to <typeparamref name="TViewModel"/>.
	/// </summary>
	public TabsBuilder Tab<TViewModel>(
		string title,
		string icon)
		where TViewModel : class
	{
		Definitions.Add(new(typeof(TViewModel), title, icon));

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
