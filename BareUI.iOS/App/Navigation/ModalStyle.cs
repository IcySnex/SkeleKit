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
	/// The sheet detent, ignored for other presentations.
	/// </summary>
	public Detent Detent { get; }

	ModalStyle(
		ModalPresentation presentation,
		Detent detent)
	{
		Presentation = presentation;
		Detent = detent;
	}


	/// <summary>
	/// A swipe-away sheet, optionally stopping at half height.
	/// </summary>
	public static ModalStyle Sheet(
		Detent detent = Detent.Large) =>
		new(ModalPresentation.Sheet, detent);

	/// <summary>
	/// A full-screen modal.
	/// </summary>
	public static ModalStyle FullScreen =>
		new(ModalPresentation.FullScreen, Detent.Large);

	/// <summary>
	/// A form sheet: a card on iPad, a sheet on iPhone.
	/// </summary>
	public static ModalStyle FormSheet =>
		new(ModalPresentation.FormSheet, Detent.Large);
}
