using System.Runtime.CompilerServices;
using Foundation;
using ObjCRuntime;

namespace BareUI;

/// <summary>
/// The identity of one item for the diffable data source. Identity is the object itself, never its value: two equal records are still two rows.
/// </summary>
sealed class ItemKey : NSObject
{
	public ItemKey(
		object item)
	{
		Item = item;
	}

	// marshaller needs this; the key cache keeps the managed ref so it stays unused
	public ItemKey(
		NativeHandle handle) : base(handle)
	{ }

	public object? Item { get; }

	public override bool Equals(
		object? other) =>
		other is ItemKey key && ReferenceEquals(key.Item, Item);

	public override int GetHashCode() =>
		Item is null ? 0 : RuntimeHelpers.GetHashCode(Item);
}
