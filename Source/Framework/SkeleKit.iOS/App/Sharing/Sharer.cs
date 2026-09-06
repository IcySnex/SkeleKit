using LinkPresentation;
using Microsoft.Extensions.Logging;
using ObjCRuntime;

namespace SkeleKit;

internal sealed class Sharer(
	ILogger<Sharer> logger) : ISharer
{
	internal sealed class ShareItemSource : UIActivityItemSource
	{
		readonly NSObject? item;
		readonly LPLinkMetadata? metadata;

		public ShareItemSource(
			NSObject item,
			LPLinkMetadata metadata)
		{
			this.item = item;
			this.metadata = metadata;
		}

		public ShareItemSource(
			NativeHandle handle) : base(handle)
		{ }


		public override NSObject GetPlaceholderData(
			UIActivityViewController activityViewController) =>
			item!;

		public override NSObject GetItemForActivity(
			UIActivityViewController activityViewController,
			NSString? activityType) =>
			item!;

		public override LPLinkMetadata GetLinkMetadata(
			UIActivityViewController activityViewController) =>
			metadata!;
	}


	static async Task<UIImage?> ResolveImage(
		ImageSource source) =>
		source.Kind is ImageSourceKind.Url
			? await Image.Loader.LoadAsync(source.Value, CancellationToken.None)
			: source.ResolveLocal();

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


	public async Task ShareAsync(
		ShareContent content)
	{
		UIImage? image = content.Image is ImageSource source ? await ResolveImage(source) : null;
		NSUrl? url = content.Url is Uri address ? NSUrl.FromString(address.ToString()) : null;

		List<NSObject> activityItems = [];

		if (content.Text is string text)
			activityItems.Add(new NSString(text));
		if (url is not null)
			activityItems.Add(url);

		if (image is not null)
		{
			LPLinkMetadata metadata = new() { ImageProvider = new(image) };
			if (content.Text is string title)
				metadata.Title = title;
			if (url is not null)
				metadata.Url = metadata.OriginalUrl = url;

			activityItems.Add(new ShareItemSource(image, metadata));
		}

		if (activityItems.Count == 0)
		{
			logger.LogWarning("The share request did not contain usable text, a URL, or an image.");
			return;
		}
		if (Top() is not UIViewController top)
		{
			logger.LogWarning("Could not present share sheet because no active view controller is available.");
			return;
		}

		UIActivityViewController controller = new([.. activityItems], null);

		if (controller.PopoverPresentationController is UIPopoverPresentationController popover && top.View is not null)
		{
			popover.SourceView = top.View;
			popover.SourceRect = new(top.View!.Bounds.GetMidX(), top.View.Bounds.GetMidY(), 0, 0);
			popover.PermittedArrowDirections = 0;
		}

		TaskCompletionSource completion = new();
		controller.CompletionWithItemsHandler = (_, _, _, _) => completion.SetResult();

		top.PresentViewController(controller, true, null);

		await completion.Task;

		GC.KeepAlive(controller);
		GC.KeepAlive(activityItems);
	}
}
