using BusinessLogic.Services;
using CantinaAPI.Extensions;
using CantinaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CantinaAPI.Controllers;

[ApiController]
[Route("/product/")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductController(IProductService serviceProduct)
    {
        _productService = serviceProduct;
    }

    [HttpPost("add")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddProductAsync([FromBody] ProductRequest productRequest)
    {
        try
        {
            await _productService.AddProduct(productRequest.ToProductDomain());
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetProducts(bool withIds = false, bool active = true)
    {
        try
        {
            var products = await _productService.GetAll();
            if (active)
                products = products.Where(p => p.Active).ToList();
            if (withIds)
            {
                return Ok(products.Select(p => p.ToIdentifiableProduct()));
            }
            return Ok(products.Select(p => p.ToProductRequest()));
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("getById")]
    public async Task<ActionResult<ProductRequest>> GetProductById([FromQuery] int id)
    {
        var products = await _productService.GetAll();
        var product = products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product.ToIdentifiableProduct());
    }

    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductRequest productRequest)
    {
        try
        {
            await _productService.UpdateProduct(id, productRequest.ToProductDomain());
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProductAsync([FromQuery] int id)
    {
        try
        {
            await _productService.DeleteProduct(id);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("updateStock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStock([FromQuery] int id, [FromQuery] int quantity)
    {
        try
        {
            await _productService.UpdateStock(id, quantity);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPatch("changeStatus")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeStatus([FromQuery] int id, [FromQuery] bool status)
    {
        try
        {
            await _productService.ChangeStatus(id, status);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

}
