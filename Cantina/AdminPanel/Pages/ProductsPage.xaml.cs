using AdminPanel.CustomControls;
using AdminPanel.Services;
using System.Windows;
using System.Windows.Controls;

namespace AdminPanel.Pages
{
    /// <summary>
    /// Interaction logic for ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private readonly ApiService _apiService;
        public ProductsPage(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            LoadProducts();
        }

        public async Task LoadProducts(bool withInactive=false)
        {
            panel.Children.Clear();
            var products = await _apiService.GetProductsAsync(withInactive);
            foreach (var product in products)
            {
                var productControl = new Product(product);
                productControl.Visibility = Visibility.Visible;
                panel.Children.Add(productControl);
            }

        }
    }
}
