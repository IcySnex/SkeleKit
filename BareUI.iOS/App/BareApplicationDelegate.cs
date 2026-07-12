namespace BareUI;

/// <summary>
/// <inheritdoc/>
/// </summary>
[Register(nameof(BareApplicationDelegate))]
public class BareApplicationDelegate : UIApplicationDelegate
{
	public override UIWindow? Window { get; set; }
}
