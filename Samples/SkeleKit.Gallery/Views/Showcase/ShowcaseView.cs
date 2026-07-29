using System.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;
using SkeleKit.Gallery.Views.Shared;

namespace SkeleKit.Gallery.Views.Showcase;

internal abstract class ShowcaseView<TViewModel> : TintView<TViewModel>
	where TViewModel : ShowcaseViewModel
{
	readonly ToolbarItem appearanceItem;


	protected ShowcaseView(
		TViewModel viewModel,
		string title,
		Color tint) : base(viewModel, tint)
	{
		Title = title;
		TitleStyle = TitleStyle.Large;
		BackgroundStyle = PageBackground.Grouped;
		BackButtonStyle = BackButtonStyle.Generic;

		appearanceItem = new()
		{
			Command = viewModel.CycleAppearanceCommand
		};
		UpdateAppearanceItem();
		ToolbarItems.Add(appearanceItem);

		Sections = new()
		{
			Padding = new(16, 12, 16, 32),
			Spacing = 28
		};

		Content = new ScrollView
		{
			Content = Sections
		};
	}


	protected StackPanel Sections { get; }


	protected void AddShowcase(
		string title,
		string summary,
		View preview,
		View code)
	{
		Sections.Children.Add(new StackPanel
		{
			Spacing = 10,

			Children =
			{
				new StackPanel
				{
					Padding = new(4, 0),
					Spacing = 3,

					Children =
					{
						new Label
						{
							Text = title,
							TextStyle = TextStyle.Headline,
							FontWeight = FontWeight.Semibold
						},

						new Label
						{
							Text = summary,
							TextStyle = TextStyle.Subheadline,
							TextColor = Colors.SecondaryLabel,
							MaxLines = 3
						}
					}
				},

				new ShowcaseBox(preview, code)
			}
		});
	}


	protected override void OnLoaded()
	{
		base.OnLoaded();
		ViewModel.PropertyChanged += OnViewModelPropertyChanged;
	}

	protected override void OnUnloaded()
	{
		ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
		base.OnUnloaded();
	}

	protected override void OnAppearing()
	{
		if (SkeleApplication.Current is SkeleApplication app)
			ViewModel.Appearance = app.Appearance;

		UpdateAppearanceItem();
		base.OnAppearing();
	}


	void OnViewModelPropertyChanged(
		object? sender,
		PropertyChangedEventArgs args)
	{
		if (args.PropertyName == nameof(ShowcaseViewModel.Appearance))
			UpdateAppearanceItem();
	}

	void UpdateAppearanceItem() =>
		appearanceItem.Icon = ViewModel.Appearance switch
		{
			Appearance.Dark => "moon.fill",
			Appearance.Light => "sun.max.fill",
			_ => "circle.lefthalf.filled"
		};
}
