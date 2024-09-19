using AdminPanel.Models;
using AdminPanel.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AdminPanel
{
    public partial class Order : UserControl
    {
        private readonly KitchenOrder _order;
        private readonly ApiService _apiService = App.AppHost!.Services.GetRequiredService<ApiService>();
        public static readonly DependencyProperty ProductsProperty =
            DependencyProperty.Register("Products", typeof(List<KitchenProduct>), typeof(Order), new PropertyMetadata(new List<KitchenProduct>()));

        public List<KitchenProduct> Products
        {
            get { return (List<KitchenProduct>)GetValue(ProductsProperty); }
            set { SetValue(ProductsProperty, value); }
        }
        private bool isHighlighted = false;

        public Order(KitchenOrder kitchenOrder = null)
        {
            _order = kitchenOrder;
            InitializeComponent();
            this.MouseLeftButtonDown += Order_MouseLeftButtonDown;
            checkStatus();
        }

        private void checkStatus()
        {
            if (_order != null)
                if (_order.status == "Processing")
                {
                    this.Background = Brushes.Green;
                    isHighlighted = true;
                }

        }

        private async void Order_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isHighlighted)
            {
                this.Background = Brushes.Green;
                isHighlighted = true;
                await _apiService.PrepareKitchenOrderAsync(_order.id);
            }
            else
            {
                await _apiService.FinishKitchenOrderAsync(_order.id);
                var parent = this.Parent as Panel;
                if (parent != null)
                {
                    parent.Children.Remove(this);
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await _apiService.CancelOrder(_order.id);
            var parent = this.Parent as Panel;
            if (parent != null)
            {
                parent.Children.Remove(this);
            }

        }
    }
}
