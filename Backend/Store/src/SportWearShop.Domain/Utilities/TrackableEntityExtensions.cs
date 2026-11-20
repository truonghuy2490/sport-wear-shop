using SportWearShop.Domain.Entities;

namespace SportWearShop.Domain.Utilities;

public static class TrackableEntityExtensions
{
    public static void SetUpdatedDateTime(
        this IUpdatedAtUtc entity,
        DateTime? updatedTimestampUtc = null)
    {
        entity.UpdatedAtUtc = updatedTimestampUtc ?? DateTime.UtcNow;
    }
}
