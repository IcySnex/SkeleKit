namespace BareUI.Tests.Styling;

/// <summary>
/// A styleable leaf standing in for a control: it carries one plain and one bindable property.
/// </summary>
class StubStyled : View
{
	public string? Tag { get; set; }

	string? label;
	Binding<string?>? labelBinding;

	public Bindable<string?> Label
	{
		get => label;
		set => labelBinding = Register(labelBinding, value, value => Set(ref label, value, affectsMeasure: false));
	}

	public string? CurrentLabel =>
		label;

	protected override Size MeasureOverride(
		Size availableSize) =>
		Size.Zero;
}

/// <summary>
/// A subtype of <see cref="StubStyled"/>, so the theme's inheritance walk has something to walk.
/// </summary>
sealed class StubStyledLeaf : StubStyled;

/// <summary>
/// An unrelated view, for type-mismatch checks.
/// </summary>
sealed class StubOther : View
{
	protected override Size MeasureOverride(
		Size availableSize) =>
		Size.Zero;
}
