using BusinessLogic.Services;
using CantinaAPI.Extensions;
using CantinaAPI.Handlers;
using CantinaAPI.Models;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CantinaAPI.Controllers
{
    [ApiController]
    [Route("/order/")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly UserManager<User> _userManager;
        private readonly INotificationOrderHandler _notificationOrderHandler;

        public OrderController(IOrderService orderService, IProductService productService, UserManager<User> userManager, INotificationOrderHandler notificationOrderHandler)
        {
            _orderService = orderService;
            _productService = productService;
            _userManager = userManager;
            _notificationOrderHandler = notificationOrderHandler;
        }

        [NonAction]
        async Task<bool> CheckStock(List<OrderItemRequest> orderRequest)
        {
            foreach (var item in orderRequest)
            {
                var product = await _productService.GetProduct(item.Id);
                if (product == null || product.Stock < item.Quantity)
                {
                    return false;
                }
            }
            return true;
        }

        [HttpPost("changeToSelfPickup")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ChangeToSelfPickup([FromQuery] int orderId)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            await _orderService.ChangeToSelfPickup(orderId, email);
            return Ok();
        }

        [HttpPost("add")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddOrder([FromBody] List<OrderItemRequest> products, [FromQuery] bool selfPickup = false)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (products == null || products.Count == 0)
            {
                return BadRequest("No products in order");
            }
            if (await CheckStock(products))
            {
                var productsDomain = products.Select(p => p.ToDomain()).ToList();
                int orderid = await _orderService.AddOrder(email, productsDomain, selfPickup);
                await _notificationOrderHandler.SendOrderNotification(orderid);
                return Ok();
            }
            else
            {
                return BadRequest("Not enough stock");
            }
        }

        [HttpGet("get/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderById(id);
            return Ok(order);
        }

        [HttpGet("getAll")]
        [Authorize(Roles = "User")]
        public async Task<List<CantinaAPI.Models.DetailedOrder>> GetAllOrders()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var orders = await _orderService.GetOrders(email);
            return orders.Select(o => o.ToAPI()).ToList();
        }
    }
}
