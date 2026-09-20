using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Collections;

internal abstract class TemplateEntry(
	string title,
	string symbol) : ObservableObject
{
	public string Title { get; } = title;

	public string Symbol { get; } = symbol;
}

internal sealed class TemplateNavigationEntry(
	string title,
	string symbol,
	string detail) : TemplateEntry(title, symbol)
{
	public string Detail { get; } = detail;
}

internal sealed partial class TemplateToggleEntry : TemplateEntry
{
	public TemplateToggleEntry(
		string title,
		string symbol,
		bool value) : base(title, symbol) =>
		Value = value;


	[ObservableProperty]
	public partial bool Value { get; set; }
}

internal sealed class TemplateActionEntry(
	string title,
	string symbol) : TemplateEntry(title, symbol);

internal sealed partial class TemplateSelectEntry : TemplateEntry
{
	public TemplateSelectEntry(
		string title,
		string symbol,
		bool isSelected) : base(title, symbol) =>
		IsSelected = isSelected;


	[ObservableProperty]
	public partial bool IsSelected { get; set; }
}

internal sealed partial class ItemTemplatesViewModel : ShowcaseViewModel
{
	public TemplateEntry[] Items { get; } =
	[
		new TemplateNavigationEntry("Account", "person.crop.circle", "Personal"),
		new TemplateSelectEntry("Automatic", "circle.lefthalf.filled", true),
		new TemplateSelectEntry("Light", "sun.max", false),
		new TemplateSelectEntry("Dark", "moon", false),
		new TemplateToggleEntry("Notifications", "bell", true),
		new TemplateActionEntry("Clear cached data", "trash")
	];

	[ObservableProperty]
	public partial string Status { get; set; } = "The checkmark follows the selected appearance.";

	public IReadOnlyList<Span> TemplatesCode { get; } =
	[
		new(
			"""
			abstract class Setting(string title) : ObservableObject
			{
				public string Title { get; } = title;
			}

			sealed class NavigationSetting(
				string title,
				string detail) : Setting(title)
			{
				public string Detail { get; } = detail;
			}

			sealed partial class SelectSetting : Setting
			{
				public SelectSetting(string title, bool isSelected) : base(title) =>
					IsSelected = isSelected;

				[ObservableProperty]
				public partial bool IsSelected { get; set; }
			}

			CollectionView<Setting> settings = new()
			{
				ItemsSource = viewModel.Items,
				ItemTemplateSelector = new ItemTemplateSelector<Setting>()
					.Add<NavigationSetting>(
						static () => new NavigationSettingCell())
					.Add<SelectSetting>(
						static () => new SelectSettingCell()),
				Layout = CollectionLayout.List(grouped: true)
			};

			sealed class NavigationSettingCell : ItemView<NavigationSetting>
			{
				public NavigationSettingCell()
				{
					Background = Colors.SecondaryGroupedBackground;

					Accessories.Add(new LabelAccessory
					{
						Text = Bind(item => item.Detail),
						Tint = Colors.SecondaryLabel
					});

					Accessories.Add(new DisclosureAccessory());

					Content = new Label { Text = Bind(item => item.Title) };
				}
			}

			sealed class SelectSettingCell : ItemView<SelectSetting>
			{
				public SelectSettingCell()
				{
					Background = Colors.SecondaryGroupedBackground;

					Accessories.Add(new CheckmarkAccessory
					{
						IsVisible = Bind(item => item.IsSelected),
						Tint = Colors.Teal
					});

					Content = new Label { Text = Bind(item => item.Title) };
				}
			}
			""")
	];


	[RelayCommand]
	void Activate(
		TemplateEntry item)
	{
		switch (item)
		{
			case TemplateSelectEntry select:
				foreach (TemplateEntry entry in Items)
				{
					if (entry is TemplateSelectEntry candidate)
						candidate.IsSelected = ReferenceEquals(candidate, select);
				}

				Status = $"Selected the {select.Title} appearance.";
				break;

			case TemplateNavigationEntry:
				Status = $"Opened {item.Title}";
				break;

			case TemplateActionEntry:
				Status = $"Ran {item.Title}";
				break;
		}
	}
}
