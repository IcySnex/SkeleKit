namespace SkeleKit;

/// <summary>
/// A file attached to a composed mail.
/// </summary>
public sealed class MailAttachment
{
	/// <summary>
	/// The file's bytes.
	/// </summary>
	public required byte[] Data { get; init; }

	/// <summary>
	/// The file name the recipient sees, including its extension.
	/// </summary>
	public required string FileName { get; init; }

	/// <summary>
	/// The file's MIME type, for example <c>application/pdf</c> or <c>image/png</c>.
	/// </summary>
	public required string MimeType { get; init; }
}
