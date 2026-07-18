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


	/// <summary>
	/// The number of children.
	/// </summary>
	public int Count =>
		items.Count;

	/// <summary>
	/// The child at <paramref name="index"/>.
	/// </summary>
	/// <param name="index">The zero-based index.</param>
	/// <returns>The child at that index.</returns>
	public View this[
		int index] =>
		items[index];


	/// <summary>
	/// Adds a child to the panel.
	/// </summary>
	/// <param name="view">The child to add.</param>
	public void Add(
		View view)
	{
		ArgumentNullException.ThrowIfNull(view);

		items.Add(view);
		view.SetParent(owner);

		changed?.Invoke();
	}

	/// <summary>
	/// Removes a child from the panel.
	/// </summary>
	/// <param name="view">The child to remove.</param>
	/// <returns><c>true</c> if the child was present and removed.</returns>
	public bool Remove(
		View view)
	{
		if (!items.Remove(view))
			return false;

		view.SetParent(null);
		changed?.Invoke();

		return true;
	}

	/// <summary>
	/// Removes all children.
	/// </summary>
	public void Clear()
	{
		if (items.Count == 0)
			return;

		foreach (View item in items)
			item.SetParent(null);

		items.Clear();
		changed?.Invoke();
	}


	/// <summary>
	/// Returns an enumerator over the children.
	/// </summary>
	/// <returns>An enumerator over the children.</returns>
	public IEnumerator<View> GetEnumerator() =>
		items.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}
