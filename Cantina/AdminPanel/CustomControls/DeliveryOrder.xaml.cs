using AdminPanel.Models;
using AdminPanel.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AdminPanel.CustomControls;

/// <summary>
/// Interaction logic for DeliveryOrder.xaml
/// </summary>
public partial class DeliveryOrder : UserControl
{
    private readonly ApiService _apiService = App.AppHost!.Services.GetRequiredService<ApiService>();
    private readonly KitchenOrder orderModel;
    public readonly int Id;
    public DeliveryOrder(KitchenOrder order)
    {
        InitializeComponent();
        DataContext = order;
        orderModel = order;
        if (order.status == "SelfPickup")
        {
            this.MouseLeftButtonDown += Order_MouseLeftButtonDown;
            border.BorderBrush = Brushes.Aqua;
        }
        Id = order.id;
    }

    private async void Order_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        await _apiService.FinishSelfPickUpOrderAsync(orderModel.id);
        var parent = this.Parent as Panel;
        if (parent != null)
        {
            parent.Children.Remove(this);
        }
    }
}
