using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Input;
using CoreLocation;
using MapKit;
using ObjCRuntime;

namespace SkeleKit;

/// <summary>
/// Embeds an interactive map in the tree, backed by a UIKit map view.
/// </summary>
/// <remarks>
/// <c>ShowsUserLocation</c> needs <c>NSLocationWhenInUseUsageDescription</c> in the app plist.
/// </remarks>
public class MapView : Control
{
	sealed class PinAnnotation : MKPointAnnotation
	{
		public PinAnnotation(
			MapPin pin) : base(new(pin.Coordinate.Latitude, pin.Coordinate.Longitude), pin.Title!, pin.Subtitle!)
		{
			Pin = pin;
		}

		// ReSharper disable once UnusedMember.Local
		public PinAnnotation(
			NativeHandle handle) : base(handle)
		{ }


		public MapPin Pin { get; } = null!;
	}


	sealed class ClusterAnnotation : MKClusterAnnotation
	{
		public ClusterAnnotation(
			IMKAnnotation[] members) : base(members)
		{ }

		// ReSharper disable once UnusedMember.Local
		public ClusterAnnotation(
			NativeHandle handle) : base(handle)
		{ }
	}


	sealed class MarkerHost : MKAnnotationView
	{
		View? content;

		public MarkerHost(
			IMKAnnotation annotation,
			string reuseIdentifier) : base(annotation, reuseIdentifier)
		{ }

		// ReSharper disable once UnusedMember.Local
		public MarkerHost(
			NativeHandle handle) : base(handle)
		{ }


		public void SetContent(
			View view)
		{
			content?.Native.RemoveFromSuperview();
			content = view;

			UIView native = view.Realize();
			view.Measure(new(double.PositiveInfinity, double.PositiveInfinity));
			Size size = view.DesiredSize;

			Bounds = new(0, 0, size.Width, size.Height);
			view.Arrange(new(0, 0, size.Width, size.Height));
			AddSubview(native);
		}
	}


	sealed class ContentHost : UIView
	{
		readonly View? content;

		public ContentHost(
			View content)
		{
			this.content = content;

			content.Measure(new(double.PositiveInfinity, double.PositiveInfinity));
			Size size = content.DesiredSize;

			Frame = new(0, 0, size.Width, size.Height);
			AddSubview(content.Realize());
			content.Arrange(new(0, 0, size.Width, size.Height));
		}

		// ReSharper disable once UnusedMember.Local
		public ContentHost(
			NativeHandle handle) : base(handle)
		{ }


