using DataAccessLayer.Repository;
using Microsoft.AspNetCore.SignalR;

namespace CantinaAPI.Hubs;

public class CamHub : Hub
{
    private readonly ICounterRepository _counterRepository;
    public CamHub(ICounterRepository counterRepository)
    {
        _counterRepository = counterRepository;
    }
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {


        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string message)
    {
        Console.WriteLine($"Mesaj primit de la client: {message}");

        await Clients.All.SendAsync("ReceiveMessage", message + "MESAJ AJUNS CU SUCCES");
    }

    public async Task UpdateCounts(int cntUp, int cntDown)
    {
        Console.WriteLine($"Contor UP: {cntUp}, Contor DOWN: {cntDown}");

        await _counterRepository.AddCounter(cntUp, cntDown);
        await Clients.All.SendAsync("ReceiveCounts", cntUp, cntDown);
    }

}
