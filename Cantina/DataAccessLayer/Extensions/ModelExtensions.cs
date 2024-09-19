using DataAccessLayer.Models;

namespace DataAccessLayer.Extensions
{
    public static class ModelExtensions
    {
        public static DetailedOrder ToDetailed(this Order orderModel, List<OrderItems> orderItems)
        {
            return new DetailedOrder
            {
                Id = orderModel.Id,
                UserId = orderModel.UserId,
                orderItems = orderItems,
                User = orderModel.User,
                Location = orderModel.Location,
                Status = orderModel.Status,
                VerificationCode = orderModel.VerificationCode,
            };
        }

    }
}
