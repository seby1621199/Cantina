using AdminPanel.Pages;
using AdminPanel.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace AdminPanel;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>

public partial class MainWindow : Window
{

    private readonly KitchenOrdersPage kitchenOrdersPage = App.AppHost!.Services.GetRequiredService<KitchenOrdersPage>();
    private readonly DeliveryOrdersPage deliveryOrdersPage = App.AppHost!.Services.GetRequiredService<DeliveryOrdersPage>();
    private readonly ProductsPage productsPage = App.AppHost!.Services.GetRequiredService<ProductsPage>();
    private readonly ApiService apiService;
    private bool withInactive = false;
    public MainWindow(ApiService apiService)
    {
        InitializeComponent();
        this.apiService = apiService;
        frame.Content = kitchenOrdersPage;
        SwitchText.Text = "Kitchen Orders";

    }

    private async void ManualButton_Click(object sender, RoutedEventArgs e)
    {
        if (frame.Content == kitchenOrdersPage)
        {
            await kitchenOrdersPage.ReloadOrders();
        }
        else
        {
            if (frame.Content == deliveryOrdersPage)
                await deliveryOrdersPage.ReloadOrders();
            else
            {
                await productsPage.LoadProducts(withInactive);
            }
        }
    }
    private void SwitchButton_Click(object sender, RoutedEventArgs e)
    {
        if (frame.Content == kitchenOrdersPage)
        {
            frame.Content = deliveryOrdersPage;
            SwitchText.Text = "Delivery Orders";
        }
        else
        {
            frame.Content = kitchenOrdersPage;
            SwitchText.Text = "Kitchen Orders";
        }
    }
    private async void ProductsButton_Click(object sender, RoutedEventArgs e)
    {
        frame.Content = productsPage;
        SwitchText.Text = "Products";
        await productsPage.LoadProducts(withInactive);
    }
    private async void AutoReloadCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        withInactive = true;
        await productsPage.LoadProducts(withInactive);
    }

    private async void AutoReloadCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {

        withInactive = false;
        await productsPage.LoadProducts(withInactive);
    }

}
