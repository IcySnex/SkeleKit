using System.Collections.Specialized;
using System.Windows.Input;
using ObjCRuntime;
using static SkeleKit.TextAlignment;

namespace SkeleKit;

/// <summary>
/// Read-only rich text that can be selected, with tappable <see cref="Link"/> runs.
/// </summary>
public class TextView : Control
{
	sealed class TextItemDelegate : UITextViewDelegate
	{
		readonly TextView? owner;

		public TextItemDelegate(
			TextView owner)
		{
			this.owner = owner;
		}

		// ReSharper disable once UnusedMember.Local
		public TextItemDelegate(
			NativeHandle handle) : base(handle)
		{ }


		public override UIAction? GetPrimaryAction(
			UITextView textView,
			UITextItem textItem,
			UIAction defaultAction) =>
			owner?.PrimaryAction(textItem, defaultAction);

		public override UITextItemMenuConfiguration? GetMenuConfiguration(
			UITextView textView,
			UITextItem textItem,
			UIMenu defaultMenu) =>
			owner?.MenuConfiguration(textItem);
	}


	// links matched by range in the delegate
	readonly List<(NSRange range, Link link)> linkRanges = [];

	TextItemDelegate? peer;
	UIAction? heldPrimary;
	UIAction[]? heldMenu;

	bool hooked;


	UITextView Ui => (UITextView)Native;


	/// <summary>
	/// The styled runs to display; a plain string becomes an unstyled run, a <see cref="Link"/> a tappable one.
	/// </summary>
	/// <remarks>
	/// Changes re-render and animate nothing, since they replace the text.<br/>
	/// Live when the list is an <c>ObservableCollection</c>.
	/// </remarks>
	public BindableList<Span> Spans
	{
		get => new(spans);
		set => spansBinding = Register(spansBinding, value.Expression, value.Value, SetSpans);
	}
	IReadOnlyList<Span>? spans;
	Binding<IReadOnlyList<Span>?>? spansBinding;

	/// <summary>
	/// Whether the text can be selected and copied.
	/// </summary>
	/// <remarks>
	/// A <see cref="Link"/> run forces selection on, since UIKit only makes text items tappable while the view is selectable.
	/// </remarks>
	public Bindable<bool> IsSelectable
	{
		get => isSelectable;
		set => isSelectableBinding = Register(isSelectableBinding, value, value => Set(ref isSelectable, value, ApplySelectable, affectsMeasure: false));
	}
	bool isSelectable;
	Binding<bool>? isSelectableBinding;

	/// <summary>
	/// The step of the native type hierarchy the text follows, or null to size it by <see cref="FontSize"/>.
	/// </summary>
	public TextStyle? TextStyle
	{
		get => textStyle;
		set => Set(ref textStyle, value, ApplyText);
	}
	TextStyle? textStyle;

	/// <summary>
	/// Explicit font size in points, overriding <see cref="TextStyle"/>.
	/// </summary>
	/// <remarks>
	/// NaN falls back to the text style, or 17 points without one.
	/// </remarks>
	public double FontSize
	{
		get => fontSize;
		set => Set(ref fontSize, value, ApplyText);
	}
	double fontSize = double.NaN;

	/// <summary>
	/// The base font weight the runs build on.
	/// </summary>
	public FontWeight FontWeight
	{
		get => weight;
		set => Set(ref weight, value, ApplyText);
	}
	FontWeight weight = FontWeight.Regular;

	/// <summary>
	/// The base font design: system, rounded, serif or monospaced.
	/// </summary>
	public FontDesign FontDesign
	{
		get => design;
		set => Set(ref design, value, ApplyText);
	}
	FontDesign design = FontDesign.Default;

	/// <summary>
	/// Base text color, or null for the system label color.
	/// </summary>
	public Bindable<Color?> TextColor
	{
		get => textColor;
		set => textColorBinding = Register(textColorBinding, value, value => Set(ref textColor, value, ApplyText, affectsMeasure: false));
	}
	Color? textColor;
	Binding<Color?>? textColorBinding;

	/// <summary>
	/// Color the links paint in, or null for the app tint.
	/// </summary>
	public Color? LinkColor
	{
		get => linkColor;
		set => Set(ref linkColor, value, ApplyText, affectsMeasure: false);
	}
	Color? linkColor;

	/// <summary>
	/// Maximum number of lines, or 0 for unlimited (wraps freely).
	/// </summary>
	public int MaxLines
	{
		get => maxLines;
		set => Set(ref maxLines, value, ApplyMaxLines);
	}
	int maxLines;

	/// <summary>
	/// Horizontal alignment of the text.
	/// </summary>
	public TextAlignment TextAlignment
	{
		get => textAlignment;
		set => Set(ref textAlignment, value, ApplyText);
	}
	TextAlignment textAlignment = Leading;

	/// <summary>
	/// Extra points between lines.
	/// </summary>
	public double LineSpacing
	{
		get => lineSpacing;
		set => Set(ref lineSpacing, value, ApplyText);
	}
	double lineSpacing;

	/// <summary>
	/// Extra points between characters (negative tightens).
	/// </summary>
	public double LetterSpacing
	{
		get => letterSpacing;
		set => Set(ref letterSpacing, value, ApplyText);
	}
	double letterSpacing;


	void SetSpans(
		IReadOnlyList<Span>? value)
	{
		if (ReferenceEquals(spans, value))
			return;

		if (hooked && spans is INotifyCollectionChanged old)
			old.CollectionChanged -= OnSpansChanged;

		spans = value;

		if (hooked && spans is INotifyCollectionChanged live)
			live.CollectionChanged += OnSpansChanged;

		if (IsRealized)
			ApplyText();

		InvalidateMeasure();
	}

