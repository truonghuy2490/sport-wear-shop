using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportWearShop.Repositories;
using SportWearShop.Repositories.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SportWearShop.APIs.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }
        // GET: api/product
        [HttpGet]
        public async Task<List<Product>> GetAllAsync()
        {
            var ps = await _context.Products.ToListAsync();
            return ps;
        }

        // GET api/product/5
        [HttpGet("{id}")]
        public async Task<Product> GetAsync(int id)
        {
            var p = await _context.Products.FindAsync(id);
            return p == null ? null : p;
        }

        
    }
}
