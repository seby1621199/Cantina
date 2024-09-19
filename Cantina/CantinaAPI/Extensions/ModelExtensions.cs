using BusinessLogic.Models;
using CantinaAPI.Models;

namespace CantinaAPI.Extensions
{
    public static class ModelExtensions
    {
        public static ProductDomain ToProductDomain(this ProductRequest productModel)
        {
            return new ProductDomain
            {
                Name = productModel.Name,
                Description = productModel.Description,
                Price = productModel.Price,
                Stock = productModel.Stock,
                Active = productModel.Active,
                Unit = productModel.Unit
            };
        }

        public static KitchenOrder ToKitchenOrder(this OrderModel orderModel)
        {
            return new KitchenOrder
            {
                Id = orderModel.Id,
                Address= orderModel.Location,
            };
        }
        public static ProductRequest ToProductRequest(this ProductDomain productModel)
        {
            return new ProductRequest
            {
                Name = productModel.Name,
                Description = productModel.Description,
                Price = productModel.Price,
                Stock = productModel.Stock,
                Active = productModel.Active,
                Unit = productModel.Unit
            };
        }

        public static IdentifiableProduct ToIdentifiableProduct(this ProductDomain productModel)
        {
            return new IdentifiableProduct
            {
                Id = productModel.Id,
                Name = productModel.Name,
                Description = productModel.Description,
                Price = productModel.Price,
                Stock = productModel.Stock,
                Active = productModel.Active,
                Unit = productModel.Unit

            };
        }

        public static Items ToDomain(this OrderItemRequest orderItemRequest)
        {
            return new Items
            {
                Id = orderItemRequest.Id,
                Quantity = orderItemRequest.Quantity
            };
        }

        public static CantinaAPI.Models.DetailedOrder ToAPI(this BusinessLogic.Models.OrderModel orderModel)
        {
            return new DetailedOrder
            {
                Id = orderModel.Id,
                OrderDate = orderModel.OrderDate,
                orderItems = orderModel.orderItems,
                Status = orderModel.Status
            };
        }

        public static KitchenOrder ToKitchenOrder(this OrderUserDetails orderModel)
        {
            return new KitchenOrder
            {
                Id = orderModel.Id,
                Address = orderModel.Address,
                Name = orderModel.Name,
                PhoneNumber = orderModel.PhoneNumber,
                VerificationCode = orderModel.VerificationCode,
                Status = orderModel.Status,
                TotalPrice=orderModel.TotalPrice,
                Products = orderModel.Products.Where(x => x.Quantity > 0).Select(x => x.ToKitchenProduct()).ToList()
            };
        }

        public static KitchenProduct ToKitchenProduct(this ItemModel item)
        {
            return new KitchenProduct
            {
                Name = item.ProductName,
                Quantity = item.Quantity
            };
        }

    }
}
