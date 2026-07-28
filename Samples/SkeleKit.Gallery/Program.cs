using Microsoft.Extensions.DependencyInjection;
using SkeleKit;
using SkeleKit.Gallery;

SkeleApplication.CreateBuilder()
	.UseServices(services => services.AddTransient<MainViewModel>())
	.UseAccent(Colors.Indigo)
	.Stack<MainView>(preferLargeTitles: true)
	.Build()
	.Run(args);
