namespace SportWearShop.Shared.ViewModels.ErrorResponseModels;

public class ErrorResponseModel
{
    public string? Title { get; set; }
    public int Status { get; set; }
    public string? Message { get; set; }
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; }
}