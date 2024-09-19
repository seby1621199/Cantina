using BusinessLogic.Exceptions;
using BusinessLogic.Services;
using CantinaAPI.Handlers;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CantinaAPI.Controllers;

[ApiController]
[Route("/delivery/")]
public class DeliveryController : ControllerBase
{
    IDeliveryService _deliveryService;
    IOrderRepository _orderRepository;
    INotificationOrderHandler _notificationOrderHandler;
    public DeliveryController(IDeliveryService deliveryService, IOrderRepository orderRepository, INotificationOrderHandler notificationOrderHandler)
    {
        _deliveryService = deliveryService;
        _orderRepository = orderRepository;
        _notificationOrderHandler = notificationOrderHandler;
    }
    [Authorize(Roles = "Delivery")]
    [HttpGet]
    [Route("/getOrders")]
    public async Task<IActionResult> GetOrders()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return BadRequest("User not found");
        }
        var status = await _deliveryService.GetStatus(userId);
        if (status == "Ready")
        {
            var ordersToDelivery = await _deliveryService.GetFiveAwaitingCourierAssignment(userId);
            foreach(var order in ordersToDelivery)
            {
                await _notificationOrderHandler.SendChangedOrderStatusNotification(order.Id, "Courier Assignment");
            }
            return Ok(ordersToDelivery);
        }
        if (status == "Delivery")
        {
            var ordersToDelivery = await _deliveryService.GetOrdersByStatus(userId, "Delivery");

            return Ok(ordersToDelivery);
        }
        if (status == "Awaiting Pickup")
        {
            var ordersToDelivery = await _deliveryService.GetOrdersByStatus(userId, "Awaiting Pickup");

            return Ok(ordersToDelivery);
        }
        return BadRequest("You are not in delivery and no delivery orders for the moment.");
    }

    [Authorize(Roles = "Delivery")]
    [HttpPost]
    [Route("/start")]
    public async Task<IActionResult> StartDelivery()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return BadRequest("User not found");
        }

        try
        {
            if (await _deliveryService.CheckAlreadyInDelivery(userId))
            {
                await _deliveryService.StartDelivery(userId);
                 var ordersToDelivery = await _deliveryService.GetOrdersByStatus(userId, "Delivery");
                foreach (var order in ordersToDelivery)
                {
                    await _notificationOrderHandler.SendStartDeliveryNotification(order.Id);
                    await _notificationOrderHandler.SendChangedOrderStatusNotification(order.Id, "Delivery");
                }
                return Ok();
            }
            return BadRequest("You are already in delivery");
        }
        catch (DeliveryException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An internal server error occurred.");
        }
    }


    [Authorize(Roles = "Delivery")]
    [HttpPost]
    [Route("/finish")]
    public async Task<IActionResult> FinishDelivery([FromQuery] int orderId)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return BadRequest("User not found");
        }
        try
        {
            await _deliveryService.FinishOrder(orderId, userId);
            await _notificationOrderHandler.SendChangedOrderStatusNotification(orderId, "Finished");
            return Ok();
        }
        catch (DeliveryException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An internal server error occurred.");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("/finishSelfPickup")]
    public async Task<IActionResult> FinishDeliverySelfPickup([FromQuery] int orderId)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return BadRequest("User not found");
        }
        try
        {
            await _deliveryService.FinishSelfPickUpOrder(orderId);
            await _notificationOrderHandler.SendChangedOrderStatusNotification(orderId, "Finished");
            return Ok();
        }
        catch (DeliveryException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An internal server error occurred.");
        }
    }


    [Authorize(Roles = "Delivery")]
    [HttpGet("/getStatus")]
    public async Task<IActionResult> GetStatus()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return BadRequest("User not found");
        }
        try
        {
            var status = await _deliveryService.GetStatus(userId);
            return Ok(status);
        }
        catch (DeliveryException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An internal server error occurred.");
        }
    }

    [Authorize(Roles = "User")]
    [HttpGet("/getVerificationCode")]
    public async Task<IActionResult> GetVerificationCode([FromQuery] int orderId)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return BadRequest("User not found");
        }
        try
        {
            var verificationCode = await _deliveryService.GetVerificationCode(orderId, userId);
            return Ok(verificationCode);
        }
        catch (DeliveryException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}
