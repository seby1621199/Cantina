using BusinessLogic.Services;
using CantinaAPI.Extensions;
using CantinaAPI.Hubs;
using CantinaAPI.Models;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.SignalR;

namespace CantinaAPI.Handlers;

public class NotificationOrderHandler : INotificationOrderHandler
{

    private readonly IHubContext<OrderHub> _hubContext;
    private readonly IKitchenService _kitchenService;
    private readonly IOrderRepository _orderRepository;

    public NotificationOrderHandler(IHubContext<OrderHub> hubContext, IKitchenService kitchenService, IOrderRepository orderRepository)
    {
        _hubContext = hubContext;
        _kitchenService = kitchenService;
        _orderRepository = orderRepository;
    }

    public async Task SendOrderNotification(int orderid)
    {
        var order = await _kitchenService.GetOrderUserDetailsById(orderid);
        await _hubContext.Clients.Group("Admin").SendAsync("ReceiveOrderNotification", order.ToKitchenOrder());
    }

    public async Task SendFinishKitchenNotification(int orderid)
    {
        var order = await _kitchenService.GetOrderUserDetailsById(orderid);
        await _hubContext.Clients.Group("Admin").SendAsync("ReceiveFinishKitchenNotification", order.ToKitchenOrder());
        var userId = await _orderRepository.GetUserId(orderid);
        if (order.Status == "Awaiting Courier Assignment")
            await SendChangedOrderStatusNotification(orderid, "Awaiting Courier Assignment");
        if (order.Status == "SelfPickup")
            await SendChangedOrderStatusNotification(orderid, "Awaiting Self Pickup");

    }

    public async Task SendChangedOrderStatusNotification(int orderId, string status)
    {
        var userId = await _orderRepository.GetUserId(orderId);
        if (userId != null)
        {
            var orderNotification = new OrderNotification(orderId, status);
            var s= _hubContext.Clients.Group(userId);
            await _hubContext.Clients.Group(userId).SendAsync("OrderStatusChanged", orderNotification);
        }
    }

    public async Task SendStartDeliveryNotification(int orderid)
    {
        var order = await _kitchenService.GetOrderUserDetailsById(orderid);
        await _hubContext.Clients.Group("Admin").SendAsync("ReceiveStartDeliveryNotification", order.ToKitchenOrder());
    }

}
