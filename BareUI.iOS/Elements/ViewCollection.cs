using System.Collections;

namespace BareUI;

/// <summary>
/// The children of a <see cref="Panel"/>, raising a change callback so the panel can relayout.
/// </summary>
public sealed class ViewCollection : IEnumerable<View>
{
	readonly List<View> items = [];
	readonly Action? changed;

	internal ViewCollection(
		Action? changed = null)
	{
		this.changed = changed;
	}


	public int Count =>
		items.Count;

	public View this[
		int index] =>
		items[index];


	public void Add(
		View view)
	{
		ArgumentNullException.ThrowIfNull(view);

		items.Add(view);
		changed?.Invoke();
	}

	public bool Remove(
		View view)
	{
		bool removed = items.Remove(view);
		if (removed)
			changed?.Invoke();

		return removed;
	}

	public void Clear()
	{
		if (items.Count == 0)
			return;

		items.Clear();
		changed?.Invoke();
	}


	public IEnumerator<View> GetEnumerator() =>
		items.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}
