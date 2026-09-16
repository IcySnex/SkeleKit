namespace SkeleKit;

/// <summary>
/// What happened to a composed mail.
/// </summary>
public enum MailResult
{
	/// <summary>
	/// The mail was sent.
	/// </summary>
	Sent,

	/// <summary>
	/// The mail was saved to Drafts.
	/// </summary>
	Saved,

	/// <summary>
	/// The composer was dismissed without sending or saving.
	/// </summary>
	Cancelled,

	/// <summary>
	/// The system failed to hand the mail over.
	/// </summary>
	Failed
}
