using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Collections;

internal sealed record ContactEntry(
	string Initials,
	string Name);

internal sealed partial class CollectionInteractionsViewModel : ShowcaseViewModel
{
	static readonly string[] Names =
	[
		"Alex Morgan",
		"Avery Singh",
		"Cameron Hall",
		"Casey Lee",
		"Devon Clarke",
		"Drew Parker",
		"Emery Jones",
		"Jamie Patel",
		"Jordan Kim",
		"Morgan Reed",
		"Quinn Foster",
		"Riley Chen",
		"Robin Shah",
		"Sam Rivera",
		"Skyler Young",
		"Taylor Brooks"
	];

	int newContact = 1;

	public ObservableCollection<ContactEntry> Items { get; } =
	[
		.. Names.Select(name => new ContactEntry(Initials(name), name))
	];

	[ObservableProperty]
	bool isEditing;

	[ObservableProperty]
	bool isRefreshing;

	public IReadOnlyList<Span> InteractionsCode { get; } =
		Code(
			"""
			CollectionView<ContactEntry> contacts = new()
			{
				ItemsSource = viewModel.Items,
				ItemTemplate = static () => new ContactCell(),
				Layout = CollectionLayout.List(),
				HighlightsSelection = false,
				SeparatorInsets = new Thickness(66, 0, 0, 0),
				RefreshCommand = viewModel.RefreshCommand,
				IsRefreshing = Bind(
					model => model.IsRefreshing,
					static (model, value) => model.IsRefreshing = value),
				ReorderCommand = viewModel.ReorderCommand,
				IsEditing = Bind(
					model => model.IsEditing,
					static (model, value) => model.IsEditing = value),
				SwipeActions =
				{
					new SwipeAction
					{
						Text = "Delete",
						IsDestructive = true,
						Command = viewModel.DeleteCommand
					}
				},
				ItemContextMenu =
				{
					new MenuAction
					{
						Text = "Move to top",
						Command = viewModel.MoveToTopCommand
					},
					new MenuAction
					{
						Text = "Delete",
						IsDestructive = true,
						Command = viewModel.DeleteCommand
					}
				},
				EmptyView = new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Text = "No contacts",
					TextStyle = TextStyle.Headline,
					TextColor = Colors.SecondaryLabel
				}
			};

			ToolbarItem edit = new()
			{
				Text = "Edit",
				Command = viewModel.ToggleEditingCommand
			};

			viewModel.PropertyChanged += (_, args) =>
			{
				if (args.PropertyName == nameof(viewModel.IsEditing))
					edit.Text = viewModel.IsEditing ? "Done" : "Edit";
			};

			ToolbarItems.Add(edit);

			sealed record ContactEntry(string Initials, string Name);

			sealed class ContactCell : ItemView<ContactEntry>
			{
				public ContactCell() =>
					Content = new Grid
					{
						Height = 60,
						Padding = new Thickness(16, 0),
						ColumnSpacing = 12,
						Columns =
						{
							GridLength.Auto,
							GridLength.Star
						},
						Children =
						{
							new Border
							{
								Width = 38,
								Height = 38,
								VerticalAlignment = VerticalAlignment.Center,
								Background = Colors.Teal.WithAlpha(0.16),
								CornerRadius = 19,
								Child = new Label
								{
									HorizontalAlignment = HorizontalAlignment.Center,
									VerticalAlignment = VerticalAlignment.Center,
									Text = Bind(item => item.Initials),
									TextStyle = TextStyle.Footnote,
									FontWeight = FontWeight.Semibold,
									TextColor = Colors.Teal
								}
							},
							new Label
							{
								VerticalAlignment = VerticalAlignment.Center,
								Text = Bind(item => item.Name),
								TextStyle = TextStyle.Body
							}.Column(1)
						}
					};
			}

			static readonly string[] Names =
			[
				"Alex Morgan",
				"Avery Singh",
				"Cameron Hall",
				"Casey Lee",
				"Devon Clarke",
				"Drew Parker",
				"Emery Jones",
				"Jamie Patel",
				"Jordan Kim",
				"Morgan Reed",
				"Quinn Foster",
				"Riley Chen",
				"Robin Shah",
				"Sam Rivera",
				"Skyler Young",
				"Taylor Brooks"
			];

			int newContact = 1;

			ObservableCollection<ContactEntry> Items { get; } =
			[
				.. Names.Select(name => new ContactEntry(Initials(name), name))
			];

			[ObservableProperty]
			bool isEditing;

			[ObservableProperty]
			bool isRefreshing;

			[RelayCommand]
			void ToggleEditing() =>
				IsEditing = !IsEditing;

			[RelayCommand]
			async Task Refresh()
			{
				try
				{
					await Task.Delay(700);

					string suffix = newContact == 1 ? "" : $" {newContact}";
					newContact++;
					Items.Insert(0, new("NC", $"New Contact{suffix}"));
					Haptics.Selection();
				}
				finally
				{
					IsRefreshing = false;
				}
			}

			[RelayCommand]
			void Delete(ContactEntry item)
			{
				Items.Remove(item);
				Haptics.Notify(HapticsNotification.Success);
			}

			[RelayCommand]
			void MoveToTop(ContactEntry item)
			{
				int index = Items.IndexOf(item);

				if (index > 0)
					Items.Move(index, 0);
			}

			[RelayCommand]
			static void Reorder(ItemMove<ContactEntry> move) =>
				Haptics.Impact(HapticStyle.Light);

			static string Initials(string name) =>
				string.Concat(
					name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
						.Take(2)
						.Select(part => char.ToUpperInvariant(part[0])));
			""");


	[RelayCommand]
	void ToggleEditing() =>
		IsEditing = !IsEditing;

	[RelayCommand]
	async Task Refresh()
	{
		try
		{
			await Task.Delay(700);

			string suffix = newContact == 1 ? "" : $" {newContact}";
			newContact++;
			Items.Insert(0, new("NC", $"New Contact{suffix}"));
			Haptics.Selection();
		}
		finally
		{
			IsRefreshing = false;
		}
	}

	[RelayCommand]
	void Delete(
		ContactEntry item)
	{
		Items.Remove(item);
		Haptics.Notify(HapticsNotification.Success);
	}

	[RelayCommand]
	void MoveToTop(
		ContactEntry item)
	{
		int index = Items.IndexOf(item);

		if (index > 0)
			Items.Move(index, 0);
	}

	[RelayCommand]
	static void Reorder(
		ItemMove<ContactEntry> move) =>
		Haptics.Impact(HapticStyle.Light);

	static string Initials(
		string name) =>
		string.Concat(
			name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.Take(2)
				.Select(part => char.ToUpperInvariant(part[0])));

	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
