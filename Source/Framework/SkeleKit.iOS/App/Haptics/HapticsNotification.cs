namespace SkeleKit;

/// <summary>
/// The type of notification event for haptic feedback.
/// </summary>
public enum HapticsNotification
{
	/// <summary>
	/// Indicates a task completed successfully.
	/// </summary>
	Success,

	/// <summary>
	/// Indicates a condition that requires user attention.
	/// </summary>
	Warning,

	/// <summary>
	/// Indicates a failed operation or critical error.
	/// </summary>
	Error
}
