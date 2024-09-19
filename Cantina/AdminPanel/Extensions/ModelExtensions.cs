using AdminPanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Extensions
{
    public static class ModelExtensions
    {
        public static KitchenProduct ToKitchenProduct(this OrderItemRequest orderItemRequest)
        {
            return new KitchenProduct
            {
                name = "Product "+orderItemRequest.Id.ToString(),
                quantity = orderItemRequest.Quantity
            };
        }

        public static ProductUpdate ToProductUpdateAPI(this ProductModel productModel)
        {
            return new ProductUpdate
            {
                name = productModel.name,
                description = productModel.description,
                price = productModel.price,
                stock=productModel.stock,
                active=productModel.active,
                unit=productModel.unit
            };
        }
    }
}
