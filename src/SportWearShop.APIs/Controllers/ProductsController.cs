using Microsoft.AspNetCore.Mvc;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;

namespace SportWearShop.APIs.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: api/products
        [HttpGet]
        [ProducesResponseType(typeof(PagingResponseModel<ProductResponseModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResponseModel<ProductResponseModel>>> GetPagedAsync(
            [FromQuery] ProductQueryModel query,
            CancellationToken cancellationToken)
        {
            var result = await _productService.GetPagedAsync(query, cancellationToken);
            return Ok(result);
        }

        // GET: api/products/5
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ProductDetailResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDetailResponseModel>> GetByIdAsync(
            long id,
            CancellationToken cancellationToken)
        {
            var result = await _productService.GetByIdAsync(id, cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    Message = $"Product with id = {id} was not found."
                });
            }

            return Ok(result);
        }

        // POST: api/products
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAsync(
            [FromBody] CreateProductRequestModel request,
            CancellationToken cancellationToken)
        {
            try
            {
                var productId = await _productService.CreateAsync(request, cancellationToken);

                return CreatedAtAction(
                    nameof(GetByIdAsync),
                    new { id = productId },
                    new
                    {
                        Message = "Product created successfully.",
                        ProductId = productId
                    });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business validation error while creating product.");
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        // PUT: api/products/5
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateAsync(
            long id,
            [FromBody] UpdateProductRequestModel request,
            CancellationToken cancellationToken)
        {
            try
            {
                var updated = await _productService.UpdateAsync(id, request, cancellationToken);

                if (!updated)
                {
                    return NotFound(new
                    {
                        Message = $"Product with id = {id} was not found."
                    });
                }

                return Ok(new
                {
                    Message = "Product updated successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business validation error while updating product {ProductId}.", id);
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        // DELETE: api/products/5
        [HttpDelete("{id:long}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAsync(
            long id,
            CancellationToken cancellationToken)
        {
            var deleted = await _productService.DeleteAsync(id, cancellationToken);

            if (!deleted)
            {
                return NotFound(new
                {
                    Message = $"Product with id = {id} was not found."
                });
            }

            return Ok(new
            {
                Message = "Product deleted successfully."
            });
        }
    }
}