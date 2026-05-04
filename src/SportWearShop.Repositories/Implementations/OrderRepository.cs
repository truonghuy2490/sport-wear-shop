using Microsoft.Extensions.Logging;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Repositories.Implementations
{
    public class OrderRepository : BaseRepository<OrderHeader>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }
    }
}
