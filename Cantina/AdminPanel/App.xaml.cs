using AdminPanel.Pages;
using AdminPanel.Services;
using AdminPanel.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace AdminPanel;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost? AppHost { get; private set; }
    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<Login>();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<KitchenOrdersPage>();
                services.AddSingleton<DeliveryOrdersPage>();
                services.AddSingleton<ProductsPage>();
                services.AddSingleton<EditProductWindow>();
                services.AddSingleton<ApiService>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();
        var startForm = AppHost.Services.GetRequiredService<Login>();
        startForm.Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        base.OnExit(e);
    }
}
