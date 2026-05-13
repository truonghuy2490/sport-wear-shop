using SportWearShop.Repositories.Enums;

namespace SportWearShop.BusinessLogics.ResponseModels.InventoryModels;

public class InventoryMovementQueryRequestModel
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public InventoryMovementType? MovementType { get; set; }

    public InventoryReferenceType? ReferenceType { get; set; }

    public long? ReferenceId { get; set; }

    public int? MinQuantity { get; set; }

    public int? MaxQuantity { get; set; }

    public DateTime? CreatedFromUtc { get; set; }

    public DateTime? CreatedToUtc { get; set; }

    public InventoryMovementSortBy SortBy { get; set; } = InventoryMovementSortBy.CreatedAtUtc;

    public bool IsAscending { get; set; } = false;
}