using Microsoft.EntityFrameworkCore.Storage;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Repositories.UnitOfWorks;

public interface IUnitOfWork : IDisposable
{
    IBaseRepository<Product> Products { get; }
    IBaseRepository<Brand> Brands { get; }
    IBaseRepository<Category> Categories { get; }
    IBaseRepository<ProductVariant> ProductVariants { get; }
    IBaseRepository<ProductImage> ProductImages { get; }
    IBaseRepository<Cart> Carts { get; }
    IBaseRepository<CartItem> CartItems { get; }
    IBaseRepository<OrderHeader> OrderHeaders { get; }
    IBaseRepository<OrderItem> OrderItems { get; }
    IBaseRepository<ProductRating> ProductRatings { get; }
    IBaseRepository<UserAddress> UserAddresses { get; }
    IBaseRepository<PaymentTransaction> PaymentTransactions { get; }
    IBaseRepository<InventoryStock> InventoryStocks { get; }
    IBaseRepository<InventoryMovement> InventoryMovements { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken = default);
}