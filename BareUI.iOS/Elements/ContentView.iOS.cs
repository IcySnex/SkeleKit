namespace BareUI;

public abstract partial class ContentView
{
	internal PageHost? Host { get; set; }

	partial void ApplyTitleCore() =>
		Host?.SetTitle(Title.Value);
}
