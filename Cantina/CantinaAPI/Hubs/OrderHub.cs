using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CantinaAPI.Hubs;

public class OrderHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User.Identity.IsAuthenticated)
        {
            var user = Context.User as ClaimsPrincipal;
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            var id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (roles.Contains("Admin"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
            }
            if (id != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, id);
            }

            await base.OnConnectedAsync();
        }
        else
        {
            Context.Abort();
        }
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (Context.User.Identity.IsAuthenticated)
        {
            var user = Context.User as ClaimsPrincipal;
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            var id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (roles.Contains("Admin"))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admin");
            }
            if(id!= null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, id);
            }

        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendNotificationProductUpdate(int id)
    {
        await Clients.All.SendAsync("ReceiveNotificationProductUpdate", id);
    }
}


