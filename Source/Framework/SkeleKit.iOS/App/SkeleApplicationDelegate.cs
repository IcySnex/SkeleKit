namespace SkeleKit;

/// <summary>
/// The UIKit application delegate SkeleKit registers for you.
/// </summary>
[Register(nameof(SkeleApplicationDelegate))]
public class SkeleApplicationDelegate : UIApplicationDelegate
{
	/// <summary>
	/// The application window managed by UIKit.
	/// </summary>
	public override UIWindow? Window { get; set; }
}
