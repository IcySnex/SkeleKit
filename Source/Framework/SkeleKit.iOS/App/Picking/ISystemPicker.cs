namespace SkeleKit;

/// <summary>
/// Presents the system photo and document pickers.
/// </summary>
public interface ISystemPicker
{
	/// <summary>
	/// Presents the photo library and returns the chosen image, or null if canceled.
	/// </summary>
	/// <param name="limit">The maximum number of images that can be selected.</param>
	/// <returns>A task containing the picked image, or null.</returns>
	Task<PickedAsset[]?> PickImagesAsync(
		int limit = 1);

	/// <summary>
	/// Presents the document browser and returns the chosen file, or null if canceled.
	/// </summary>
	/// <param name="extensions">File extensions to allow (for example <c>pdf</c>), or none for any file.</param>
	/// <returns>A task containing the picked file, or null.</returns>
	Task<PickedAsset?> PickFileAsync(
		params string[] extensions);
}
