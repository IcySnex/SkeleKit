using System.ComponentModel;

namespace BareUI;

abstract class BindingBase
{
	public abstract void Attach(
		object? source);

	public abstract void Detach();
}

// live binding: watches the source, pushes values into the control, and (two-way) back out
sealed class Binding<T>(
	BindingExpression<T> expression,
	Action<T?> apply) : BindingBase
{
	readonly List<INotifyPropertyChanged> subscriptions = [];

	object? source;

	public UpdateTrigger Trigger =>
		expression.Trigger;

	public override void Attach(
		object? source)
	{
		Detach();

		this.source = source;
		if (source is null)
			return;

		if (expression.Mode is not BindingMode.OneTime)
			Subscribe(source);

		if (expression.Mode is not BindingMode.OneWayToSource)
			apply(expression.Getter(source));
	}

	public override void Detach()
	{
		foreach (INotifyPropertyChanged subscription in subscriptions)
			subscription.PropertyChanged -= OnSourcePropertyChanged;

		subscriptions.Clear();
		source = null;
	}

	/// <summary>
	/// Writes the control's current value back to the source.
	/// </summary>
	public void PushToSource(
		T? value)
	{
		if (expression.Mode is not (BindingMode.TwoWay or BindingMode.OneWayToSource))
			return;

		if (expression.Setter is { } setter && source is { } current)
			setter(current, value);
	}

	void Subscribe(
		object source)
	{
		object? current = source;

		foreach (BindingSegment segment in expression.Segments)
		{
			if (current is INotifyPropertyChanged notifier)
			{
				notifier.PropertyChanged += OnSourcePropertyChanged;
				subscriptions.Add(notifier);
			}

			if (segment.Step is null || current is null)
				break;

			current = segment.Step(current);
		}
	}

	void OnSourcePropertyChanged(
		object? sender,
		PropertyChangedEventArgs e)
	{
		if (source is null)
			return;

		if (e.PropertyName is { Length: > 0 } name && !Watches(name))
			return;

		// an intermediate may have been replaced, so rebuild the subscriptions too
		object? current = source;
		Attach(current);
	}

	bool Watches(
		string name)
	{
		foreach (BindingSegment segment in expression.Segments)
			if (segment.Name == name)
				return true;

		return false;
	}
}