		public override CGSize IntrinsicContentSize => content is null ? CGSize.Empty : new((nfloat)content.DesiredSize.Width, (nfloat)content.DesiredSize.Height);


		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			content?.Arrange(new(0, 0, Bounds.Width, Bounds.Height));
		}
	}


	sealed class MapPeer : MKMapViewDelegate
	{
		const string MarkerId = "skele.pin";
		const string CustomId = "skele.pin.custom";
		const string ClusterId = "skele.cluster";
		const string ClusterCustomId = "skele.cluster.custom";
		const string ClusterGroup = "skele.pins";


		static void ApplyCallout(
			MKAnnotationView view,
			MapPin pin,
			MapView? owner)
		{
			if (pin.Callout is Func<View> callout)
			{
				ContentHost host = new(callout());
				owner?.Root(host);

				view.CanShowCallout = true;
				view.DetailCalloutAccessoryView = host;

				return;
			}

			view.CanShowCallout = pin.Title is not null || pin.Subtitle is not null;
			view.DetailCalloutAccessoryView = null;
		}


		readonly MapView? owner;
		double selectionSpan;

		public MapPeer(
			MapView owner)
		{
			this.owner = owner;
		}

		// ReSharper disable once UnusedMember.Local
		public MapPeer(
			NativeHandle handle) : base(handle)
		{ }


		public override MKAnnotationView? GetViewForAnnotation(
			MKMapView mapView,
			IMKAnnotation annotation)
		{
			if (annotation is ClusterAnnotation cluster)
			{
				int count = cluster.MemberAnnotations.Length;

				if (owner?.ClusterMarker is Func<int, View> clusterMarker)
				{
					MarkerHost clusterHost = mapView.DequeueReusableAnnotation(ClusterCustomId) as MarkerHost ?? new MarkerHost(cluster, ClusterCustomId);

					clusterHost.Annotation = cluster;
					clusterHost.SetContent(clusterMarker(count));
					owner?.Root(clusterHost);

					return clusterHost;
				}

				MKMarkerAnnotationView clusterView = mapView.DequeueReusableAnnotation(ClusterId) as MKMarkerAnnotationView ?? new MKMarkerAnnotationView(cluster, ClusterId);

				clusterView.Annotation = cluster;
				clusterView.GlyphText = count.ToString(CultureInfo.CurrentCulture);

				return clusterView;
			}

			if (annotation is not PinAnnotation pin)
				return null;

			string? clusterId = owner?.ClustersPins == true ? ClusterGroup : null;

			if (pin.Pin.Marker is Func<View> marker)
			{
				MarkerHost host = mapView.DequeueReusableAnnotation(CustomId) as MarkerHost ?? new MarkerHost(annotation, CustomId);

				host.Annotation = annotation;
				host.ClusteringIdentifier = clusterId;
				host.SetContent(marker());
				owner?.Root(host);
				ApplyCallout(host, pin.Pin, owner);

				return host;
			}

			MKMarkerAnnotationView view = mapView.DequeueReusableAnnotation(MarkerId) as MKMarkerAnnotationView ?? new MKMarkerAnnotationView(annotation, MarkerId);

			view.Annotation = annotation;
			view.ClusteringIdentifier = clusterId;
			view.MarkerTintColor = pin.Pin.Tint?.ToUIColor();
			view.GlyphImage = pin.Pin.Symbol is string symbol ? UIImage.GetSystemImage(symbol) : null;
			ApplyCallout(view, pin.Pin, owner);

			return view;
		}

		public override MKClusterAnnotation CreateClusterAnnotation(
			MKMapView mapView,
			IMKAnnotation[] memberAnnotations) =>
			new ClusterAnnotation(memberAnnotations);

		public override MKOverlayRenderer OverlayRenderer(
			MKMapView mapView,
			IMKOverlay overlay)
		{
			MKOverlayPathRenderer? renderer = overlay switch
			{
				MKPolyline line => new MKPolylineRenderer(line),
				MKPolygon polygon => new MKPolygonRenderer(polygon),
				MKCircle circle => new MKCircleRenderer(circle),
				_ => null
			};

			if (renderer is null)
				return new(overlay);

			if (owner?.ModelFor(overlay) is MapOverlay model)
			{
				renderer.StrokeColor = model.StrokeColor?.ToUIColor();
				renderer.LineWidth = (nfloat)model.StrokeWidth;
				renderer.FillColor = model.FillColor?.ToUIColor();

				if (model.LineDash is double[] dash)
				{
					NSNumber[] pattern = new NSNumber[dash.Length];

					for (int index = 0; index < dash.Length; index++)
						pattern[index] = NSNumber.FromDouble(dash[index]);

					renderer.LineDashPattern = pattern;
				}
			}

			return renderer;
		}

		public override void DidChangeVisibleRegion(
			MKMapView mapView)
		{
			if (selectionSpan > 0 && mapView.Region.Span.LatitudeDelta > selectionSpan * 1.5)
			{
				foreach (IMKAnnotation annotation in mapView.SelectedAnnotations)
					mapView.DeselectAnnotation(annotation, false);

				selectionSpan = 0;
			}

			owner?.OnRegionChanged();
		}

		public override void DidSelectAnnotation(
			MKMapView mapView,
			IMKAnnotation annotation)
		{
			selectionSpan = mapView.Region.Span.LatitudeDelta;

			if (annotation is PinAnnotation pin)
				owner?.OnPinSelected(pin.Pin);
		}

		public override void DidDeselectAnnotation(
			MKMapView mapView,
			IMKAnnotation annotation) =>
			selectionSpan = 0;
	}


	static double Fill(
		double value) =>
		double.IsFinite(value) ? value : 0;

	static MKMapType ToMapType(
		MapKind kind) =>
		kind switch
		{
			MapKind.Muted => MKMapType.MutedStandard,
			MapKind.Satellite => MKMapType.Satellite,
			MapKind.Hybrid => MKMapType.Hybrid,
			_ => MKMapType.Standard
		};

	static CLLocationCoordinate2D[] ToCoordinates(
		Coordinate[] points)
	{
		CLLocationCoordinate2D[] result = new CLLocationCoordinate2D[points.Length];

		for (int index = 0; index < points.Length; index++)
			result[index] = new(points[index].Latitude, points[index].Longitude);

		return result;
	}

	static IMKOverlay ToNativeOverlay(
		MapOverlay overlay) =>
		overlay switch
		{
			MapPolyline line => MKPolyline.FromCoordinates(ToCoordinates(line.Points)),
			MapPolygon polygon => MKPolygon.FromCoordinates(ToCoordinates(polygon.Points)),
			MapCircle circle => MKCircle.Circle(new(circle.Center.Latitude, circle.Center.Longitude), circle.RadiusMeters),
			_ => throw new NotSupportedException()
		};

	static bool NearlyEquals(
		MKCoordinateRegion native,
		MapRegion region) =>
		Math.Abs(native.Center.Latitude - region.Center.Latitude) < 1e-6 &&
		Math.Abs(native.Center.Longitude - region.Center.Longitude) < 1e-6 &&
		Math.Abs(native.Span.LatitudeDelta - region.LatitudeSpan) < 1e-6 &&
		Math.Abs(native.Span.LongitudeDelta - region.LongitudeSpan) < 1e-6;

	static MapRegion FromNative(
		MKCoordinateRegion native) =>
		new(new(native.Center.Latitude, native.Center.Longitude), native.Span.LatitudeDelta, native.Span.LongitudeDelta);

	static MKCoordinateRegion ToNative(
		MapRegion region) =>
		new(new(region.Center.Latitude, region.Center.Longitude), new(region.LatitudeSpan, region.LongitudeSpan));


	readonly List<PinAnnotation> added = [];
	// ReSharper disable once CollectionNeverQueried.Local
	readonly List<UIView> rootedHosts = [];
	readonly List<IMKOverlay> nativeOverlays = [];
	readonly Dictionary<IMKOverlay, MapOverlay> overlayModels = [];
	MapPeer? peer;
	bool hooked;
	bool applyingRegion;


	MKMapView Ui => (MKMapView)Native;


	/// <summary>
	/// The visible extent, updated two-way as the user pans and zooms.
	/// </summary>
	public Bindable<MapRegion> Region
	{
		get => region;
		set => regionBinding = Register(regionBinding, value, value => Set(ref region, value, ApplyRegion, affectsMeasure: false));
	}
	MapRegion region;
	Binding<MapRegion>? regionBinding;

	/// <summary>
	/// The base imagery the map draws.
	/// </summary>
	public MapKind Kind
	{
		get => kind;
		set => Set(ref kind, value, ApplyKind, affectsMeasure: false);
	}
	MapKind kind;

	/// <summary>
	/// Whether the blue dot marking the user's location is shown.
	/// </summary>
	public bool ShowsUserLocation
	{
		get => showsUserLocation;
		set => Set(ref showsUserLocation, value, ApplyChrome, affectsMeasure: false);
	}
	bool showsUserLocation;

	/// <summary>
	/// Whether the user can pan the map.
	/// </summary>
	public bool ScrollEnabled
	{
		get => scrollEnabled;
		set => Set(ref scrollEnabled, value, ApplyInteractions, affectsMeasure: false);
	}
	bool scrollEnabled = true;

	/// <summary>
	/// Whether the user can zoom the map.
	/// </summary>
	public bool ZoomEnabled
	{
		get => zoomEnabled;
		set => Set(ref zoomEnabled, value, ApplyInteractions, affectsMeasure: false);
	}
	bool zoomEnabled = true;

	/// <summary>
	/// Whether the user can rotate the map.
	/// </summary>
	public bool RotateEnabled
	{
		get => rotateEnabled;
		set => Set(ref rotateEnabled, value, ApplyInteractions, affectsMeasure: false);
	}
	bool rotateEnabled = true;

	/// <summary>
	/// Whether the user can tilt the map into a 3D pitch.
	/// </summary>
	public bool PitchEnabled
	{
		get => pitchEnabled;
		set => Set(ref pitchEnabled, value, ApplyInteractions, affectsMeasure: false);
	}
	bool pitchEnabled = true;

	/// <summary>
	/// Whether the compass appears when the map is rotated.
	/// </summary>
	public bool ShowsCompass
	{
		get => showsCompass;
		set => Set(ref showsCompass, value, ApplyChrome, affectsMeasure: false);
	}
	bool showsCompass = true;

	/// <summary>
	/// Whether a distance scale appears while zooming.
	/// </summary>
	public bool ShowsScale
	{
		get => showsScale;
		set => Set(ref showsScale, value, ApplyChrome, affectsMeasure: false);
	}
	bool showsScale;

	/// <summary>
	/// Whether live traffic is drawn.
	/// </summary>
	public bool ShowsTraffic
	{
		get => showsTraffic;
		set => Set(ref showsTraffic, value, ApplyChrome, affectsMeasure: false);
	}
	bool showsTraffic;

	/// <summary>
	/// The markers dropped on the map.
	/// </summary>
	public BindableList<MapPin> Pins
	{
		get => new(pins);
		set => pinsBinding = Register(pinsBinding, value.Expression, value.Value, SetPins);
	}
	IReadOnlyList<MapPin> pins = [];
	Binding<IReadOnlyList<MapPin>?>? pinsBinding;

	/// <summary>
	/// The shapes drawn on the map beneath its pins.
	/// </summary>
	public BindableList<MapOverlay> Overlays
	{
		get => new(overlays);
		set => overlaysBinding = Register(overlaysBinding, value.Expression, value.Value, SetOverlays);
	}
	IReadOnlyList<MapOverlay> overlays = [];
	Binding<IReadOnlyList<MapOverlay>?>? overlaysBinding;

	/// <summary>
	/// Whether nearby pins collapse into a single counted marker that splits apart on zoom.
	/// </summary>
	public bool ClustersPins
	{
		get;
		set => Set(ref field, value, ReloadPins, affectsMeasure: false);
	}

	/// <summary>
	/// Builds a custom view for a cluster from its pin count, or null for the native counted marker.
	/// </summary>
	/// <remarks>
	/// Only used while <see cref="ClustersPins"/> is on.
	/// </remarks>
	public Func<int, View>? ClusterMarker
	{
		get;
		set => Set(ref field, value, ReloadPins, affectsMeasure: false);
	}

	/// <summary>
	/// Command invoked with the tapped pin.
	/// </summary>
	public ICommand? PinCommand { get; set; }


	void ApplyRegion()
	{
		if (!IsRealized || region.LatitudeSpan <= 0 || region.LongitudeSpan <= 0 || NearlyEquals(Ui.Region, region))
			return;

		SetRegion(region, animated: false);
	}

	void ApplyKind()
	{
		if (IsRealized)
			Ui.MapType = ToMapType(kind);
	}

	void ApplyInteractions()
	{
		if (!IsRealized)
			return;

		Ui.ScrollEnabled = scrollEnabled;
		Ui.ZoomEnabled = zoomEnabled;
		Ui.RotateEnabled = rotateEnabled;
		Ui.PitchEnabled = pitchEnabled;
	}

	void ApplyChrome()
	{
		if (!IsRealized)
			return;

		Ui.ShowsUserLocation = showsUserLocation;
		Ui.ShowsCompass = showsCompass;
		Ui.ShowsScale = showsScale;
		Ui.ShowsTraffic = showsTraffic;
	}

	void SetPins(
		IReadOnlyList<MapPin>? value)
	{
		if (ReferenceEquals(pins, value))
			return;

		if (hooked && pins is INotifyCollectionChanged old)
			old.CollectionChanged -= OnPinsChanged;

		pins = value ?? [];

		if (hooked && pins is INotifyCollectionChanged live)
			live.CollectionChanged += OnPinsChanged;

		ReloadPins();
	}

	void OnPinsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs args) =>
		ReloadPins();

	void ReloadPins()
	{
		if (!IsRealized)
			return;

		rootedHosts.Clear();

		if (added.Count > 0)
		{
			Ui.RemoveAnnotations([.. added]);
			added.Clear();
		}

		foreach (MapPin pin in pins)
			added.Add(new(pin));

		if (added.Count > 0)
			Ui.AddAnnotations([.. added]);
	}

	void SetOverlays(
		IReadOnlyList<MapOverlay>? value)
	{
		if (ReferenceEquals(overlays, value))
			return;

		if (hooked && overlays is INotifyCollectionChanged old)
			old.CollectionChanged -= OnOverlaysChanged;

		overlays = value ?? [];

		if (hooked && overlays is INotifyCollectionChanged live)
			live.CollectionChanged += OnOverlaysChanged;

		ReloadOverlays();
	}

	void OnOverlaysChanged(
		object? sender,
		NotifyCollectionChangedEventArgs args) =>
		ReloadOverlays();

	void ReloadOverlays()
	{
		if (!IsRealized)
			return;

		if (nativeOverlays.Count > 0)
		{
			Ui.RemoveOverlays([.. nativeOverlays]);
			nativeOverlays.Clear();
			overlayModels.Clear();
		}

		foreach (MapOverlay overlay in overlays)
		{
			IMKOverlay native = ToNativeOverlay(overlay);
			nativeOverlays.Add(native);
			overlayModels[native] = overlay;
		}

		if (nativeOverlays.Count > 0)
			Ui.AddOverlays([.. nativeOverlays]);
	}

	void OnRegionChanged()
	{
		if (applyingRegion)
			return;

		Set(ref region, FromNative(Ui.Region), affectsMeasure: false);
		regionBinding?.PushToSource(region);
	}

	void OnPinSelected(
		MapPin pin)
	{
		if (PinCommand is ICommand command && command.CanExecute(pin))
			command.Execute(pin);
	}

	void Root(
		UIView host) =>
		rootedHosts.Add(host);

	MapOverlay? ModelFor(
		IMKOverlay overlay) =>
		overlayModels.GetValueOrDefault(overlay);


	private protected override UIView CreateNative()
	{
		MKMapView view = new(CGRect.Empty);

		peer = new(this);
		view.Delegate = peer;

		return view;
	}

	private protected override void ApplyProperties()
	{
		ApplyKind();
		ApplyInteractions();
		ApplyChrome();
		ApplyRegion();

		if (pins is INotifyCollectionChanged livePins)
			livePins.CollectionChanged += OnPinsChanged;

		if (overlays is INotifyCollectionChanged liveOverlays)
			liveOverlays.CollectionChanged += OnOverlaysChanged;

		hooked = true;

		ReloadPins();
		ReloadOverlays();
	}

	private protected override void OnUnrealized()
	{
		rootedHosts.Clear();

		if (hooked && pins is INotifyCollectionChanged livePins)
			livePins.CollectionChanged -= OnPinsChanged;

		if (hooked && overlays is INotifyCollectionChanged liveOverlays)
			liveOverlays.CollectionChanged -= OnOverlaysChanged;

		hooked = false;
	}


	protected override Size MeasureOverride(
		Size availableSize) =>
		new(Fill(availableSize.Width), Fill(availableSize.Height));


	/// <summary>
	/// Moves the map to a region.
	/// </summary>
	/// <param name="region">The extent to show.</param>
	/// <param name="animated">Whether the move animates.</param>
	public void SetRegion(
		MapRegion region,
		bool animated)
	{
		if (!IsRealized)
			return;

		applyingRegion = true;
		Ui.SetRegion(ToNative(region), animated);
		applyingRegion = false;
	}
}
