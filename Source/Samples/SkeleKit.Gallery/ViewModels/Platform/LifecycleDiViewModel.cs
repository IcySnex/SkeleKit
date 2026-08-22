using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Platform;

internal enum GalleryLifecyclePhase
{
	Foreground,
	Background
}

internal sealed partial class LifecycleDiViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StatusTitle))]
	[NotifyPropertyChangedFor(nameof(StatusIcon))]
	GalleryLifecyclePhase phase = GalleryLifecyclePhase.Foreground;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TransitionCounts))]
	int backgroundCount;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TransitionCounts))]
	int foregroundCount;

	[ObservableProperty]
	string lastTransition = "No transitions recorded";

	public string StatusTitle =>
		Phase is GalleryLifecyclePhase.Foreground
			? "Foreground"
			: "Background";

	public string StatusIcon =>
		Phase is GalleryLifecyclePhase.Foreground
			? "app.badge.checkmark"
			: "moon.zzz";

	public string TransitionCounts =>
		$"Background {BackgroundCount}  ·  Foreground {ForegroundCount}";

	public IReadOnlyList<Span> LifecycleCode { get; } =
	[
		new(
			"""
			AppLifecycle lifecycle = new();

			SkeleApplication.CreateBuilder()
				.UseServices(services => services.AddSingleton(lifecycle))
				.UseLifecycle(
					background: lifecycle.EnteredBackground,
					foreground: lifecycle.EnteredForeground)
				.Build()
				.Run(args);
			""")
	];

	public IReadOnlyList<Span> RegistrationCode { get; } =
	[
		new(
			"""
			.UseServices(services =>
			{
				services.AddSingleton<IMovieStore, MovieStore>();
				services.AddSingleton<SessionState>();
				services.AddTransient<MovieViewModel>();
			})
			""")
	];

	public IReadOnlyList<Span> InjectionCode { get; } =
	[
		new(
			"""
			public sealed class MovieViewModel(
				IMovieStore movies,
				INavigator navigator)
			{
				public int MovieCount => movies.Count;
			}

			[Page]
			public sealed class MovieView : ContentView<MovieViewModel>
			{
				public MovieView(MovieViewModel viewModel) : base(viewModel)
				{
					Content = new Label
					{
						Text = Bind(
							model => model.MovieCount,
							count => $"{count} movies")
					};
				}
			}
			""")
	];


	internal void EnteredBackground()
	{
		BackgroundCount++;
		Phase = GalleryLifecyclePhase.Background;
		LastTransition = $"Background at {DateTime.Now:HH:mm:ss}";
	}

	internal void EnteredForeground()
	{
		ForegroundCount++;
		Phase = GalleryLifecyclePhase.Foreground;
		LastTransition = $"Foreground at {DateTime.Now:HH:mm:ss}";
	}
}
