using System.ComponentModel;

namespace SkeleKit;

internal abstract class BindingBase
{
	public abstract void Attach(
		object? source);

	public abstract void Detach();
}

internal sealed class Binding<T>(
	BindingExpression<T> expression,
	Action<T?> apply) : BindingBase
{
	readonly List<INotifyPropertyChanged> subscriptions = [];

	object? source;


	public UpdateTrigger Trigger => expression.Trigger;


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

		if (e.PropertyName is string name && name.Length > 0 && !Watches(name))
			return;

		MainThread.Post(Refresh);
	}

	void Refresh()
	{
		if (source is not null)
			Attach(source);
	}

	bool Watches(
		string name)
	{
		foreach (BindingSegment segment in expression.Segments)
		{
			if (segment.Name == name)
				return true;
		}

		return false;
	}


	public override void Attach(
		object? source)
	{
		Detach();

		if (expression.Mode is BindingMode.TwoWay or BindingMode.OneWayToSource
			&& expression.Setter is null)
			throw new InvalidOperationException("A writable binding needs a source setter.");

		this.source = source;
		if (source is null)
			return;

		if (expression.Mode is not (BindingMode.OneTime or BindingMode.OneWayToSource))
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

	public void PushToSource(
		T? value)
	{
		if (expression.Mode is not (BindingMode.TwoWay or BindingMode.OneWayToSource))
			return;

		if (expression.Setter is not null && source is not null)
			expression.Setter(source, value);
	}
}
