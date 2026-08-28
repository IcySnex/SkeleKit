namespace SkeleKit;

public readonly partial struct ImageSource
{
	internal UIImage? ResolveLocal(
		UIImageSymbolConfiguration? symbolConfiguration = null)
	{
		if (Kind is ImageSourceKind.Url)
			throw new NotSupportedException("Remote image URLs are supported by Image.Source, not control icons.");

		if (Kind is ImageSourceKind.Data)
			return Bytes is byte[] bytes
				? UIImage.LoadFromData(NSData.FromArray(bytes))
				: null;

		if (string.IsNullOrEmpty(Value))
			return null;

		string value = Value;
		UIImage? Bundle() => UIImage.FromBundle(value);
		UIImage? Symbol() =>
			symbolConfiguration is null
				? UIImage.GetSystemImage(value)
				: UIImage.GetSystemImage(value, symbolConfiguration);

		return Kind switch
		{
			ImageSourceKind.Symbol => Symbol(),
			ImageSourceKind.Bundle => Bundle(),
			_ => Bundle() ?? Symbol()
		};
	}
}
