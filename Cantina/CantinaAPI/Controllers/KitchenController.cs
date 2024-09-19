using BusinessLogic.Services;
using CantinaAPI.Extensions;
using CantinaAPI.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CantinaAPI.Controllers;

[ApiController]
[Route("/kitchen/")]
public class KitchenController : ControllerBase
{
    private readonly IKitchenService _kitchenService;
    private readonly INotificationOrderHandler _notificationOrderHandler;
    public KitchenController(IKitchenService kitchenService, INotificationOrderHandler notificationOrderHandler)
    {
        _kitchenService = kitchenService;
        _notificationOrderHandler = notificationOrderHandler;
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("prepare")]
    public async Task<IActionResult> StartPackaging([FromQuery] int orderId)
    {
        try
        {
            await _kitchenService.StartPackaging(orderId);
            
            await _notificationOrderHandler.SendChangedOrderStatusNotification(orderId, "Processing");

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("finish")]
    public async Task<IActionResult> FinishPackaging([FromQuery] int orderId)
    {
        try
        {
            var verificationCode = await _kitchenService.FinishPackaging(orderId);
            await _notificationOrderHandler.SendFinishKitchenNotification(orderId);
            //
            await _notificationOrderHandler.SendChangedOrderStatusNotification(orderId, "Awaiting Courier Assignment");

            return Ok(verificationCode);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("getAllKitchen")]
    public async Task<IActionResult> GetAllKitchenOrders()
    {
        try
        {
            var orders = await _kitchenService.GetAllOrdersKitchen();
            return Ok(orders.Select(o => o.ToKitchenOrder()));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("getAllPickup")]
    public async Task<IActionResult> GetAllPickupOrders()
    {
        try
        {
            var orders = await _kitchenService.GetPickupOrders();
            return Ok(orders.Select(o => o.ToKitchenOrder()));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("CancelOrder")]
    public async Task<IActionResult> CancelOrder([FromQuery] int orderId)
    {
        try
        {
            await _kitchenService.CancelOrder(orderId);
            await _notificationOrderHandler.SendChangedOrderStatusNotification(orderId, "Cancelled");
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


}
