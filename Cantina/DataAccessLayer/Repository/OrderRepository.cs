using DataAccessLayer.Extensions;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(CantinaContext context) : base(context)
    {

    }
    public async Task<List<Order>> DeliveryGetOrdersByStatus(string deliveryId, string status)
    {
        var orders = new List<Order>();
        orders = await _context.Orders.Where(x => x.Status == status && x.DeliveryPersonId == deliveryId).Include(O => O.User).ToListAsync();
        return orders;
    }

    public async Task UpdateStatus(int orderId, string status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            throw new Exception("Order not found");
        }
        order.Status = status;
        await _context.SaveChangesAsync();
    }


    public async Task<List<DetailedOrder>> GetOrdersByUser(string userId)
    {
        var orders = await _context.Orders.Where(x => x.UserId == userId).Include(o => o.User).OrderByDescending(o => o.OrderDate).ToListAsync();
        List<DetailedOrder> detailedOrders = new List<DetailedOrder>();
        await ToDetailedOrders(orders, detailedOrders);

        return detailedOrders;
    }

    public async Task<List<DetailedOrder>> GetFiveAwaitingCourierAssignment()
    {
        var orders = await _context.Orders.Include(o=>o.User)
                                          .Where(x => x.Status == "Awaiting Courier Assignment" && x.Location != "Self Pickup")
                                          .OrderBy(o => o.Location)
                                          .OrderByDescending(o => o.OrderDate)
                                          .ThenBy(o => o.Location).Take(5)
                                          .ToListAsync();

        List<DetailedOrder> detailedOrders = new List<DetailedOrder>();
        await ToDetailedOrders(orders, detailedOrders);


        return detailedOrders;
    }

    public async Task<bool> SetDeliveryPerson(string deliveryPersonId, int orderId)
    {
        var order = await _context.Orders.Where(x => x.Id == orderId).FirstOrDefaultAsync();
        if (order != null && order.DeliveryPersonId == null)
        {
            order.DeliveryPersonId = deliveryPersonId;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    private async Task ToDetailedOrders(List<Order> orders, List<DetailedOrder> detailedOrders)
    {
        foreach (var order in orders)
        {
            var orderItems = await _context.OrderItems.Where(x => x.OrderId == order.Id).ToListAsync();
            foreach (var orderItem in orderItems)
            {
                orderItem.Product = await _context.Products.FirstOrDefaultAsync(x => x.Id == orderItem.ProductId)??throw new Exception("No product.");
            }
            detailedOrders.Add(order.ToDetailed(orderItems));
        }
    }

    public async Task<DetailedOrder> GetDetailedOrder(int id)
    {
        var order = await _context.Orders.Include(o=>o.User).FirstOrDefaultAsync(x => x.Id == id);
        var detailedOrder = new DetailedOrder();
        if (order != null)
        {
            var orderItems = await _context.OrderItems.Where(x => x.OrderId == order.Id).ToListAsync();
            foreach (var orderItem in orderItems)
            {
                orderItem.Product = await _context.Products.FirstOrDefaultAsync(x => x.Id == orderItem.ProductId)?? throw new Exception("No product.");
            }
            detailedOrder = order.ToDetailed(orderItems);
        }
        return detailedOrder;
    }

    public async Task<bool> CheckAlreadyDelivery(string deliveryId)
    {
        var status = await GetDeliveryStatus(deliveryId);
        if (status != "Ready")
        {
            return true;
        }
        return false;
    }

    public async Task<List<DetailedOrder>> GetAllOrdersByStatus(string status)
    {

        var orders = await _context.Orders.Where(x => x.Status == status).ToListAsync();
        List<DetailedOrder> detailedOrders = new List<DetailedOrder>();
        await ToDetailedOrders(orders, detailedOrders);

        return detailedOrders;
    }

    public async Task<string> GetVerificationCode(int orderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (order != null)
        {
            return order.VerificationCode;
        }
        return String.Empty;
    }

    public async Task UpdateLocation(int orderId, string location)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId) ?? throw new Exception("Order not found");
        order.Location = location;
        await _context.SaveChangesAsync();
    }

    public async Task<string> GetDeliveryStatus(string deliveryId)
    {


        bool hasDeliveryOrder = await _context.Orders
            .AnyAsync(o => o.DeliveryPersonId == deliveryId && o.Status == "Delivery");

        if (hasDeliveryOrder)
        {
            return "Delivery";
        }


        bool hasAwaitingPickupOrder = await _context.Orders
            .AnyAsync(o => o.DeliveryPersonId == deliveryId && o.Status == "Awaiting Pickup");

        if (hasAwaitingPickupOrder)
        {
            return "Awaiting Pickup";
        }

        return "Ready";
    }

    public async Task<string> GetUserId(int orderId)
    {
        var order = await _context.Orders.Include(o => o.User).FirstOrDefaultAsync(x => x.Id == orderId);
        if (order == null)
            throw new Exception("Order not found");
        var userId = order.User.Id;
        return userId;
    }
}
