using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SkeleKit.Tests.Binding;

class Notifier : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	protected void Raise(
		[CallerMemberName] string? name = null) =>
		PropertyChanged?.Invoke(this, new(name));
}

class MovieViewModel : Notifier
{
	string title = "";
	public string Title
	{
		get => title;
		set { title = value; Raise(); }
	}

	int minutes;
	public int Minutes
	{
		get => minutes;
		set { minutes = value; Raise(); }
	}

	bool enabled;
	public bool Enabled
	{
		get => enabled;
		set { enabled = value; Raise(); }
	}

	string query = "";
	public string Query
	{
		get => query;
		set { query = value; Raise(); }
	}

	int searchScope;
	public int SearchScope
	{
		get => searchScope;
		set { searchScope = value; Raise(); }
	}

	Movie? movie;
	public Movie? Movie
	{
		get => movie;
		set { movie = value; Raise(); }
	}
}

class Movie : Notifier
{
	string name = "";
	public string Name
	{
		get => name;
		set { name = value; Raise(); }
	}

	Director? director;
	public Director? Director
	{
		get => director;
		set { director = value; Raise(); }
	}
}

class Director : Notifier
{
	string name = "";
	public string Name
	{
		get => name;
		set { name = value; Raise(); }
	}
}

// a leaf view with one bindable property, standing in for a real control (controls are iOS-only)
class StubBound : View
{
	string? text;
	Binding<string?>? textBinding;

	public Bindable<string?> Text
	{
		get => text;
		set => textBinding = Register(textBinding, value, ApplyText);
	}

	public string? Current =>
		text;

	void ApplyText(
		string? value) =>
		Set(ref text, value, affectsMeasure: false);

	// what a control's native change event would do
	public void SimulateEdit(
		string? value)
	{
		Set(ref text, value, affectsMeasure: false);
		textBinding?.PushToSource(value);
	}

	protected override Size MeasureOverride(
		Size availableSize) =>
		Size.Zero;
}

class StubDoubleBound : View
{
	double value;
	Binding<double>? valueBinding;

	public Bindable<double> Value
	{
		get => value;
		set => valueBinding = Register(valueBinding, value, ApplyValue);
	}

	public double Current =>
		value;

	void ApplyValue(
		double value) =>
		Set(ref this.value, value, affectsMeasure: false);

	public void SimulateEdit(
		double value)
	{
		Set(ref this.value, value, affectsMeasure: false);
		valueBinding?.PushToSource(value);
	}

	protected override Size MeasureOverride(
		Size availableSize) =>
		Size.Zero;
}

class StubPage : ContentView;
