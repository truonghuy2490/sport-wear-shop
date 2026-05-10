using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.InventoryModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
using SportWearShop.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SportWearShop.BusinessLogics.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(
        IUnitOfWork unitOfWork,
        ILogger<InventoryService> logger
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;        
    }

    public async Task<InventoryStockResponseModel> GetStockByVariantIdAsync(
        long productVariantId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation(
            "Retrieving inventory stock. ProductVariantId={ProductVariantId}",
            productVariantId);

        

        var stock = await _unitOfWork.InventoryStocks.FirstOrDefaultAsync(
            predicate: s => s.ProductVariantId == productVariantId 
                                && s.ProductVariant.Status == ProductVariantStatus.Active, // GPT said can use this without including XD
            selector: s => new InventoryStockResponseModel
            {
                ProductVariantId = s.ProductVariantId,
                Sku = s.ProductVariant.Sku,
                QuantityOnHand = s.QuantityOnHand,
                QuantityReserved = s.QuantityReserved,
                UpdatedAtUtc = s.UpdatedAtUtc
            },
            asNoTracking: true,
            cancellationToken: cancellationToken
            // ,includes: new Expression<Func<Repositories.Entities.InventoryStock, object>> []
            // {
            //     s => s.ProductVariant
            // }
        );
        if (stock == null)
        {
            _logger.LogWarning(
                "Inventory stock not found. ProductVariantId={ProductVariantId}",
                productVariantId);

            throw new NotFoundException(
                $"Inventory stock for product variant ID {productVariantId} was not found.");
        }

        return stock;
    }

    public async Task<List<InventoryMovementResponseModel>> GetMovementsByVariantIdAsync(
        long productVariantId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation(
            "Retrieving inventory movements. ProductVariantId={ProductVariantId}",
            productVariantId);
        var isVariantExist = await _unitOfWork.ProductVariants.AnyAsync(
            predicate: pv => pv.ProductVariantId == productVariantId
                                && pv.Status == ProductVariantStatus.Active,
            cancellationToken: cancellationToken
        );

        if (!isVariantExist)
        {
            _logger.LogWarning(
                "Get inventory movements failed. Product variant not found. ProductVariantId={ProductVariantId}",
                productVariantId);

            throw new NotFoundException(
                $"Product variant with ID {productVariantId} was not found.");
        }

        var movements = await _unitOfWork.InventoryMovements.FindAsync(
            filter: m => m.ProductVariantId == productVariantId,
            selector: m => new InventoryMovementResponseModel
            {
                InventoryMovementId = m.InventoryMovementId,
                ProductVariantId = m.ProductVariantId,
                Sku = m.ProductVariant.Sku,
                MovementType = m.MovementType.ToString(),
                Quantity = m.Quantity,
                ReferenceType = m.ReferenceType.ToString(),
                ReferenceId = m.ReferenceId,
                Note = m.Note,
                CreatedAtUtc = m.CreatedAtUtc
            },
            asNoTracking: true,
            sortBy: m => m.CreatedAtUtc,
            ascending: true,
            includes: new Expression<Func<InventoryMovement, object>>[]
            {
                m => m.ProductVariant
            }
        );

        return movements;
    }

    // Stock Movement by Staff: Stock In, Stock Out
    public async Task<InventoryStockResponseModel> StockInAsync(
        StockInRequestModel request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Stock in inventory. ProductVariantId={ProductVariantId}, Quantity={Quantity}, StaffId={StaffId}",
            request.ProductVariantId,
            request.Quantity,
            request.StaffId);

        if (request.Quantity <= 0)
        {
            _logger.LogWarning("Stock in inventory. Quantity must be greater than zero.");
            throw new BadRequestException("Quantity must be greater than zero.");
        }

        var stock = await GetTrackedStockAsync(request.ProductVariantId, cancellationToken);

        stock.QuantityOnHand += request.Quantity;
        stock.UpdatedAtUtc = DateTime.UtcNow;
        
        var movement = new InventoryMovement
        {
            ProductVariantId = request.ProductVariantId,
            MovementType = InventoryMovementType.StockIn, // Stock In
            Quantity = request.Quantity,
            ReferenceType = InventoryReferenceType.Staff, // By Staff
            ReferenceId = request.StaffId,
            Note = request.Note,
            CreatedAtUtc = DateTime.UtcNow
        };

        _unitOfWork.InventoryStocks.Update(stock);
        await _unitOfWork.InventoryMovements.AddAsync(movement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetStockByVariantIdAsync(request.ProductVariantId, cancellationToken);
    }

    public async Task<InventoryStockResponseModel> StockOutAsync(
        StockOutRequestModel request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Stock out inventory. ProductVariantId={ProductVariantId}, Quantity={Quantity}, StaffId={StaffId}",
            request.ProductVariantId,
            request.Quantity,
            request.StaffId);

        if (request.Quantity <= 0)
        {
            _logger.LogWarning("Stock out inventory. Quantity must be greater than zero.");
            throw new BadRequestException("Quantity must be greater than zero.");
        }

        var stock = await GetTrackedStockAsync(request.ProductVariantId, cancellationToken);

        var availableStock = stock.QuantityOnHand - stock.QuantityReserved;

        if (availableStock < request.Quantity)
        {
            throw new ConflictException("Not enough available stock.");
        }

        stock.QuantityOnHand -= request.Quantity;
        stock.UpdatedAtUtc = DateTime.UtcNow;

        var movement = new InventoryMovement
        {
            ProductVariantId = request.ProductVariantId,
            MovementType = InventoryMovementType.StockOut, // Stock Out
            Quantity = request.Quantity,
            ReferenceType = InventoryReferenceType.Staff, // By Staff
            ReferenceId = request.StaffId,
            Note = request.Note,
            CreatedAtUtc = DateTime.UtcNow
        };

        _unitOfWork.InventoryStocks.Update(stock);
        await _unitOfWork.InventoryMovements.AddAsync(movement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetStockByVariantIdAsync(request.ProductVariantId, cancellationToken);
    }

    // Stock Movement by Order: how can use it ?? ><
    public async Task<InventoryStockResponseModel> ReserveAsync(
        ReserveStockRequestModel request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Reserving inventory stock. ProductVariantId={ProductVariantId}, Quantity={Quantity}, OrderId={OrderId}",
            request.ProductVariantId,
            request.Quantity,
            request.OrderId);

        if (request.Quantity <= 0)
        {
            _logger.LogWarning("Stock Reserve inventory. Quantity must be greater than zero.");
            throw new BadRequestException("Quantity must be greater than zero.");
        }
        var stock = await GetTrackedStockAsync(request.ProductVariantId, cancellationToken);

        var availableStock = stock.QuantityOnHand - stock.QuantityReserved;

        if (availableStock < request.Quantity)
        {
            _logger.LogWarning("Stock Reserve inventory. Not enough available stock to reserve.");
            throw new ConflictException("Not enough available stock to reserve.");
        }

        stock.QuantityReserved += request.Quantity;
        stock.UpdatedAtUtc = DateTime.UtcNow;

        var movement = new InventoryMovement{
            ProductVariantId = request.ProductVariantId,
            MovementType = InventoryMovementType.Reserve, // Reserve Stock
            Quantity = request.Quantity,
            ReferenceType = InventoryReferenceType.Order, // By Order
            ReferenceId = request.OrderId,
            Note = request.Note,
        };

        _unitOfWork.InventoryStocks.Update(stock);
        await _unitOfWork.InventoryMovements.AddAsync(movement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetStockByVariantIdAsync(request.ProductVariantId, cancellationToken);
    }

    public async Task<InventoryStockResponseModel> ReleaseAsync(
        ReleaseStockRequestModel request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Releasing reserved inventory stock. ProductVariantId={ProductVariantId}, Quantity={Quantity}, OrderId={OrderId}",
            request.ProductVariantId,
            request.Quantity,
            request.OrderId);

        if (request.Quantity <= 0)
        {
            _logger.LogWarning("Releasing reserved inventory stock. Quantity must be greater than zero.");
            throw new BadRequestException("Quantity must be greater than zero.");
        }
        var stock = await GetTrackedStockAsync(request.ProductVariantId, cancellationToken);

        if (stock.QuantityReserved < request.Quantity)
        {
            _logger.LogWarning("Releasing reserved inventory stock. Cannot release more than reserved quantity.");
            throw new ConflictException("Cannot release more than reserved quantity.");
        }
        stock.QuantityReserved -= request.Quantity;
        stock.UpdatedAtUtc = DateTime.UtcNow;

        var movement = new InventoryMovement
        {
            ProductVariantId = request.ProductVariantId,
            MovementType = InventoryMovementType.Release, // Release Stock
            Quantity = request.Quantity,
            ReferenceType = InventoryReferenceType.Order, // By Order 
            ReferenceId = request.OrderId,
            Note = request.Note
        };
        
        _unitOfWork.InventoryStocks.Update(stock);
        await _unitOfWork.InventoryMovements.AddAsync(movement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetStockByVariantIdAsync(request.ProductVariantId, cancellationToken);

    }

    public async Task<InventoryStockResponseModel> SoldAsync(
        SoldStockRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Confirming sold inventory stock. ProductVariantId={ProductVariantId}, Quantity={Quantity}, OrderId={OrderId}",
            request.ProductVariantId,
            request.Quantity,
            request.OrderId);

        if (request.Quantity <= 0)
        {
            _logger.LogWarning("Confirming sold inventory stock. Quantity must be greater than zero.");
            throw new BadRequestException("Quantity must be greater than zero.");
        }

        var stock = await GetTrackedStockAsync(request.ProductVariantId, cancellationToken);

        if (stock.QuantityReserved < request.Quantity)
        {
            _logger.LogWarning("Confirming sold inventory stock. Cannot sell more than reserved quantity.");
            throw new ConflictException("Cannot sell more than reserved quantity.");
        }

        if (stock.QuantityOnHand < request.Quantity)
        {
            _logger.LogWarning("Confirming sold inventory stock. Not enough stock on hand.");
            throw new ConflictException("Not enough stock on hand.");
        }

        stock.QuantityReserved -= request.Quantity;
        stock.QuantityOnHand -= request.Quantity;
        stock.UpdatedAtUtc = DateTime.UtcNow;

        var movement = new InventoryMovement
        {
            ProductVariantId = request.ProductVariantId,
            MovementType = InventoryMovementType.Sold, // Sold Stock
            Quantity = request.Quantity,
            ReferenceType = InventoryReferenceType.Order, // By Order
            ReferenceId = request.OrderId,
            Note = request.Note
        };

        _unitOfWork.InventoryStocks.Update(stock);
        await _unitOfWork.InventoryMovements.AddAsync(movement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetStockByVariantIdAsync(request.ProductVariantId, cancellationToken);
    }

    // --- Helper ---
    private async Task<InventoryStock> GetTrackedStockAsync(
        long productVariantId,
        CancellationToken cancellationToken = default
    )
    {
        var stock = await _unitOfWork.InventoryStocks.FirstOrDefaultAsync(
            predicate: stock => stock.ProductVariantId == productVariantId
                                    && stock.ProductVariant.Status == ProductVariantStatus.Active,
            selector: stock => stock,
            asNoTracking: false,
            cancellationToken: cancellationToken
        );

        if (stock == null)
        {
            _logger.LogWarning(
                "Inventory stock for product variant ID ProductVariantId={ProductVariantId} was not found.", 
                productVariantId);
            throw new NotFoundException(
                $"Inventory stock for product variant ID {productVariantId} was not found.");
        }

        return stock;
    }

}

