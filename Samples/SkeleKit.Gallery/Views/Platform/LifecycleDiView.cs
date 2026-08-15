using SkeleKit.Gallery.ViewModels.Platform;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Platform;

[Page]
internal sealed class LifecycleDiView : ShowcaseView<LifecycleDiViewModel>
{
	public LifecycleDiView(
		LifecycleDiViewModel viewModel) : base(viewModel, "Lifecycle & DI", Colors.Green)
	{
		AddLifecycleShowcase();
		AddCodeShowcase(
			"Service registration",
			"Choose singleton or transient lifetimes when configuring the application container.",
			Code(model => model.RegistrationCode));
		AddCodeShowcase(
			"Constructor injection",
			"Pages and ViewModels receive their registered dependencies through ordinary constructors.",
			Code(model => model.InjectionCode));
	}


	void AddLifecycleShowcase()
	{
		AddShowcase(
			"Foreground transitions",
			"Send the app to the background and return to update the live transition history.",
			ShowcaseBox.Canvas(
				new StackPanel
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Spacing = 7,

					Children =
					{
						new Image
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							Source = Bind(
								model => model.StatusIcon,
								static icon => (ImageSource?)ImageSource.Symbol(icon)),
							SymbolSize = 30,
							Tint = Colors.Green
						},

						new Label
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							Text = Bind(model => model.StatusTitle),
							TextStyle = TextStyle.Title3,
							FontWeight = FontWeight.Semibold,
							TextAlignment = TextAlignment.Center
						},

						new Label
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							Text = Bind(model => model.TransitionCounts),
							TextStyle = TextStyle.Subheadline,
							TextColor = Colors.SecondaryLabel,
							TextAlignment = TextAlignment.Center
						},

						new Label
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							Text = Bind(model => model.LastTransition),
							TextStyle = TextStyle.Caption1,
							TextColor = Colors.TertiaryLabel,
							TextAlignment = TextAlignment.Center
						}
					}
				},
				190),
			Code(model => model.LifecycleCode));
	}
}
