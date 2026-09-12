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

internal sealed partial class ItemTemplatesViewModel : ShowcaseViewModel
{
	public TemplateEntry[] Items { get; } =
	[
		new TemplateNavigationEntry("Account", "person.crop.circle", "Personal"),
		new TemplateToggleEntry("Notifications", "bell", true),
		new TemplateActionEntry("Clear cached data", "trash")
	];

	[ObservableProperty]
	public partial string Status { get; set; } = "Select a navigation or action row.";

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

			sealed partial class ToggleSetting : Setting
			{
				public ToggleSetting(string title, bool value) : base(title) =>
					Value = value;

				[ObservableProperty]
				public partial bool Value { get; set; }
			}

			CollectionView<Setting> settings = new()
			{
				ItemsSource = viewModel.Items,
				ItemTemplateSelector = new ItemTemplateSelector<Setting>()
					.Add<NavigationSetting>(
						static () => new NavigationSettingCell())
					.Add<ToggleSetting>(
						static () => new ToggleSettingCell()),
				Layout = CollectionLayout.List(grouped: true)
			};

			sealed class ToggleSettingCell : ItemView<ToggleSetting>
			{
				public ToggleSettingCell()
				{
					HighlightBackground = null;

					Content = new Switch
					{
						IsOn = Bind(item => item.Value).TwoWay(
							static (item, value) => item.Value = value)
					};
				}
			}
			""")
	];


	[RelayCommand]
	void Activate(
		TemplateEntry item)
	{
		Status = item switch
		{
			TemplateNavigationEntry => $"Opened {item.Title}",
			TemplateActionEntry => $"Ran {item.Title}",
			_ => Status
		};
	}
}
