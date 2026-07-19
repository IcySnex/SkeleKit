namespace SkeleKit;

/// <summary>
/// The presentation style of a modal page.
/// </summary>
public readonly struct ModalStyle
{
	/// <summary>
	/// Let the system choose the best presentation style dynamically.
	/// </summary>
	public static ModalStyle Automatic => new(ModalPresentation.Automatic, [Detent.Large]);

	/// <summary>
	/// Covers the entire screen and unloads the background.
	/// </summary>
	public static ModalStyle FullScreen => new(ModalPresentation.FullScreen, [Detent.Large]);

	/// <summary>
	/// A centered card layout on iPad/desktop, and a full sheet on iPhone.
	/// </summary>
	public static ModalStyle FormSheet => new(ModalPresentation.FormSheet, [Detent.Large]);

	/// <summary>
	/// Presents inside the parent bounds instead of the full screen.
	/// </summary>
	public static ModalStyle CurrentContext => new(ModalPresentation.CurrentContext, [Detent.Large]);

	/// <summary>
	/// Covers the whole screen but keeps the background loaded.
	/// </summary>
	public static ModalStyle OverFullScreen => new(ModalPresentation.OverFullScreen, [Detent.Large]);

	/// <summary>
	/// Presents inside the parent bounds while keeping the background loaded.
	/// </summary>
	public static ModalStyle OverCurrentContext => new(ModalPresentation.OverCurrentContext, [Detent.Large]);

	/// <summary>
	/// A contextual floating bubble anchored to a view on large displays.
	/// </summary>
	/// <param name="anchor">The view the popover points at.</param>
	/// <param name="arrows">The directions the arrow may point.</param>
	/// <returns>The popover presentation style.</returns>
	public static ModalStyle Popover(
		View anchor,
		PopoverArrow arrows = PopoverArrow.Any) =>
		new(ModalPresentation.Popover, [Detent.Large], anchor, arrows);

	/// <summary>
	/// An interactive, swipe-to-dismiss sheet.
	/// </summary>
	/// <remarks>
	/// Pass more than one height to let the user drag between them, opening at the first.
	/// </remarks>
	/// <param name="detents">The heights the sheet may rest at, the first being the one it opens at. Defaults to full height.</param>
	/// <returns>The sheet presentation style.</returns>
	public static ModalStyle Sheet(
		params Detent[] detents) =>
		new(ModalPresentation.PageSheet, detents.Length > 0 ? detents : [Detent.Large]);


	ModalStyle(
		ModalPresentation presentation,
		IReadOnlyList<Detent> detents,
		View? anchor = null,
		PopoverArrow arrows = PopoverArrow.Any)
	{
		Presentation = presentation;
		Detents = detents;
		Anchor = anchor;
		Arrows = arrows;
	}


	/// <summary>
	/// How the modal is presented.
	/// </summary>
	public ModalPresentation Presentation { get; }

	/// <summary>
	/// The heights a sheet may rest at.
	/// </summary>
	/// <remarks>
	/// It opens at the first and can be dragged between them; ignored for other presentations.
	/// </remarks>
	public IReadOnlyList<Detent> Detents { get; }

	/// <summary>
	/// The view a popover points at, or null.
	/// </summary>
	/// <remarks>
	/// Ignored for other presentations.
	/// </remarks>
	public View? Anchor { get; }

	/// <summary>
	/// The directions a popover's arrow may point.
	/// </summary>
	public PopoverArrow Arrows { get; }
}