	void HookSpans()
	{
		if (hooked)
			return;

		hooked = true;

		if (spans is INotifyCollectionChanged live)
			live.CollectionChanged += OnSpansChanged;
	}

	void UnhookSpans()
	{
		if (!hooked)
			return;

		if (spans is INotifyCollectionChanged live)
			live.CollectionChanged -= OnSpansChanged;

		hooked = false;
	}

	void OnSpansChanged(
		object? sender,
		NotifyCollectionChangedEventArgs args)
	{
		ApplyText();
		InvalidateMeasure();
	}

	void ApplyText()
	{
		if (!IsRealized)
			return;

		linkRanges.Clear();

		NSMutableParagraphStyle paragraph = new()
		{
			LineSpacing = (nfloat)lineSpacing,
			Alignment = Alignment()
		};

		UIColor baseColor = textColor?.ToUIColor() ?? UIColor.Label;
		UIColor link = linkColor?.ToUIColor() ?? Tint?.ToUIColor() ?? UIColor.Link;

		// links use the view's own link style; per-run colors touch plain runs only
		Ui.WeakLinkTextAttributes = new UIStringAttributes
		{
			ForegroundColor = link,
			UnderlineStyle = NSUnderlineStyle.Single
		}.Dictionary;

		NSMutableAttributedString composed = new();

		foreach (Span span in spans ?? [])
		{
			int start = (int)composed.Length;

			UIStringAttributes attributes = new()
			{
				ParagraphStyle = paragraph,
				Font = FontFor(span),
				ForegroundColor = span.TextColor?.ToUIColor() ?? baseColor
			};

			if (letterSpacing is not 0)
				attributes.KerningAdjustment = (float)letterSpacing;
			if (span.Underline)
				attributes.UnderlineStyle = NSUnderlineStyle.Single;
			if (span.Strikethrough)
				attributes.StrikethroughStyle = NSUnderlineStyle.Single;

			// the URL just carries our link index, never opens
			if (span is Link tappable)
			{
				attributes.Link = new($"skelekit://link/{linkRanges.Count}");
				linkRanges.Add((new(start, span.Text.Length), tappable));
			}

			composed.Append(new(span.Text, attributes));
		}

		Ui.AttributedText = composed;
		ApplySelectable();
	}

	void ApplySelectable() =>
		Ui.Selectable = isSelectable || linkRanges.Count > 0;

	void ApplyMaxLines()
	{
		Ui.TextContainer.MaximumNumberOfLines = (nuint)maxLines;
		Ui.TextContainer.LineBreakMode = maxLines > 0 ? UILineBreakMode.TailTruncation : UILineBreakMode.WordWrap;
		ApplyText();
	}

	UITextAlignment Alignment() =>
		textAlignment switch
		{
			Center => UITextAlignment.Center,
			Trailing => UITextAlignment.Right,
			_ => UITextAlignment.Left
		};

	UIFont FontFor(
		Span span)
	{
		FontWeight w = span.Bold ? FontWeight.Bold : span.FontWeight ?? weight;
		FontDesign d = span.FontDesign ?? design;
		double size = double.IsNaN(span.FontSize) ? fontSize : span.FontSize;

		return FontSpec.UsesTextStyle(textStyle, size)
			? Fonts.Preferred(textStyle!.Value, w, d)
			: Fonts.Scaled(FontSpec.SizeOf(size), w, d);
	}

	Link? LinkFor(
		UITextItem item)
	{
		foreach ((NSRange range, Link link) in linkRanges)
		{
			if (item.Range.Location >= range.Location && item.Range.Location < range.Location + range.Length)
				return link;
		}

		return null;
	}


	private protected override UIView CreateNative()
	{
		UITextView view = new()
		{
			BackgroundColor = UIColor.Clear,
			Editable = false,
			ScrollEnabled = false,
			Selectable = false,
			TextContainerInset = UIEdgeInsets.Zero,
			AdjustsFontForContentSizeCategory = true
		};

		view.TextContainer.LineFragmentPadding = 0;

		peer = new(this);
		view.Delegate = peer;

		return view;
	}

	private protected override void ApplyProperties()
	{
		HookSpans();
		ApplyMaxLines();
	}

	private protected override void OnUnrealized() =>
		UnhookSpans();


	internal override void TintChanged()
	{
		if (IsRealized)
			ApplyText();
	}


	internal UIAction? PrimaryAction(
		UITextItem item,
		UIAction fallback)
	{
		if (LinkFor(item) is not { Command: ICommand command } link)
			return fallback;

		heldPrimary = UIAction.Create(_ =>
		{
			if (command.CanExecute(link.CommandParameter))
				command.Execute(link.CommandParameter);
		});

		return heldPrimary;
	}

	internal UITextItemMenuConfiguration? MenuConfiguration(
		UITextItem item)
	{
		if (LinkFor(item) is not { ContextMenu.Count: > 0 } link)
			return null;

		heldMenu = new UIAction[link.ContextMenu.Count];

		for (int index = 0; index < link.ContextMenu.Count; index++)
		{
			MenuAction entry = link.ContextMenu[index];

			heldMenu[index] = UIAction.Create(
				entry.Text,
				entry.Icon is string icon ? UIImage.GetSystemImage(icon) : null,
				null,
				_ =>
				{
					if (entry.Command is ICommand command && command.CanExecute(entry.CommandParameter))
						command.Execute(entry.CommandParameter);
				});

			if (entry.IsDestructive)
				heldMenu[index].Attributes = UIMenuElementAttributes.Destructive;
		}

		return UITextItemMenuConfiguration.Create(UIMenu.Create(heldMenu));
	}
}
