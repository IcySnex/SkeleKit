using Foundation;
using UIKit;

namespace BareUI.Gallery;

// Temporary manual bootstrap — replaced by BareApp once the app model (M4) exists.
[Register(nameof(AppDelegate))]
public class AppDelegate : UIApplicationDelegate
{
	public override bool FinishedLaunching(
		UIApplication application,
		NSDictionary launchOptions) =>
		true;
}
