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
	/// Gets the total number of element nodes contained inside this tier layout collection.
	/// </summary>
	public int Count =>
		items.Count;

	/// <summary>
	/// Accesses a managed subview node at the targeted indexed location context.
	/// </summary>
	/// <param name="index">The zero-based position index to reference.</param>
	/// <returns>The allocated layout view element.</returns>
	public View this[
		int index] =>
		items[index];


	/// <summary>
	/// Appends an element container tracking node onto this hierarchy chain layout.
	/// </summary>
	/// <param name="view">The active view container asset reference to bind.</param>
	public void Add(
		View view)
	{
		ArgumentNullException.ThrowIfNull(view);

		items.Add(view);
		view.SetParent(owner);

		changed?.Invoke();
	}

	/// <summary>
	/// Detaches a managed framework element block clean out of the immediate visual hierarchy sequence.
	/// </summary>
	/// <param name="view">The target active tree layout node component to locate and remove.</param>
	/// <returns><c>true</c> if the element was found and detached successfully; otherwise, <c>false</c>.</returns>
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
	/// Disconnects and resets every structural branch view currently tracked by this container layout context.
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
	/// Returns an iterator loop tracker structured to sweep across the layout views context collection safely.
	/// </summary>
	/// <returns>An enumerator for processing child view nodes sequentially.</returns>
	public IEnumerator<View> GetEnumerator() =>
		items.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}
