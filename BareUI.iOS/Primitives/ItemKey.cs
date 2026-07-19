using System.Runtime.CompilerServices;
using ObjCRuntime;

namespace BareUI;

internal sealed class ItemKey : NSObject
{
	public ItemKey(
		object item)
	{
		Item = item;
	}

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
