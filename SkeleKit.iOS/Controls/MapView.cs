using System.Collections.Specialized;
using System.Windows.Input;
using CoreLocation;
using MapKit;
using ObjCRuntime;

namespace SkeleKit;

/// <summary>
/// Embeds an interactive map in the tree, backed by a UIKit map view.
/// </summary>
/// <remarks>
/// Shows a two-way <see cref="Region"/>, drops markers from <see cref="Pins"/>, and reports taps through <see cref="SelectionCommand"/> and <see cref="PinSelected"/>.<br/>
/// Give it a bounded slot (a fill row, an explicit height), since map content has no intrinsic size to measure against.<br/>
/// <see cref="ShowsUserLocation"/> needs <c>NSLocationWhenInUseUsageDescription</c> in the app plist.
/// </remarks>
public class MapView : Control
{
	sealed class PinAnnotation : MKPointAnnotation
	{
		public PinAnnotation(
			MapPin pin) : base(new CLLocationCoordinate2D(pin.Coordinate.Latitude, pin.Coordinate.Longitude), pin.Title!, pin.Subtitle!)
		{
			Pin = pin;
		}

		// ReSharper disable once UnusedMember.Local
		public PinAnnotation(
			NativeHandle handle) : base(handle)
		{ }


		public MapPin Pin { get; } = null!;
	}


	sealed class MapPeer : MKMapViewDelegate
	{
		const string MarkerId = "skele.pin";

		readonly MapView? owner;

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
			if (annotation is not PinAnnotation pin)
				return null;

			MKMarkerAnnotationView view = mapView.DequeueReusableAnnotation(MarkerId) as MKMarkerAnnotationView ?? new MKMarkerAnnotationView(annotation, MarkerId);

			view.Annotation = annotation;
			view.CanShowCallout = pin.Pin.Title is not null || pin.Pin.Subtitle is not null;
			view.MarkerTintColor = pin.Pin.Tint?.ToUIColor();
			view.GlyphImage = pin.Pin.Symbol is string symbol ? UIImage.GetSystemImage(symbol) : null;

			return view;
		}

		public override void DidChangeVisibleRegion(
			MKMapView mapView) =>
			owner?.OnRegionChanged();

		public override void DidSelectAnnotation(
			MKMapView mapView,
			IMKAnnotation annotation)
		{
			if (annotation is PinAnnotation pin)
				owner?.OnPinSelected(pin.Pin);
		}
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


	MapPeer? peer;
	readonly List<PinAnnotation> added = [];
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
	/// Invoked with the tapped pin.
	/// </summary>
	public ICommand? SelectionCommand { get; set; }

	/// <summary>
	/// Called with the pin the user tapped.
	/// </summary>
	public Action<MapPin>? PinSelected { get; set; }


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

		if (added.Count > 0)
		{
			Ui.RemoveAnnotations([.. added]);
			added.Clear();
		}

		foreach (MapPin pin in pins)
			added.Add(new PinAnnotation(pin));

		if (added.Count > 0)
			Ui.AddAnnotations([.. added]);
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
		if (SelectionCommand is ICommand command && command.CanExecute(pin))
			command.Execute(pin);

		PinSelected?.Invoke(pin);
	}


	static bool NearlyEquals(
		MKCoordinateRegion native,
		MapRegion region) =>
		Math.Abs(native.Center.Latitude - region.Center.Latitude) < 1e-6 &&
		Math.Abs(native.Center.Longitude - region.Center.Longitude) < 1e-6 &&
		Math.Abs(native.Span.LatitudeDelta - region.LatitudeSpan) < 1e-6 &&
		Math.Abs(native.Span.LongitudeDelta - region.LongitudeSpan) < 1e-6;

	static MapRegion FromNative(
		MKCoordinateRegion native) =>
		new(new Coordinate(native.Center.Latitude, native.Center.Longitude), native.Span.LatitudeDelta, native.Span.LongitudeDelta);

	static MKCoordinateRegion ToNative(
		MapRegion region) =>
		new(new CLLocationCoordinate2D(region.Center.Latitude, region.Center.Longitude), new MKCoordinateSpan(region.LatitudeSpan, region.LongitudeSpan));


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

		if (pins is INotifyCollectionChanged live)
		{
			live.CollectionChanged += OnPinsChanged;
			hooked = true;
		}

		ReloadPins();
	}

	private protected override void OnUnrealized()
	{
		if (hooked && pins is INotifyCollectionChanged live)
			live.CollectionChanged -= OnPinsChanged;

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
