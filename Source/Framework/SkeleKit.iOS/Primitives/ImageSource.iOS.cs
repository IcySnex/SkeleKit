namespace SkeleKit;

public readonly partial struct ImageSource
{
	internal UIImage? ResolveLocal(
		double symbolValue = double.NaN,
		double defaultSymbolSize = double.NaN,
		FontWeight? defaultSymbolWeight = null)
	{
		if (Kind is ImageSourceKind.Url)
			throw new NotSupportedException("Remote image URLs are supported by Image.Source, not control icons.");

		if (Kind is ImageSourceKind.Data)
			return Bytes is byte[] bytes
				? UIImage.LoadFromData(NSData.FromArray(bytes))
				: null;

		if (string.IsNullOrEmpty(Value))
			return null;

		UIImageSymbolConfiguration? symbolConfiguration = CreateSymbolConfiguration(
			double.IsNaN(SymbolSize) ? defaultSymbolSize : SymbolSize,
			SymbolWeight ?? defaultSymbolWeight,
			SymbolScale,
			SymbolColors,
			PrefersMulticolor);
		string value = Value;
		UIImage? Bundle() => UIImage.FromBundle(value);
		UIImage? Symbol()
		{
			if (!double.IsNaN(symbolValue))
			{
				return UIImage.GetSystemImage(
					value,
					Math.Clamp(symbolValue, 0, 1),
					symbolConfiguration ?? UIImageSymbolConfiguration.UnspecifiedConfiguration);
			}

			return symbolConfiguration is null
				? UIImage.GetSystemImage(value)
				: UIImage.GetSystemImage(value, symbolConfiguration);
		}

		return Kind switch
		{
			ImageSourceKind.Symbol => Symbol(),
			ImageSourceKind.Bundle => Bundle(),
			_ => Symbol() ?? Bundle()
		};
	}

	internal UIImageSymbolConfiguration? CreateSymbolConfiguration(
		double defaultSymbolSize,
		FontWeight? defaultSymbolWeight) =>
		CreateSymbolConfiguration(
			double.IsNaN(SymbolSize) ? defaultSymbolSize : SymbolSize,
			SymbolWeight ?? defaultSymbolWeight,
			SymbolScale,
			SymbolColors,
			PrefersMulticolor);

	internal static UIImageSymbolConfiguration? CreateSymbolConfiguration(
		double size,
		FontWeight? weight,
		SymbolScale scale,
		IEnumerable<Color> colors,
		bool prefersMulticolor)
	{
		UIImageSymbolConfiguration? configuration = null;
		Color[] renderingColors = [.. colors];

		void Add(UIImageSymbolConfiguration next) =>
			configuration = configuration is null
				? next
				: (UIImageSymbolConfiguration)configuration.GetConfiguration(next);

		if (!double.IsNaN(size))
			Add(UIImageSymbolConfiguration.Create((nfloat)size));

		if (weight is FontWeight symbolWeight)
			Add(UIImageSymbolConfiguration.Create(Weight(symbolWeight)));

		if (scale is not SymbolScale.Default)
			Add(UIImageSymbolConfiguration.Create(Scale(scale)));

		switch (renderingColors.Length)
		{
			case 1:
				Add(UIImageSymbolConfiguration.Create(renderingColors[0].ToUIColor()));
				break;
			case > 1:
				Add(UIImageSymbolConfiguration.Create([.. renderingColors.Select(color => color.ToUIColor())]));
				break;
		}

		if (prefersMulticolor)
			Add(UIImageSymbolConfiguration.ConfigurationPreferringMulticolor);

		return configuration;
	}

	static UIImageSymbolScale Scale(
		SymbolScale scale) =>
		scale switch
		{
			SymbolScale.Small => UIImageSymbolScale.Small,
			SymbolScale.Large => UIImageSymbolScale.Large,
			_ => UIImageSymbolScale.Medium
		};

	static UIImageSymbolWeight Weight(
		FontWeight weight) =>
		weight switch
		{
			FontWeight.UltraLight => UIImageSymbolWeight.UltraLight,
			FontWeight.Thin => UIImageSymbolWeight.Thin,
			FontWeight.Light => UIImageSymbolWeight.Light,
			FontWeight.Medium => UIImageSymbolWeight.Medium,
			FontWeight.Semibold => UIImageSymbolWeight.Semibold,
			FontWeight.Bold => UIImageSymbolWeight.Bold,
			FontWeight.Heavy => UIImageSymbolWeight.Heavy,
			FontWeight.Black => UIImageSymbolWeight.Black,
			_ => UIImageSymbolWeight.Regular
		};
}
