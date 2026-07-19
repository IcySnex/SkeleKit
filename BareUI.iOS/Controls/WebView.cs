using ObjCRuntime;
using WebKit;

namespace BareUI;

/// <summary>
/// Embeds live web content in the tree, backed by a UIKit web view.
/// </summary>
/// <remarks>
/// Loads a <see cref="Url"/> or raw <see cref="Html"/>, reports navigation through <see cref="Navigated"/> and <see cref="NavigationFailed"/>, and runs JavaScript through <see cref="EvaluateAsync"/>.<br/>
/// Give it a bounded slot (a fill row, an explicit height), since web content has no intrinsic size to measure against.
/// </remarks>
public class WebView : Control
{
	sealed class NavigationPeer : WKNavigationDelegate
	{
		readonly WebView? owner;

		public NavigationPeer(
			WebView owner)
		{
			this.owner = owner;
		}

		// ReSharper disable once UnusedMember.Local
		public NavigationPeer(
			NativeHandle handle) : base(handle)
		{ }


		public override void DidFinishNavigation(
			WKWebView webView,
			WKNavigation navigation) =>
			owner?.Navigated?.Invoke(webView.Url?.AbsoluteString ?? "");

		public override void DidFailNavigation(
			WKWebView webView,
			WKNavigation navigation,
			NSError error) =>
			owner?.NavigationFailed?.Invoke(error.LocalizedDescription);

		public override void DidFailProvisionalNavigation(
			WKWebView webView,
			WKNavigation navigation,
			NSError error) =>
			owner?.NavigationFailed?.Invoke(error.LocalizedDescription);
	}


	NavigationPeer? peer;


	WKWebView Ui => (WKWebView)Native;


	/// <summary>
	/// The web address to load. Takes effect when <see cref="Html"/> is not set.
	/// </summary>
	public Bindable<string?> Url
	{
		get => url;
		set => urlBinding = Register(urlBinding, value, value => Set(ref url, value, ApplyContent, affectsMeasure: false));
	}
	string? url;
	Binding<string?>? urlBinding;

	/// <summary>
	/// Raw HTML to load, overriding <see cref="Url"/> when set.
	/// </summary>
	public Bindable<string?> Html
	{
		get => html;
		set => htmlBinding = Register(htmlBinding, value, value => Set(ref html, value, ApplyContent, affectsMeasure: false));
	}
	string? html;
	Binding<string?>? htmlBinding;

	/// <summary>
	/// Whether swiping from the screen edge navigates back and forward through history.
	/// </summary>
	public bool AllowsBackGestures
	{
		get => allowsBackGestures;
		set => Set(ref allowsBackGestures, value, ApplyBackGestures, affectsMeasure: false);
	}
	bool allowsBackGestures;

	/// <summary>
	/// Called with the final address each time a page finishes loading.
	/// </summary>
	public Action<string>? Navigated { get; set; }

	/// <summary>
	/// Called with the failure description when a load fails.
	/// </summary>
	public Action<string>? NavigationFailed { get; set; }


	static double Fill(
		double value) =>
		double.IsFinite(value) ? value : 0;

	void ApplyContent()
	{
		if (!IsRealized)
			return;

		if (html is not null)
			Ui.LoadHtmlString(html, null!);
		else if (url is not null && NSUrl.FromString(url) is NSUrl target)
			Ui.LoadRequest(new NSUrlRequest(target));
	}

	void ApplyBackGestures()
	{
		if (IsRealized)
			Ui.AllowsBackForwardNavigationGestures = allowsBackGestures;
	}


	private protected override UIView CreateNative()
	{
		WKWebView view = new(CGRect.Empty, new WKWebViewConfiguration())
		{
			BackgroundColor = UIColor.Clear,
			Opaque = false
		};

		peer = new(this);
		view.NavigationDelegate = peer;

		return view;
	}

	private protected override void ApplyProperties()
	{
		ApplyBackGestures();
		ApplyContent();
	}


	protected override Size MeasureOverride(
		Size availableSize) =>
		new(Fill(availableSize.Width), Fill(availableSize.Height));


	/// <summary>
	/// Navigates back to the previous page in history, if any.
	/// </summary>
	public void GoBack()
	{
		if (IsRealized)
			Ui.GoBack();
	}

	/// <summary>
	/// Navigates forward to the next page in history, if any.
	/// </summary>
	public void GoForward()
	{
		if (IsRealized)
			Ui.GoForward();
	}

	/// <summary>
	/// Reloads the current page.
	/// </summary>
	public void Reload()
	{
		if (IsRealized)
			Ui.Reload();
	}

	/// <summary>
	/// Runs JavaScript in the current page and returns its result as a string.
	/// </summary>
	/// <param name="javaScript">The script to evaluate.</param>
	/// <returns>The result rendered as a string, or null when there is none.</returns>
	public async Task<string?> EvaluateAsync(
		string javaScript)
	{
		if (!IsRealized)
			return null;

		NSObject result = await Ui.EvaluateJavaScriptAsync(javaScript);
		return result.ToString();
	}
}
