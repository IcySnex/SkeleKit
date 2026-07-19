namespace SkeleKit;

/// <summary>
/// A file or an image chosen from the device.
/// </summary>
public sealed class PickedAsset
{
	internal PickedAsset(
		byte[] data,
		string name)
	{
		Data = data;
		Name = name;
	}


	/// <summary>
	/// The data represented as bytes.
	/// </summary>
	public byte[] Data { get; }

	/// <summary>
	/// The file name.
	/// </summary>
	public string Name { get; }

}
