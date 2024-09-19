using AdminPanel.Extensions;
using AdminPanel.Models;
using AdminPanel.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace AdminPanel.Views;

/// <summary>
/// Interaction logic for EditProductWindow.xaml
/// </summary>
public partial class EditProductWindow : Window
{
    private ProductModel productModel;
    private ApiService apiService= App.AppHost.Services.GetRequiredService<ApiService>();
    public EditProductWindow()
    {
        InitializeComponent();
    }

    public void SetProduct(ProductModel product)
    {
        DataContext = product;
        productModel = product;
    }

    protected override void OnClosed(EventArgs e)
    {
        this.Hide();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        await apiService.UpdateProduct(productModel.ToProductUpdateAPI(),productModel.id);
        await apiService.SendNotificationProductUpdate(productModel.id);
        this.Hide();
    }
}
