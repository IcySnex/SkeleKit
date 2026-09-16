namespace SkeleKit;

/// <summary>
/// The system mail composer.
/// </summary>
public interface IMailer
{
	/// <summary>
	/// Whether the device can send mail because a Mail account is configured.
	/// </summary>
	/// <remarks>
	/// Always false in the simulator. Use it to hide or disable a mail action.
	/// </remarks>
	bool CanSendMail { get; }

	/// <summary>
	/// Presents the system mail composer with the given content.
	/// </summary>
	/// <param name="content">The recipients, subject, body and attachments.</param>
	/// <returns>A task containing what the person did with the composer.</returns>
	/// <exception cref="InvalidOperationException">Thrown if no Mail account is configured, or there is no active view controller to present from.</exception>
	Task<MailResult> ComposeAsync(
		MailContent content);
}
