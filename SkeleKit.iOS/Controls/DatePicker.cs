namespace SkeleKit;

/// <summary>
/// A date and time picker.
/// </summary>
public class DatePicker : Control
{
	static NSDate ToNative(
		DateTime value) =>
		(NSDate)DateTime.SpecifyKind(value, value.Kind is DateTimeKind.Unspecified ? DateTimeKind.Local : value.Kind).ToUniversalTime();


	UIDatePicker Ui => (UIDatePicker)Native;


	/// <summary>
	/// The picked date, in local time. Two-way by default.
	/// </summary>
	public Bindable<DateTime> Date
	{
		get => date;
		set => dateBinding = Register(dateBinding, value, value => Set(ref date, value, ApplyDate, affectsMeasure: false));
	}
	DateTime date = DateTime.Now;
	Binding<DateTime>? dateBinding;

	/// <summary>
	/// What the picker lets the user pick.
	/// </summary>
	public DatePickerMode Mode
	{
		get => mode;
		set => Set(ref mode, value, ApplyStyle);
	}
	DatePickerMode mode = DatePickerMode.Date;

	/// <summary>
	/// How the picker presents itself.
	/// </summary>
	public DatePickerStyle Kind
	{
		get => kind;
		set => Set(ref kind, value, ApplyStyle);
	}
	DatePickerStyle kind = DatePickerStyle.Compact;

	/// <summary>
	/// The earliest pickable date, or null for no bound.
	/// </summary>
	public DateTime? Minimum
	{
		get => minimum;
		set => Set(ref minimum, value, ApplyRange, affectsMeasure: false);
	}
	DateTime? minimum;

	/// <summary>
	/// The latest pickable date, or null for no bound.
	/// </summary>
	public DateTime? Maximum
	{
		get => maximum;
		set => Set(ref maximum, value, ApplyRange, affectsMeasure: false);
	}
	DateTime? maximum;

	/// <summary>
	/// Invoked with the new value whenever the user picks a date.
	/// </summary>
	public Action<DateTime>? DateChanged { get; set; }


	void ApplyStyle()
	{
		Ui.Mode = mode switch
		{
			DatePickerMode.Time => UIDatePickerMode.Time,
			DatePickerMode.DateAndTime => UIDatePickerMode.DateAndTime,
			_ => UIDatePickerMode.Date
		};

		Ui.PreferredDatePickerStyle = kind switch
		{
			DatePickerStyle.Inline => UIDatePickerStyle.Inline,
			DatePickerStyle.Wheels => UIDatePickerStyle.Wheels,
			_ => UIDatePickerStyle.Compact
		};
	}

	void ApplyRange()
	{
		Ui.MinimumDate = minimum is DateTime min ? ToNative(min) : null;
		Ui.MaximumDate = maximum is DateTime max ? ToNative(max) : null;
	}

	void ApplyDate() =>
		Ui.Date = ToNative(date);

	void OnDateChanged()
	{
		DateTime value = ((DateTime)Ui.Date).ToLocalTime();

		Set(ref date, value, affectsMeasure: false);
		dateBinding?.PushToSource(value);
		DateChanged?.Invoke(value);
	}


	protected override Size MeasureOverride(
		Size availableSize)
	{
		Size fallback = base.MeasureOverride(availableSize);
		CGSize intrinsic = Ui.IntrinsicContentSize;

		return new(
			intrinsic.Width == UIView.NoIntrinsicMetric ? fallback.Width : intrinsic.Width,
			intrinsic.Height == UIView.NoIntrinsicMetric ? fallback.Height : intrinsic.Height);
	}


	private protected override UIView CreateNative()
	{
		UIDatePicker picker = new();
		picker.ValueChanged += (_, _) => OnDateChanged();

		return picker;
	}

	private protected override void ApplyProperties()
	{
		ApplyStyle();
		ApplyRange();
		ApplyDate();
	}
}
