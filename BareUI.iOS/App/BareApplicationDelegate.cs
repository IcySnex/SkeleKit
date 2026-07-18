namespace BareUI;

/// <summary>
/// The UIKit application delegate BareUI registers for you.
/// </summary>
[Register(nameof(BareApplicationDelegate))]
public class BareApplicationDelegate : UIApplicationDelegate
{
	public override UIWindow? Window { get; set; }
}
