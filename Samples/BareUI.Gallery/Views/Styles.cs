namespace BareUI.Gallery.Views;

/// <summary>
/// The app's shared styles: one typed block of setters per view type.
/// </summary>
public static class Styles
{
	/// <summary>
	/// A dimmed footnote heading a demo section.
	/// </summary>
	public static readonly Style<Label> Caption = new(label =>
	{
		label.TextStyle = TextStyle.Footnote;
		label.TextColor = Palette.Secondary;
	});

	/// <summary>
	/// A dimmed line of detail below a title.
	/// </summary>
	public static readonly Style<Label> Detail = new(label =>
	{
		label.TextStyle = TextStyle.Subheadline;
		label.TextColor = Palette.Secondary;
	});

	/// <summary>
	/// The title of a cell.
	/// </summary>
	public static readonly Style<Label> Title = new(label =>
		label.TextStyle = TextStyle.Headline);

	/// <summary>
	/// A <see cref="Caption"/> in bold, heading a section of a list.
	/// </summary>
	public static readonly Style<Label> SectionHeader = new(Caption, label =>
	{
		label.Bold = true;
		label.Margin = new Thickness(16, 8);
	});

	/// <summary>
	/// A rounded card holding a block of content.
	/// </summary>
	public static readonly Style<Border> Card = new(border =>
	{
		border.Background = Palette.Card;
		border.CornerRadius = 12;
		border.Padding = new Thickness(16);
	});

	/// <summary>
	/// A <see cref="Card"/> lifted off the page with a shadow.
	/// </summary>
	public static readonly Style<Border> ProminentCard = new(Card, border =>
		border.Shadow = new(opacity: 0.2, radius: 8, offsetY: 4));
}
