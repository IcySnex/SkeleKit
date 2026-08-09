using System.Runtime.CompilerServices;
using SkeleKit.Gallery.ViewModels.Showcase;
using SkeleKit.Gallery.Views.Abstract;

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
			Command = Command.From(CycleAppearance)
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


	protected void AddCodePage(
		string title,
		Func<IReadOnlyList<Span>> source) =>
		ToolbarItems.Add(new ToolbarItem
		{
			Icon = "chevron.left.forwardslash.chevron.right",
			Command = Command.From(() =>
				_ = Navigator.PushViewAsync(new ShowcaseCodeView(title, source(), Tint ?? Colors.Label)))
		});

	protected static View Code(
		Func<TViewModel, IReadOnlyList<Span>> getter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		ShowcaseBox.Code(Bind(getter, CSharpSyntax.Highlight, path));

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

	protected static View PreviewWithSettings(
		View canvas,
		params View[] settings)
	{
		StackPanel configuration = new()
		{
			Padding = 16,
			Spacing = 16
		};

		foreach (View setting in settings)
			configuration.Children.Add(setting);

		return new StackPanel
		{
			Children =
			{
				canvas,
				new Divider(),
				configuration
			}
		};
	}

	protected static View SettingRow(
		string title,
		View control) =>
		new Grid
		{
			ColumnSpacing = 12,
			MinHeight = 34,

			Columns =
			{
				GridLength.Star,
				GridLength.Auto
			},

			Children =
			{
				SettingLabel(title),

				control.Column(1)
			}
		};

	protected static View LabeledControl(
		string title,
		View control) =>
		new StackPanel
		{
			Spacing = 8,

			Children =
			{
				SettingLabel(title),

				control
			}
		};

	protected static View LabeledSlider(
		string title,
		BindingExpression<string?> value,
		Slider slider) =>
		new StackPanel
		{
			Spacing = 8,

			Children =
			{
				new Grid
				{
					Columns =
					{
						GridLength.Star,
						GridLength.Auto
					},

					Children =
					{
						SettingLabel(title),

						new Label
						{
							VerticalAlignment = VerticalAlignment.Center,
							Text = value,
							TextStyle = TextStyle.Subheadline,
							TextColor = Colors.SecondaryLabel
						}.Column(1)
					}
				},

				slider
			}
		};


	protected override void OnAppearing()
	{
		UpdateAppearanceItem();
		base.OnAppearing();
	}


	void CycleAppearance()
	{
		if (SkeleApplication.Current is not SkeleApplication app)
			return;

		app.Appearance = app.Appearance switch
		{
			Appearance.System => Appearance.Dark,
			Appearance.Dark => Appearance.Light,
			_ => Appearance.System
		};
		UpdateAppearanceItem();
	}

	void UpdateAppearanceItem() =>
		appearanceItem.Icon = SkeleApplication.Current?.Appearance switch
		{
			Appearance.Dark => "moon.fill",
			Appearance.Light => "sun.max.fill",
			_ => "circle.lefthalf.filled"
		};

	static Label SettingLabel(
		string title) =>
		new()
		{
			VerticalAlignment = VerticalAlignment.Center,
			Text = title,
			TextStyle = TextStyle.Subheadline,
			FontWeight = FontWeight.Medium
		};
}
