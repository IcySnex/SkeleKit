namespace BareUI;

/// <summary>
/// How a modal page is presented.
/// </summary>
public enum ModalPresentation
{
	/// <summary>A sheet the user can swipe away.</summary>
	Sheet,

	/// <summary>Covers the whole screen.</summary>
	FullScreen,

	/// <summary>A centred card on iPad, a sheet on iPhone.</summary>
	FormSheet
}
