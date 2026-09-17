using SkeleKit.Gallery.ViewModels.Platform;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Platform;

[Page]
internal sealed class SplitViewsView : ShowcaseView<SplitViewsViewModel>
{
	public SplitViewsView(
		SplitViewsViewModel viewModel) : base(viewModel, "Split View", Colors.Green)
	{
		AddDemoShowcase(viewModel);
		AddCodeShowcase(
			"App shell",
			"Use a split view as the root shell with two or three columns, each with its own navigation stack.",
			Code(vm => vm.ShellCode));
		AddCodeShowcase(
			"Tab destination",
			"Any tab or sidebar destination can be a split view while the other tabs keep their single-stack behavior.",
			Code(vm => vm.TabCode));
		AddCodeShowcase(
			"Column navigation",
			"Push, pop and pop to root on a single column. When the split view collapses, the calls route to the visible stack.",
			Code(vm => vm.ColumnNavigationCode));
		AddCodeShowcase(
			"Column visibility",
			"Show and hide columns through the ISplitView service. Visibility queries and toggling require iOS 26.",
			Code(vm => vm.VisibilityCode));
	}


	void AddDemoShowcase(
		SplitViewsViewModel viewModel)
	{
		Button open = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Text = "Open the Notes demo",
			Icon = ImageSource.Symbol("rectangle.split.2x1"),
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Medium,
			Command = viewModel.OpenDemoCommand
		};

		AddShowcase(
			"Live demo",
			"The Notes destination is a hidden split view: it never appears in the tab bar or sidebar, and this button selects it through the navigator. Select a notebook on the left and drill into notes on the right.",
			ShowcaseBox.Canvas(open, 140),
			Code(vm => vm.DemoCode));
	}
}
