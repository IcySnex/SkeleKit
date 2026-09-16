using MessageUI;
using Microsoft.Extensions.Logging;
using ObjCRuntime;

namespace SkeleKit;

internal sealed class Mailer(
	ILogger<Mailer> logger) : IMailer
{
	sealed class ComposerDelegate : MFMailComposeViewControllerDelegate
	{
		readonly TaskCompletionSource<MailResult> completion = null!;
		readonly ILogger<Mailer> logger = null!;

		public ComposerDelegate(
			TaskCompletionSource<MailResult> completion,
			ILogger<Mailer> logger)
		{
			this.completion = completion;
			this.logger = logger;
		}

		// ReSharper disable once UnusedMember.Local
		public ComposerDelegate(
			NativeHandle handle) : base(handle)
		{ }


		public override void Finished(
			MFMailComposeViewController controller,
			MFMailComposeResult result,
			NSError? error)
		{
			if (error is not null)
				logger.LogError("The mail composer failed: {Error}", error.LocalizedDescription);

			controller.DismissViewController(true, () => completion.TrySetResult(Map(result)));
		}
	}


	static UIViewController? Top()
	{
		UIViewController? controller = UIApplication.SharedApplication
			.ConnectedScenes
			.OfType<UIWindowScene>()
			.SelectMany(scene => scene.Windows)
			.FirstOrDefault(window => window.IsKeyWindow)?
			.RootViewController;

		while (controller?.PresentedViewController is UIViewController presented)
			controller = presented;

		return controller;
	}

	static MailResult Map(
		MFMailComposeResult result) =>
		result switch
		{
			MFMailComposeResult.Sent => MailResult.Sent,
			MFMailComposeResult.Saved => MailResult.Saved,
			MFMailComposeResult.Cancelled => MailResult.Cancelled,
			_ => MailResult.Failed
		};

	static NSData Data(
		MailAttachment attachment) =>
		NSData.FromArray(attachment.Data)
		?? throw new InvalidOperationException($"The attachment '{attachment.FileName}' could not be read.");


	public bool CanSendMail => MFMailComposeViewController.CanSendMail;


	public Task<MailResult> ComposeAsync(
		MailContent content)
	{
		if (!CanSendMail)
			throw new InvalidOperationException("There is no Mail account configured to send from.");

		UIViewController top = Top() ?? throw new InvalidOperationException("There is no active view controller to present the mail composer from.");

		TaskCompletionSource<MailResult> completion = new();
		ComposerDelegate composerDelegate = new(completion, logger);

		MFMailComposeViewController composer = new()
		{
			MailComposeDelegate = composerDelegate
		};

		if (content.To.Count > 0)
			composer.SetToRecipients([.. content.To]);
		if (content.Cc.Count > 0)
			composer.SetCcRecipients([.. content.Cc]);
		if (content.Bcc.Count > 0)
			composer.SetBccRecipients([.. content.Bcc]);

		composer.SetSubject(content.Subject ?? "");
		composer.SetMessageBody(content.Body ?? "", content.IsHtml);

		foreach (MailAttachment attachment in content.Attachments)
		{
			NSData data = Data(attachment);
			composer.AddAttachmentData(data, attachment.MimeType, attachment.FileName);
		}

		top.PresentViewController(composer, true, null);

		return completion.Task;
	}
}
