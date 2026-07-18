using LinkPresentation;
using ObjCRuntime;

namespace BareUI;

internal sealed class Sharer : ISharer
{
	public async Task ShareAsync(
		params ShareItem[] items)
	{
		if (items is not { Length: > 0 })
			return;

		List<NSObject> activityItems = [];

		foreach (ShareItem item in items)
			if (await Resolve(item) is { } value)
				activityItems.Add(value);

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


	static async Task<NSObject?> Resolve(
		ShareItem item) =>
		item.Kind switch
		{
			ShareItemKind.Text => new NSString(item.Text ?? ""),
			ShareItemKind.Url => NSUrl.FromString(item.Url?.ToString() ?? ""),
			ShareItemKind.Image => item.Image is { } source && await ResolveImage(source) is { } image
				? new ImageActivityItem(image)
				: null,
			_ => null
		};

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

// a bare UIImage transfers but has no share-sheet header preview; the metadata supplies the thumbnail
internal sealed class ImageActivityItem : UIActivityItemSource
{
	readonly UIImage? image;

	public ImageActivityItem(
		UIImage image)
	{
		this.image = image;
	}

	// see LayoutHost
	public ImageActivityItem(
		NativeHandle handle) : base(handle)
	{ }


	public override NSObject GetPlaceholderData(
		UIActivityViewController activityViewController) =>
		image!;

	public override NSObject GetItemForActivity(
		UIActivityViewController activityViewController,
		NSString activityType) =>
		image!;

	public override LPLinkMetadata GetLinkMetadata(
		UIActivityViewController activityViewController) =>
		new() { ImageProvider = image is { } value ? new NSItemProvider(value) : null };
}
