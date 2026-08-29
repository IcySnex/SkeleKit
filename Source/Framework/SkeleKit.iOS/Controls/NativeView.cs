namespace SkeleKit;

/// <summary>
/// Escape hatch to embed any UIKit view in a SkeleKit tree.
/// </summary>
public class NativeView : Control
{
	readonly UIView? view;
	readonly Func<UIView>? create;

	/// <summary>
	/// Creates a wrapper that borrows an existing UIKit view.
	/// </summary>
	/// <remarks>
	/// The caller retains ownership; the wrapper removes the view from its hierarchy but never disposes it.
	/// </remarks>
	/// <param name="view">The caller-owned UIKit view to embed.</param>
	public NativeView(
		UIView view)
	{
		ArgumentNullException.ThrowIfNull(view);
		this.view = view;
	}

	/// <summary>
	/// Creates an owned UIKit view whenever the wrapper is realized.
	/// </summary>
	/// <remarks>
	/// The wrapper disposes each created view when it is unrealized. The factory must return a new view on every call.
	/// </remarks>
	/// <param name="create">Creates the UIKit view to embed.</param>
	public NativeView(
		Func<UIView> create)
	{
		ArgumentNullException.ThrowIfNull(create);
		this.create = create;
	}


	private protected override bool OwnsNative => create is not null;

	private protected override UIView CreateNative() =>
		create is null
			? view!
			: create() ?? throw new InvalidOperationException("The NativeView factory returned null.");
}
