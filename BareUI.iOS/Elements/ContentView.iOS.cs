namespace BareUI;

public abstract partial class ContentView
{
	internal PageHost? Host { get; set; }

	partial void ApplyTitleCore()
	{
		if (Host is { } host)
			host.Title = Title.Value;
	}
}
