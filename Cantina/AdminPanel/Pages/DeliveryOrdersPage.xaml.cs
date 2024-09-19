using AdminPanel.CustomControls;
using AdminPanel.Models;
using AdminPanel.Services;
using System.Windows.Controls;

namespace AdminPanel;

/// <summary>
/// Interaction logic for DeliveryOrdersPage.xaml
/// </summary>
/// 
public partial class DeliveryOrdersPage : Page
{
    private readonly ApiService _apiService;
    public DeliveryOrdersPage(ApiService apiService)
    {
        _apiService = apiService;
        InitializeComponent();
        LoadOrdersPickup();
        _apiService.SetLoadDeliveryMethod(LoadOrderDelivery);
        _apiService.SetStartDeliveryMethod(RemoveOrder);
    }

    public async Task ReloadOrders(bool withInactive = false)
    {
        panel.Children.Clear();
        await LoadOrdersPickup(withInactive);
    }
    private async Task LoadOrdersPickup(bool withInactive = false)
    {
        var orders = await _apiService.GetDeliveryOrdersAsync();
        foreach (var order in orders)
        {
            var deliveryOrderControl = new DeliveryOrder(order);
            deliveryOrderControl.Visibility = System.Windows.Visibility.Visible;
            panel.Children.Add(deliveryOrderControl);
        }
    }


    private async void LoadOrderDelivery(KitchenOrder kitchenOrder)
    {
        var deliveryOrderControl = new DeliveryOrder(kitchenOrder);
        deliveryOrderControl.Visibility = System.Windows.Visibility.Visible;
        panel.Children.Add(deliveryOrderControl);
    }

    public void RemoveOrder(KitchenOrder orderModel)
    {
        var order = panel.Children.OfType<DeliveryOrder>().FirstOrDefault(x => x.Id == orderModel.id);
        if (order != null)
        {
            panel.Children.Remove(order);
        }
    }




}
