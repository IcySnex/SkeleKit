using Foundation;
using UIKit;

namespace BareUI.Gallery;

// temp bootstrap, replaced by BareApp in M4
[Register(nameof(AppDelegate))]
public class AppDelegate : UIApplicationDelegate
{
	public override bool FinishedLaunching(
		UIApplication application,
		NSDictionary launchOptions) =>
		true;
}
