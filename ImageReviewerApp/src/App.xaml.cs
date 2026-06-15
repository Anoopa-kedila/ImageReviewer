using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ImageReviewerApp.Contracts;
using ImageReviewerApp.Services;
using ImageReviewerApp.Factories;
using ImageReviewerApp.ViewModels;

namespace ImageReviewerApp;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Core services
        services.AddSingleton<IImageLoaderService, ImageLoaderService>();
        services.AddSingleton<IImageSaveService, ImageSaveService>();
        services.AddSingleton<IImageOperationFactory, ImageOperationFactory>();
        services.AddSingleton<IDialogService, DialogService>();

        // ViewModels and Views
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
    }
}


