namespace SportWearShop.Domain.Entities;

public sealed class ProductRating : IUpdatedAtUtc
{
    public long ProductRatingId { get; set; }
    public long ProductId { get; set; }
    public long CustomerId { get; set; }
    public int RatingValue { get; set; }
    public string? ReviewText { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
