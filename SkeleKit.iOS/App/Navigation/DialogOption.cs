namespace SkeleKit;

/// <summary>
/// A selectable action shown in a dialog.
/// </summary>
public readonly record struct DialogOption
{
	/// <summary>
	/// Converts a string into a standard dialog option.
	/// </summary>
	/// <param name="text">The option's title.</param>
	/// <returns>A standard dialog option with the supplied title.</returns>
	public static implicit operator DialogOption(
		string text) =>
		new(text);


	/// <summary>
	/// Creates a dialog option.
	/// </summary>
	/// <param name="text">The option's title.</param>
	/// <param name="isDestructive">Whether the option is styled as destructive.</param>
	public DialogOption(
		string text,
		bool isDestructive = false)
	{
		Text = text;
		IsDestructive = isDestructive;
	}


	/// <summary>
	/// The option's title.
	/// </summary>
	public string Text { get; }

	/// <summary>
	/// Whether the option is styled as destructive.
	/// </summary>
	public bool IsDestructive { get; }
}
