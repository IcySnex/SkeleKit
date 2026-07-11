using System.Collections;

namespace BareUI;

/// <summary>
/// The children of a <see cref="Panel"/>, raising a change callback so the panel can relayout.
/// </summary>
public sealed class ViewCollection : IEnumerable<View>
{
	readonly List<View> items = [];
	readonly View? owner;
	readonly Action? changed;

	internal ViewCollection(
		View? owner = null,
		Action? changed = null)
	{
		this.owner = owner;
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
		view.SetParent(owner);

		changed?.Invoke();
	}

	public bool Remove(
		View view)
	{
		if (!items.Remove(view))
			return false;

		view.SetParent(null);
		changed?.Invoke();

		return true;
	}

	public void Clear()
	{
		if (items.Count == 0)
			return;

		foreach (View item in items)
			item.SetParent(null);

		items.Clear();
		changed?.Invoke();
	}


	public IEnumerator<View> GetEnumerator() =>
		items.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}
