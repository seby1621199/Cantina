using AdminPanel.CustomControls;
using AdminPanel.Extensions;
using AdminPanel.Models;
using AdminPanel.Services;
using System.Windows;
using System.Windows.Controls;

namespace AdminPanel;

/// <summary>
/// Interaction logic for KitchenOrdersPage.xaml
/// </summary>
public partial class KitchenOrdersPage : Page
{
    private readonly ApiService _apiService;
    public KitchenOrdersPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
        LoadOrdersAsync();
        _apiService.InitializeSignalRAsync(LoadOrder);
    }

   

    private void LoadOrder(KitchenOrder order)
    {

        var orderControl = new Order(order);
        orderControl.Products = order.products;
        orderControl.Visibility = Visibility.Visible;

        panel.Children.Add(orderControl);
    }

    public async Task ReloadOrders()
    {
        panel.Children.Clear();
        await LoadOrdersAsync();
    }
    private async Task LoadOrdersAsync()
    {
        var orders = await _apiService.GetOrdersAsync();
        foreach (var order in orders)
        {
            var orderControl = new Order(order);
            orderControl.Products = order.products;
            orderControl.Visibility = Visibility.Visible;
            panel.Children.Add(orderControl);
        }
    }
}
