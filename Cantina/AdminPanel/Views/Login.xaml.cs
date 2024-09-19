using AdminPanel.Services;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Windows;
using System.Windows.Media;

namespace AdminPanel;

/// <summary>
/// Interaction logic for Login.xaml
/// </summary>
public partial class Login : Window
{
    private readonly ApiService _apiService;
    public static Task<List<string>> GetRolesFromTokenAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token) ?? throw new ArgumentException("Invalid JWT token");
        var roles = jwtToken.Claims
            .Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" || c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            .Select(c => c.Value)
            .ToList();

        return Task.FromResult(roles);
    }


    public Login(ApiService apiService)
    {
        _apiService = apiService;
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var token = await _apiService.CheckCredentialsAsync(EmailField.Text, PasswordField.Password);
        if (token != null)
        {
            var roles = await GetRolesFromTokenAsync(token);
            if (roles.Contains("Admin"))
            {
                _apiService.SetToken(token);
                var mainWindow = App.AppHost?.Services.GetRequiredService<MainWindow>();
                mainWindow?.Show();
                this.Close();
            }
            else
            {
                message.Text = "You are not an admin";
                message.Foreground = Brushes.Red;
                message.Visibility = Visibility.Visible;
            }
        }
        else
        {
            message.Text = "Invalid credentials";
            message.Foreground = Brushes.Red;
            message.Visibility = Visibility.Visible;
        }
    }
}
