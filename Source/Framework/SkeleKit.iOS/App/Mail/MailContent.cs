namespace SkeleKit;

/// <summary>
/// What to hand the mail composer: recipients, a subject, a body and optional attachments.
/// </summary>
public sealed class MailContent
{
	/// <summary>
	/// Primary recipients.
	/// </summary>
	public IList<string> To { get; } = [];

	/// <summary>
	/// Carbon-copy recipients.
	/// </summary>
	public IList<string> Cc { get; } = [];

	/// <summary>
	/// Blind carbon-copy recipients.
	/// </summary>
	public IList<string> Bcc { get; } = [];

	/// <summary>
	/// The subject line, or null for an empty one.
	/// </summary>
	public string? Subject { get; set; }

	/// <summary>
	/// The message body, or null for an empty one.
	/// </summary>
	public string? Body { get; set; }

	/// <summary>
	/// Whether <see cref="Body"/> is HTML instead of plain text.
	/// </summary>
	public bool IsHtml { get; set; }

	/// <summary>
	/// Files to attach.
	/// </summary>
	public IList<MailAttachment> Attachments { get; } = [];
}
