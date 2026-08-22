namespace SkeleKit;

/// <summary>
/// The UIKit application delegate SkeleKit registers for you.
/// </summary>
[Register(nameof(SkeleApplicationDelegate))]
public class SkeleApplicationDelegate : UIApplicationDelegate
{
	public override UIWindow? Window { get; set; }
}
