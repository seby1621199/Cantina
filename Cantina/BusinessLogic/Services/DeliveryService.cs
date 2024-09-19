using BusinessLogic.Exceptions;
using BusinessLogic.Extensions;
using BusinessLogic.Models;
using DataAccessLayer.Repository;

namespace BusinessLogic.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IOrderRepository _orderRepository;
        public DeliveryService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        private async Task<string> GetVerificationCodeAndSetDeliveryPerson(string DeliveryPersonId, int orderId)
        {
            if (await _orderRepository.SetDeliveryPerson(DeliveryPersonId, orderId))
                return await _orderRepository.GetVerificationCode(orderId);
            return String.Empty;
        }

        public async Task<List<DeliveryDetailsOrder>> GetFiveAwaitingCourierAssignment(string DeliveryPersonId)
        {
            var orders = await _orderRepository.GetFiveAwaitingCourierAssignment();
            var toDeliveryOrders = new List<DeliveryDetailsOrder>();
            foreach (var order in orders)
            {
                await ChangeOrderStatus(order.Id, "Awaiting Pickup");
                await GetVerificationCodeAndSetDeliveryPerson(DeliveryPersonId, order.Id);
                var newOrderDetails = order.ToDeliveryDetailsOrder();
                toDeliveryOrders.Add(newOrderDetails);
            }
            return toDeliveryOrders;
        }

        public async Task ChangeOrderStatus(int orderId, string status)
        {
            await _orderRepository.UpdateStatus(orderId, status);
        }


        public async Task<bool> CheckAlreadyInDelivery(string deliveryId)
        {
            if (await _orderRepository.CheckAlreadyDelivery(deliveryId))
            {
                return true;
            }
            return false;
        }

        public async Task FinishSelfPickUpOrder(int Id)
        {
            var order = await _orderRepository.GetById(Id);
            if (order == null)
            {
                throw new DeliveryException("Order not found");
            }
            if (order.Status != "SelfPickup")
            {
                throw new DeliveryException("Order is not in delivery");
            }
            await _orderRepository.UpdateStatus(Id, "Finished");

        }

        public async Task FinishOrder(int id, string deliveryPersonId)
        {
            var order = await _orderRepository.GetById(id);
            if (order == null)
            {
                throw new DeliveryException("Order not found");
            }
            if (order.Status != "Delivery")
            {
                throw new DeliveryException("Order is not in delivery");
            }
            if (order.DeliveryPersonId != deliveryPersonId)
            {
                throw new DeliveryException("Order is not assigned to you");
            }
            await _orderRepository.UpdateStatus(id, "Finished");
        }

        public async Task StartDelivery(string userId)
        {

            if (await _orderRepository.CheckAlreadyDelivery(userId) == true)
            {

                var orders = await _orderRepository.DeliveryGetOrdersByStatus(userId, "Awaiting Pickup");
                if (orders.Count > 0)
                    foreach (var order in orders)
                    {
                        await _orderRepository.UpdateStatus(order.Id, "Delivery");
                    }
                else
                {
                    throw new DeliveryException("No orders to start delivery.");
                }


            }
            else
            {
                throw new DeliveryException("No orders to start delivery.");
            }
        }

        public async Task<string> GetStatus(string deliveryId)
        {
            var status = await _orderRepository.GetDeliveryStatus(deliveryId);
            return status;
        }

        public async Task<List<DeliveryDetailsOrder>> GetOrdersByStatus(string userId, string status)
        {
            var orders = await _orderRepository.DeliveryGetOrdersByStatus(userId, status);
            return orders.Select(o => o.ToDeliveryDetailsOrder()).ToList();
        }

        public async Task<string> GetVerificationCode(int orderId, string userId)
        {
            var order = await _orderRepository.GetDetailedOrder(orderId);
            if (order == null)
            {
                throw new DeliveryException("Order not found");
            }
            if (order.User.Id != userId)
            {
                throw new DeliveryException("This order is not yours!");
            }
            if (order.Status != "SelfPickup")
            {
                throw new DeliveryException("Order is not ready!");
            }
            return order.VerificationCode;
        }
    }
}
