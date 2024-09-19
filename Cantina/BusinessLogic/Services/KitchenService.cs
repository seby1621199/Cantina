using BusinessLogic.Extensions;
using BusinessLogic.Models;
using DataAccessLayer.Repository;

namespace BusinessLogic.Services
{
    public class KitchenService : IKitchenService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemsRepository _orderItemsRepository;
        public KitchenService(IOrderRepository orderRepository, IOrderItemsRepository orderItemsRepository)
        {
            _orderRepository = orderRepository;
            _orderItemsRepository = orderItemsRepository;
        }
        public async Task StartPackaging(int orderId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }
            if (order.Status != "Pending")
            {
                throw new Exception("Order is not in processing");
            }
            await _orderRepository.UpdateStatus(orderId, "Processing");
        }

        public async Task<List<OrderUserDetails>> GetAllOrdersKitchen()
        {
            var ordersProcessing = await _orderRepository.GetAllOrdersByStatus("Processing");
            var odersPending = await _orderRepository.GetAllOrdersByStatus("Pending");
            var orders = ordersProcessing.Concat(odersPending).ToList();
            if (orders.Count == 0)
            {
                throw new Exception("No orders pending");
            }
            return orders.Select(o => o.ToOrderUserDetails()).ToList();
        }


        public async Task<string> FinishPackaging(int orderId)
        {
            var order = await _orderRepository.GetDetailedOrder(orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }
            if (order.Location == "SelfPickup")
            {
                await _orderRepository.UpdateStatus(orderId, "SelfPickup");
            }
            else
            {
                if (order.Status == "Processing")
                    await _orderRepository.UpdateStatus(orderId, "Awaiting Courier Assignment");
            }
            return order.VerificationCode;
        }

        public async Task<List<OrderUserDetails>> GetPickupOrders()
        {
            var ordersReady = await _orderRepository.GetAllOrdersByStatus("Awaiting Courier Assignment");
            var ordersToPickup = await _orderRepository.GetAllOrdersByStatus("Awaiting Pickup");
            var ordersSelfPickup = await _orderRepository.GetAllOrdersByStatus("SelfPickup");
            var orders = ordersReady.Concat(ordersToPickup).Concat(ordersSelfPickup).ToList();
            return orders.Select(o => o.ToOrderUserDetails()).ToList();
        }

        public async Task<OrderUserDetails> GetOrderUserDetailsById(int orderId)
        {
            var order = await _orderRepository.GetDetailedOrder(orderId);
            return order.ToOrderUserDetails();
        }

        public async Task CancelOrder(int orderId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }
            if (order.Status == "Processing" || order.Status == "Pending")
            {
                await _orderRepository.UpdateStatus(orderId, "Cancelled");
            }
            else
                throw new Exception("Order cannot be cancelled");
        }
    }
}
