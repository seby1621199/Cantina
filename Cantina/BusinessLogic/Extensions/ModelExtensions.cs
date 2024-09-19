using BusinessLogic.Models;
using DataAccessLayer.Models;
using System.Runtime.CompilerServices;

namespace BusinessLogic.Extensions
{
    public static class ModelExtensions
    {
        public static Product ToProductDAL(this ProductDomain productModel)
        {
            return new Product
            {
                Name = productModel.Name,
                Description = productModel.Description,
                Price = productModel.Price,
                Stock = productModel.Stock,
                IsActive = productModel.Active,
                Unit=productModel.Unit,
            };
        }

        public static DeliveryDetailsOrder ToDeliveryDetailsOrder(this DetailedOrder detailedOrder)
        {
            return new DeliveryDetailsOrder
            {
                Id = detailedOrder.Id,
                Name = detailedOrder.User.LastName + " " + detailedOrder.User.FirstName,
                UserPhone = detailedOrder.User.PhoneNumber,
                VerificationCode = detailedOrder.VerificationCode,
                Location = detailedOrder.Location,
                Status = detailedOrder.Status,
                
            };
        }

        public static DeliveryDetailsOrder ToDeliveryDetailsOrder(this Order detailedOrder)
        {
            return new DeliveryDetailsOrder
            {
                Id = detailedOrder.Id,
                Name = detailedOrder.User.LastName + " " + detailedOrder.User.FirstName,
                UserPhone = detailedOrder.User.PhoneNumber,
                VerificationCode = detailedOrder.VerificationCode,
                Location = detailedOrder.Location,
                Status = detailedOrder.Status,
            };
        }

        public static ProductDomain? ToProductDomain(this Product productModel)
        {
            if (productModel == null)
                return null;

            return new ProductDomain
            {
                Id = productModel.Id,
                Name = productModel.Name,
                Description = productModel.Description,
                Price = productModel.Price,
                Stock = productModel.Stock,
                Active = productModel.IsActive,
                Unit = productModel.Unit
            };
        }

        public static Order ToOrderDAL(this OrderModel orderModel)
        {
            return new Order
            {
                UserId = orderModel.UserId

            };
        }

        public static OrderModel ToOrderModel(this Order orderModel, List<OrderItems> orderItems)
        {
            return new OrderModel
            {
                Id = orderModel.Id,
                OrderDate = orderModel.OrderDate,
                UserId = orderModel.UserId,
                orderItems = orderItems.Select(x => x.ToItemModel()).ToList(),
                Status = orderModel.Status,
                Location = orderModel.Location,
                VerificationCode = orderModel.VerificationCode
            };
        }

        public static PickUpOrderModel ToPickUp(this DetailedOrder order)
        {
            return new PickUpOrderModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Location = order.Location,
                Status = order.Status,
                User = order.User.UserName,
                VerificationCode = order.VerificationCode,
                orderItems = order.orderItems.Select(x => x.ToItemModel()).ToList()
            };
        }

        public static ItemModel ToItemModel(this OrderItems itemModel)
        {
            return new ItemModel
            {
                ProductId = itemModel.ProductId,
                ProductName = itemModel.Product.Name,
                Quantity = itemModel.Quantity,
                UnitPrice = itemModel.Product.Price,

            };
        }

        public static OrderModel ToDomain(this DetailedOrder detailedOrder)
        {
            return new OrderModel
            {
                Id = detailedOrder.Id,
                OrderDate = detailedOrder.OrderDate,
                UserId = detailedOrder.UserId,
                orderItems = detailedOrder.orderItems.Select(x => x.ToItemModel()).ToList()
            };
        }

        public static OrderUserDetails ToOrderUserDetails(this DetailedOrder detailedOrder)
        {
            return new OrderUserDetails
            {
                Id= detailedOrder.Id,
                Name = detailedOrder.User.UserName,
                Address = detailedOrder.Location,
                PhoneNumber = detailedOrder.User.PhoneNumber,
                VerificationCode = detailedOrder.VerificationCode,
                Status = detailedOrder.Status,
                TotalPrice=detailedOrder.orderItems.Sum(x => x.Quantity * x.Product.Price),
                Products = detailedOrder.orderItems.Select(x => x.ToItemModel()).ToList()
            };
        }


    }
}
