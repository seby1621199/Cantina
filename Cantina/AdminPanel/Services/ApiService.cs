using AdminPanel.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace AdminPanel.Services;

public class ApiService
{
    private readonly HttpClient _httpClient = new();
    private string _token = string.Empty;
    private readonly string _baseUrl = "https://localhost:7076";
    private HubConnection _hubConnection;
    public delegate void Operation(KitchenOrder orders);

    public void SetToken(string token)
    {
        _token = token ?? throw new ArgumentNullException(nameof(token));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task PrepareKitchenOrderAsync(int id)
    {
        var url = $"{_baseUrl}/kitchen/prepare?orderId={id}";

        var response = await _httpClient.PostAsync(url, null);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error preparing order: {response.StatusCode} - {errorContent}");
        }
    }

    public async Task CancelOrder(int id)
    {
        var url = $"{_baseUrl}/kitchen/CancelOrder?orderId={id}";

        var response = await _httpClient.PostAsync(url, null);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error preparing order: {response.StatusCode} - {errorContent}");
        }
    }

    public async Task<string> FinishKitchenOrderAsync(int id)
    {
        var url = $"{_baseUrl}/kitchen/finish?orderId={id}";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error finishing order: {response.StatusCode} - {errorContent}");
        }

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string?> CheckCredentialsAsync(string email, string password)
    {
        var url = $"{_baseUrl}/user/login";
        var loginData = new { email, password };
        var json = JsonSerializer.Serialize(loginData);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, data);
        return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
    }

    public async Task<ICollection<KitchenOrder>> GetDeliveryOrdersAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/kitchen/getAllPickup");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<KitchenOrder>>(responseContent) ?? Enumerable.Empty<KitchenOrder>().ToList();
        }

        return Enumerable.Empty<KitchenOrder>().ToList();
    }

    public async Task InitializeSignalRAsync(Operation operation)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{_baseUrl}/orderHub", options => options.AccessTokenProvider = () => Task.FromResult(_token))
            .Build();

        _hubConnection.On<KitchenOrder>("ReceiveOrderNotification", order => Application.Current.Dispatcher.Invoke(() => operation(order)));

        await _hubConnection.StartAsync();
    }

    public async Task SetLoadDeliveryMethod(Operation operation)
    {
        if (_hubConnection?.State != HubConnectionState.Connected)
        {
            await ReconnectAsync();
        }
        if(_hubConnection!=null)
        _hubConnection.On<KitchenOrder>("ReceiveFinishKitchenNotification", order => Application.Current.Dispatcher.Invoke(() => operation(order)));
    }

    public async Task SetStartDeliveryMethod(Operation operation)
    {
        if (_hubConnection?.State != HubConnectionState.Connected)
        {
            await ReconnectAsync();
        }

        _hubConnection.On<KitchenOrder>("ReceiveStartDeliveryNotification", order => Application.Current.Dispatcher.Invoke(() => operation(order)));
    }

    public async Task SendNotificationProductUpdate(int id)
    {
        if (_hubConnection?.State != HubConnectionState.Connected)
        {
            await ReconnectAsync();
        }
        await _hubConnection.InvokeAsync("SendNotificationProductUpdate", id);
    }

    private async Task ReconnectAsync()
    {
        while (_hubConnection?.State != HubConnectionState.Connected)
        {
            try
            {
                await _hubConnection.StartAsync();
            }
            catch
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }

    public async Task<IEnumerable<KitchenOrder>> GetOrdersAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/kitchen/getAllKitchen");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<KitchenOrder>>(responseContent) ?? Enumerable.Empty<KitchenOrder>();
        }

        return [];
    }

    public async Task ChangeProductStatusAsync(int id, bool status)
    {
        var url = $"{_baseUrl}/product/changeStatus?id={id}&status={status.ToString().ToLower()}";

        var response = await _httpClient.PatchAsync(url, null);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error changing product status: {response.StatusCode} - {errorContent}");
        }

        await SendNotificationProductUpdate(id);
    }

    public async Task<ICollection<ProductModel>> GetProductsAsync(bool withInactive)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/product/getAll?withIds=true&active={withInactive}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProductModel>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
        }

        return [];
    }

    public async Task<ProductModel> GetProductByIdAsync(int id)
    {
        var url = $"{_baseUrl}/product/getById?id={id}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductModel>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (product == null)
            {
                throw new Exception("Product data was not found.");
            }

            return product;
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Error retrieving product: {response.StatusCode} - {errorContent}");
    }

    public async Task UpdateProduct(ProductUpdate product, int id)
    {
        var url = $"{_baseUrl}/product/update?id={id}";

        var json = JsonSerializer.Serialize(product);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(url, data);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error updating product: {response.StatusCode} - {errorContent}");
        }
    }

    public async Task FinishSelfPickUpOrderAsync(int id)
    {
        var url = $"{_baseUrl}/finishSelfPickup?orderId={id}";

        var response = await _httpClient.PostAsync(url, null);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error finishing order: {response.StatusCode} - {errorContent}");
        }
    }
}
