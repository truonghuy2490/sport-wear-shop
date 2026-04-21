using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductRatingModels
{
    public class ProductRatingResponseModel
    {
        public long ProductRatingId { get; set; }

        public long ProductId { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public int RatingValue { get; set; }

        public string? ReviewText { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
