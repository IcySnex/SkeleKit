using System.Windows.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Platform;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Platform;

[Page]
internal sealed class SystemPickingView : ShowcaseView<SystemPickingViewModel>
{
	public SystemPickingView(
		SystemPickingViewModel viewModel) : base(viewModel, "System Picking", Colors.Mint)
	{
		AddImagesShowcase(viewModel);
		AddFileShowcase(viewModel);
	}


	void AddImagesShowcase(
		SystemPickingViewModel viewModel)
	{
		Picker<ShowcaseOption<int>> limit = new()
		{
			MinWidth = 140,
			ItemsSource = viewModel.ImageLimits,
			SelectedItem = Bind(vm => vm.SelectedImageLimit)
				.TwoWay((vm, val) => vm.SelectedImageLimit = val!)
		};

		AddShowcase(
			"Images",
			"Pick one or several images from the system photo library and preview the first result.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 10,

						Children =
						{
							new Image
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Width = 140,
								Height = 100,
								Source = Bind(vm => vm.ImagePreview),
								Stretch = Stretch.UniformToFill,
								Background = Colors.SecondaryBackground,
								CornerRadius = 14
							},

							PickButton(
								"Pick images",
								"photo.on.rectangle.angled",
								viewModel.PickImagesCommand),

							ResultLabel(Bind(vm => vm.ImagesResult))
						}
					},
					220),
				SettingRow("Selection limit", limit)),
			Code(vm => vm.ImagesCode));
	}

	void AddFileShowcase(
		SystemPickingViewModel viewModel)
	{
		Picker<ShowcaseOption<string[]>> filter = new()
		{
			MinWidth = 140,
			ItemsSource = viewModel.FileFilters,
			SelectedItem = Bind(vm => vm.SelectedFileFilter)
				.TwoWay((vm, val) => vm.SelectedFileFilter = val!)
		};

		AddShowcase(
			"Files",
			"Pick any document or restrict the system browser to selected file extensions.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 10,

						Children =
						{
							new Image
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Width = 72,
								Height = 62,
								Source = ImageSource.Symbol("doc.fill"),
								SymbolSize = 54,
								Tint = Colors.Mint
							},

							PickButton(
								"Pick file",
								"doc.badge.plus",
								viewModel.PickFileCommand),

							ResultLabel(Bind(vm => vm.FileResult))
						}
					},
					190),
				SettingRow("Allowed files", filter)),
			Code(vm => vm.FileCode));
	}


	static Button PickButton(
		string title,
		ImageSource icon,
		ICommand command) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = title,
			Icon = icon,
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Medium,
			Command = command
		};

	static Label ResultLabel(
		BindingExpression<string?> result) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = result,
			TextStyle = TextStyle.Footnote,
			TextColor = Colors.SecondaryLabel,
			TextAlignment = TextAlignment.Center,
			MaxLines = 2
		};
}
