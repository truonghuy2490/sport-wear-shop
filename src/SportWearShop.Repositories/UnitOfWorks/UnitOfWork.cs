using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Implementations;
using SportWearShop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Repositories.UnitOfWorks;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    private IBaseRepository<Product>? _product;
    private IBaseRepository<Brand>? _brands;
    private IBaseRepository<Category>? _categories;
    private IBaseRepository<ProductVariant>? _productVariants;
    private IBaseRepository<ProductImage>? _productImages;
    private ICartRepository? _carts;
    private IBaseRepository<CartItem>? _cartItems;
    private IBaseRepository<OrderHeader>? _orderHeaders;
    private IBaseRepository<OrderItem>? _orderItems;
    private IBaseRepository<ProductRating>? _productRatings;
    private IBaseRepository<UserAddress>? _userAddresses;
    private IBaseRepository<PaymentTransaction>? _paymentTransactions;
    private IBaseRepository<InventoryStock>? _inventoryStocks;
    private IBaseRepository<InventoryMovement>? _inventoryMovements;
   
    

    private ICartRepository _cartRepository;

    public UnitOfWork(
        AppDbContext context,
        ILogger<UnitOfWork> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public IBaseRepository<Product> Products =>
        _product ??= new BaseRepository<Product>(_context);

    public IBaseRepository<Brand> Brands =>
        _brands ??= new BaseRepository<Brand>(_context);

    public IBaseRepository<Category> Categories =>
        _categories ??= new BaseRepository<Category>(_context);

    public IBaseRepository<ProductVariant> ProductVariants =>
        _productVariants ??= new BaseRepository<ProductVariant>(_context);

    public IBaseRepository<ProductImage> ProductImages =>
        _productImages ??= new BaseRepository<ProductImage>(_context);

    public ICartRepository Carts => 
        _carts ??= new CartRepository(_context);

    public IBaseRepository<CartItem> CartItems =>
        _cartItems ??= new BaseRepository<CartItem>(_context);

    public IBaseRepository<OrderHeader> OrderHeaders =>
        _orderHeaders ??= new BaseRepository<OrderHeader>(_context);

    public IBaseRepository<OrderItem> OrderItems =>
        _orderItems ??= new BaseRepository<OrderItem>(_context);

    public IBaseRepository<ProductRating> ProductRatings =>
        _productRatings ??= new BaseRepository<ProductRating>(_context);

    public IBaseRepository<UserAddress> UserAddresses =>
        _userAddresses ??= new BaseRepository<UserAddress>(_context);

    public IBaseRepository<PaymentTransaction> PaymentTransactions =>
        _paymentTransactions ??= new BaseRepository<PaymentTransaction>(_context);

    public IBaseRepository<InventoryStock> InventoryStocks =>
        _inventoryStocks ??= new BaseRepository<InventoryStock>(_context);

    public IBaseRepository<InventoryMovement> InventoryMovements =>
        _inventoryMovements ??= new BaseRepository<InventoryMovement>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Saving changes through UnitOfWork");
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving changes through UnitOfWork");
            throw;
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Beginning database transaction");
        return await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Transaction committed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while committing transaction");

            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackTransactionAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        _logger.LogWarning("Rolling back transaction");
        await transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}