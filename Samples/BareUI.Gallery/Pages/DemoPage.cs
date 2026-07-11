namespace BareUI.Gallery;

/// <summary>
/// Wraps one control demo: the title and the tree to show.
/// </summary>
public class DemoViewModel(
	string title,
	Func<View> content)
{
	public string Title { get; } = title;

	public Func<View> Content { get; } = content;
}

/// <summary>
/// Renders whichever demo the menu pushed.
/// </summary>
public class DemoPage : ContentView<DemoViewModel>
{
	protected override View Build()
	{
		Title = ViewModel!.Title;

		return ViewModel.Content();
	}
}
