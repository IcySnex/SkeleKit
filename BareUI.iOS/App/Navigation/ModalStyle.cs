namespace BareUI;

/// <summary>
/// The presentation style of a modal page.
/// </summary>
public readonly struct ModalStyle
{
	/// <summary>
	/// How the modal is presented.
	/// </summary>
	public ModalPresentation Presentation { get; }

	/// <summary>
	/// The heights a sheet may rest at. It opens at the first and can be dragged between them. Ignored for other presentations.
	/// </summary>
	public IReadOnlyList<Detent> Detents { get; }

	ModalStyle(
		ModalPresentation presentation,
		IReadOnlyList<Detent> detents)
	{
		Presentation = presentation;
		Detents = detents;
	}


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
	/// A contextual floating bubble modal on large displays.
	/// </summary>
	public static ModalStyle Popover => new(ModalPresentation.Popover, [Detent.Large]);

	/// <summary>
	/// An interactive, swipe-to-dismiss sheet. Pass more than one height to let the user drag between them, opening at the first.
	/// </summary>
	/// <param name="detents">The heights the sheet may rest at, the first being the one it opens at. Defaults to full height.</param>
	/// <returns>The sheet presentation style.</returns>
	public static ModalStyle Sheet(
		params Detent[] detents) =>
		new(ModalPresentation.PageSheet, detents.Length > 0 ? detents : [Detent.Large]);
}
