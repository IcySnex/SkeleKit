namespace SkeleKit;

/// <summary>
/// What a <c>DatePicker</c> lets the user pick.
/// </summary>
public enum DatePickerMode
{
	/// <summary>
	/// A calendar date.
	/// </summary>
	Date,

	/// <summary>
	/// A time of day.
	/// </summary>
	Time,

	/// <summary>
	/// A date and a time together.
	/// </summary>
	DateAndTime
}

/// <summary>
/// How a <c>DatePicker</c> presents itself.
/// </summary>
public enum DatePickerStyle
{
	/// <summary>
	/// A compact pill that expands into a popover. The default.
	/// </summary>
	Compact,

	/// <summary>
	/// The full calendar or clock, laid out inline.
	/// </summary>
	Inline,

	/// <summary>
	/// The classic spinning wheels.
	/// </summary>
	Wheels
}
