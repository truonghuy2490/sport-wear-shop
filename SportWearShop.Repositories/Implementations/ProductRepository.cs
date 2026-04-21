using Microsoft.Extensions.Logging;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Repositories.Implementations
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context, ILogger<BaseRepository<Product>> logger) : base(context, logger)
        {
        }
    }
}
