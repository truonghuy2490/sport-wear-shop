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

        /// <summary>
        /// Get paged list of products with filtering
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged list of products</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagingResponseModel<ProductResponseModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResponseModel<ProductResponseModel>>> GetPagedAsync(
            [FromQuery] ProductQueryModel query,
            CancellationToken cancellationToken)
        {
            var result = await _productService.GetPagedAsync(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Product details</returns>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ProductDetailResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDetailResponseModel>> GetByIdAsync(
            long id,
            CancellationToken cancellationToken)
        {
            var result = await _productService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get product by slug
        /// </summary>
        /// <param name="slug">Product slug</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Product details</returns>
        [HttpGet("slug/{slug}")]
        [ProducesResponseType(typeof(ProductDetailResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDetailResponseModel>> GetBySlugAsync(
            string slug,
            CancellationToken cancellationToken)
        {
            var result = await _productService.GetBySlugAsync(slug, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="request">Product creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created product</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProductResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ProductResponseModel>> CreateAsync(
            [FromBody] CreateProductRequestModel request,
            CancellationToken cancellationToken)
        {
            var result = await _productService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = result.ProductId }, result);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="request">Updated product data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated product</returns>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(ProductResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ProductResponseModel>> UpdateAsync(
            long id,
            [FromBody] UpdateProductRequestModel request,
            CancellationToken cancellationToken)
        {
            var result = await _productService.UpdateAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Delete a product (soft delete by default)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="softDelete">If true, soft delete (mark as inactive); if false, hard delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content</returns>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(
            long id,
            [FromQuery] bool softDelete = true,
            CancellationToken cancellationToken = default)
        {
            await _productService.DeleteAsync(id, softDelete, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Update product status
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="status">New status (Active, Inactive, Draft)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated product</returns>
        [HttpPatch("{id:long}/status")]
        [ProducesResponseType(typeof(ProductResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponseModel>> UpdateStatusAsync(
            long id,
            [FromBody] UpdateStatusRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _productService.UpdateStatusAsync(id, request.Status, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get products by brand
        /// </summary>
        /// <param name="brandId">Brand ID</param>
        /// <param name="limit">Maximum number of products to return</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of products by brand</returns>
        [HttpGet("brand/{brandId}")]
        [ProducesResponseType(typeof(IEnumerable<ProductResponseModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductResponseModel>>> GetProductsByBrandAsync(
            int brandId,
            [FromQuery] int limit = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _productService.GetProductsByBrandAsync(brandId, limit, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get related products
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="limit">Maximum number of related products to return</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of related products</returns>
        [HttpGet("{id:long}/related")]
        [ProducesResponseType(typeof(IEnumerable<ProductResponseModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductResponseModel>>> GetRelatedProductsAsync(
            long id,
            [FromQuery] int limit = 5,
            CancellationToken cancellationToken = default)
        {
            var result = await _productService.GetRelatedProductsAsync(id, limit, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Check if product exists by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if exists, false otherwise</returns>
        [HttpGet("{id:long}/exists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ExistsAsync(
            long id,
            CancellationToken cancellationToken = default)
        {
            var result = await _productService.ExistsAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Check if product exists by code
        /// </summary>
        /// <param name="productCode">Product code</param>
        /// <param name="excludeId">Optional product ID to exclude</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if exists, false otherwise</returns>
        [HttpGet("code/{productCode}/exists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ExistsByCodeAsync(
            string productCode,
            [FromQuery] long? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _productService.ExistsByCodeAsync(productCode, excludeId, cancellationToken);
            return Ok(result);
        }
    }
}