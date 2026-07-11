#if IOS
using CoreGraphics;
using UIKit;
#endif

namespace BareUI;

/// <summary>
/// A multi-line text input wrapping <c>UITextView</c>.
/// </summary>
public class TextEditor : Control
{
	/// <summary>
	/// The current text.
	/// </summary>
	public string? Text { get; set; }

	/// <summary>
	/// Font size in points.
	/// </summary>
	public double FontSize { get; set; } = 17;

	/// <summary>
	/// Invoked with the new value whenever the text changes.
	/// </summary>
	public Action<string>? TextChanged { get; set; }

#if IOS
	private protected override UIView CreateNative()
	{
		UITextView view = new()
		{
			Text = Text,
			Editable = true,
			Font = UIFont.SystemFontOfSize((nfloat)FontSize)
		};

		view.Changed += (sender, e) =>
		{
			Text = view.Text;
			TextChanged?.Invoke(Text ?? "");
		};

		return view;
	}

	// UITextView over-reports empty height; size from content, floor at one line
	protected override Size MeasureOverride(
		Size availableSize)
	{
		UITextView view = (UITextView)Native;

		CGSize fit = view.SizeThatFits(ClampToFinite(availableSize));

		UIFont font = view.Font ?? UIFont.SystemFontOfSize((nfloat)FontSize);
		UIEdgeInsets inset = view.TextContainerInset;
		nfloat lineFloor = (nfloat)Math.Ceiling(font.LineHeight) + inset.Top + inset.Bottom;

		nfloat resultHeight = string.IsNullOrEmpty(view.Text)
			? lineFloor
			: (nfloat)Math.Max(fit.Height, lineFloor);

		return new(fit.Width, resultHeight);
	}
#endif
}
