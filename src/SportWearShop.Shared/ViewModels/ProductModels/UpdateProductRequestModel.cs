using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Shared.ViewModels.ProductModels
{
    public class UpdateProductRequestModel
    {
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? Gender { get; set; }
        public string? BaseMaterial { get; set; }
        public string? Status { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
    }
    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}
