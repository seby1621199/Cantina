using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AdminPanel.CustomControls;

/// <summary>
/// Interaction logic for Product.xaml
/// </summary>
public partial class Product : UserControl
{
    public ProductModel productModel;
    private readonly ApiService _apiService= App.AppHost.Services.GetRequiredService<ApiService>();
    private readonly EditProductWindow editProductWindow = App.AppHost.Services.GetRequiredService<EditProductWindow>();
    //private readonly 
    public Product(ProductModel product)
    {
        InitializeComponent();
        DataContext = product;
        productModel = product;
        this.MouseLeftButtonDown += Order_MouseLeftButtonDown;
        border.BorderBrush = productModel.active ? Brushes.Green : Brushes.Red;
    }

    private void Setup(ProductModel productModel)
    {
        DataContext = productModel;
        this.productModel = productModel;
        border.BorderBrush = productModel.active ? Brushes.Green : Brushes.Red;
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        editProductWindow.SetProduct(productModel);
        editProductWindow.Show();
    }


    private async void Order_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {

        await _apiService.ChangeProductStatusAsync(productModel.id,!productModel.active);
        border.BorderBrush=productModel.active?Brushes.Red:Brushes.Green;
        var newproduct=await  _apiService.GetProductByIdAsync(productModel.id);
        Setup(newproduct);
    }
}
