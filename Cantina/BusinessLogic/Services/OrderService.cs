using BusinessLogic.Extensions;
using BusinessLogic.Models;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly UserManager<User> _userManager;
    private readonly IProductService _productService;
    private readonly IOrderItemsRepository _orderItemsRepository;


    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, UserManager<User> userManager, IProductService productService, IOrderItemsRepository orderItemsRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _userManager = userManager;
        _productService = productService;
        _orderItemsRepository = orderItemsRepository;
    }
    public async Task<int> AddOrder(string userEmail, List<Items> orderItems,bool selfPickup)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        var order = new Order
        {
            UserId = user.Id,
            Location = user.Location
        };
        if(selfPickup)
        {
            order.Location = "SelfPickup";
        }
        await ValidateOrderItemsAsync(orderItems);
        int id = await _orderRepository.Add(order);

        foreach (var item in orderItems)
        {
            var product = await _productRepository.GetById(item.Id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            product.Stock -= item.Quantity;
            await _productRepository.UpdateStock(product.Id, product.Stock);

            await _orderItemsRepository.Add(new OrderItems
            {
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = item.Quantity,
                UserId = user.Id
            });

        }
        return id;
    }

    private async Task ValidateOrderItemsAsync(List<Items> orderItems)
    {
        foreach (var item in orderItems)
        {
            var product = await _productRepository.GetById(item.Id);
            if (item.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than 0");
            }
            if (product == null || !product.IsActive)
            {
                throw new Exception($"Product with ID {item.Id} not found or inactive");
            }

            if (product.Stock < item.Quantity)
            {
                throw new Exception($"Not enough stock for product with ID {item.Id}");
            }
        }
    }


    public async Task DeleteOrder(string userEmail, int orderId)
    {
        var order = await _orderRepository.GetById(orderId);
        if (order == null)
        {
            throw new Exception("Order not found");
        }
        if (order.UserId != _userManager.FindByEmailAsync(userEmail).Result.Id)
        {
            throw new Exception("You are not allowed to delete this order");
        }
        var orderItems = await _orderItemsRepository.GetAllByOrderId(orderId);
        await _orderRepository.Delete(orderId);

    }

    public async Task<OrderModel> GetOrderById(int id)
    {
        var order = await _orderRepository.GetById(id);
        if (order == null)
        {
            throw new Exception("Order not found");
        }
        var orderModel = order.ToOrderModel(await _orderItemsRepository.GetAllByOrderId(id));
        return orderModel;
    }

    public async Task<List<OrderModel>> GetOrders(string userEmail)
    {
        var userId = _userManager.FindByEmailAsync(userEmail).Result.Id;
        var orders = await _orderRepository.GetOrdersByUser(userId);
        return orders.Select(o => o.ToOrderModel(_orderItemsRepository.GetAllByOrderId(o.Id).Result)).ToList();
    }

    public async Task ChangeToSelfPickup(int orderId, string? email)
    {
        var order = await _orderRepository.GetById(orderId);
        if (order == null)
        {
            throw new Exception("Order not found");
        }
        if (order.UserId != _userManager.FindByEmailAsync(email).Result.Id && !(order.Status == "Awaiting Courier Assignment" || order.Status == "Processing" || order.Status == "Pending"))
        {
            throw new Exception("You are not allowed to change this order. Delivery has already started.");
        }
        await _orderRepository.UpdateStatus(orderId, "SelfPickup");
        await _orderRepository.UpdateLocation(orderId, "SelfPickup");
    }
}
