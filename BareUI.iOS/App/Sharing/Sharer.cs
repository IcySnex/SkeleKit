using LinkPresentation;
using ObjCRuntime;

namespace BareUI;

internal sealed class Sharer : ISharer
{
	public async Task ShareAsync(
		ShareContent content)
	{
		if (content is null)
			return;

		UIImage? image = content.Image is { } source ? await ResolveImage(source) : null;
		NSUrl? url = content.Url is { } address ? NSUrl.FromString(address.ToString()) : null;

		List<NSObject> activityItems = [];

		if (content.Text is { } text)
			activityItems.Add(new NSString(text));
		if (url is not null)
			activityItems.Add(url);

		// only an image needs a hand-built preview; text and a link get iOS's own (a link auto-fetches its
		// card). One metadata for the whole share, carried by the image item, so nothing races for the header.
		if (image is not null)
		{
			LPLinkMetadata metadata = new() { ImageProvider = new NSItemProvider(image) };
			if (content.Text is { } title)
				metadata.Title = title;
			if (url is not null)
				metadata.Url = metadata.OriginalUrl = url;

			activityItems.Add(new ShareItemSource(image, metadata));
		}

		if (activityItems.Count == 0 || Top() is not UIViewController top)
			return;

		UIActivityViewController controller = new([.. activityItems], null);

		// iPad presents it as a popover, which crashes without an anchor; centre it on the page
		if (controller.PopoverPresentationController is { } popover)
		{
			popover.SourceView = top.View;
			popover.SourceRect = new(top.View!.Bounds.GetMidX(), top.View.Bounds.GetMidY(), 0, 0);
			popover.PermittedArrowDirections = 0;
		}

		TaskCompletionSource completion = new();
		controller.CompletionWithItemsHandler = (_, _, _, _) => completion.SetResult();

		top.PresentViewController(controller, true, null);

		await completion.Task;

		// UIKit only retains the native peers; keep the managed side alive until the sheet is gone
		GC.KeepAlive(controller);
		GC.KeepAlive(activityItems);
	}


	// mirrors the Image control: symbol/bundle resolve locally, a URL rides the shared loader
	static async Task<UIImage?> ResolveImage(
		ImageSource source) =>
		source.Kind switch
		{
			ImageSourceKind.Symbol => UIImage.GetSystemImage(source.Value),
			ImageSourceKind.Bundle => UIImage.FromBundle(source.Value),
			ImageSourceKind.Url => await Image.Loader.LoadAsync(source.Value, default),
			_ => UIImage.FromBundle(source.Value) ?? UIImage.GetSystemImage(source.Value)
		};

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
}

// shares its item like a plain object, but also carries the one metadata that previews the whole share
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

	// see LayoutHost
	public ShareItemSource(
		NativeHandle handle) : base(handle)
	{ }


	public override NSObject GetPlaceholderData(
		UIActivityViewController activityViewController) =>
		item!;

	public override NSObject GetItemForActivity(
		UIActivityViewController activityViewController,
		NSString activityType) =>
		item!;

	public override LPLinkMetadata GetLinkMetadata(
		UIActivityViewController activityViewController) =>
		metadata!;
}
