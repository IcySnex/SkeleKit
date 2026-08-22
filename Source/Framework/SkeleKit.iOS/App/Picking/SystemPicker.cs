using ObjCRuntime;
using PhotosUI;
using UniformTypeIdentifiers;

namespace SkeleKit;

internal sealed class SystemPicker : ISystemPicker
{
	sealed class PhotoDelegate : PHPickerViewControllerDelegate
	{
		readonly TaskCompletionSource<PHPickerResult[]> completion = null!;

		public PhotoDelegate(
			TaskCompletionSource<PHPickerResult[]> completion)
		{
			this.completion = completion;
		}

		// ReSharper disable once UnusedMember.Local
		public PhotoDelegate(
			NativeHandle handle) : base(handle)
		{ }


		public override void DidFinishPicking(
			PHPickerViewController picker,
			PHPickerResult[] results)
		{
			picker.DismissViewController(true, null);
			completion.TrySetResult(results);
		}
	}

	sealed class DocumentDelegate : UIDocumentPickerDelegate
	{
		readonly TaskCompletionSource<NSUrl?> completion = null!;

		public DocumentDelegate(
			TaskCompletionSource<NSUrl?> completion)
		{
			this.completion = completion;
		}

		// ReSharper disable once UnusedMember.Local
		public DocumentDelegate(
			NativeHandle handle) : base(handle)
		{ }


		public override void DidPickDocument(
			UIDocumentPickerViewController controller,
			NSUrl[] urls) =>
			completion.TrySetResult(urls.Length > 0 ? urls[0] : null);

		public override void WasCancelled(
			UIDocumentPickerViewController controller) =>
			completion.TrySetResult(null);
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


	public async Task<PickedAsset[]?> PickImagesAsync(
		int limit = 1)
	{
		if (Top() is not UIViewController top)
			return null;

		PHPickerConfiguration configuration = new()
		{
			Filter = PHPickerFilter.ImagesFilter,
			SelectionLimit = limit
		};

		PHPickerViewController picker = new(configuration);

		TaskCompletionSource<PHPickerResult[]> completion = new();
		PhotoDelegate handler = new(completion);
		picker.Delegate = handler;

		top.PresentViewController(picker, true, null);

		PHPickerResult[] results = await completion.Task;
		if (results.Length <= 0)
			return null;

		PickedAsset[] assets = new PickedAsset[results.Length];
		for (int i = 0; i < results.Length; i++)
		{
			PHPickerResult result = results[i];

			NSData data = await result.ItemProvider.LoadDataRepresentationAsync(UTTypes.Image.Identifier);
			assets[i] = new([.. data], result.ItemProvider.SuggestedName ?? "image");
		}

		return assets;
	}

	public async Task<PickedAsset?> PickFileAsync(
		params string[] extensions)
	{
		if (Top() is not UIViewController top)
			return null;

		UTType[] types = [.. extensions.Select(UTType.CreateFromExtension).OfType<UTType>()];
		if (types.Length == 0)
			types = [UTTypes.Item];

		UIDocumentPickerViewController picker = new(types, true);

		TaskCompletionSource<NSUrl?> completion = new();
		DocumentDelegate handler = new(completion);
		picker.Delegate = handler;

		top.PresentViewController(picker, true, null);

		NSUrl? url = await completion.Task;
		if (url is null)
			return null;

		NSData data = NSData.FromUrl(url);
		return new([.. data], url.LastPathComponent ?? "file");
	}
}
