namespace DerivativesDesk.Core.Models;

public class Order
{
    public int OrderId { get; set; }
    public int ContractId { get; set; }
    public int PmId { get; set; }
    public string Side { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? FillDate { get; set; }
    public DateTime? CancelDate { get; set; }
    public string? CancelReason { get; set; }
    public string? Notes { get; set; }

    // Joined fields
    public string Symbol { get; set; } = string.Empty;
    public string PmName { get; set; } = string.Empty;
    public string PmType { get; set; } = string.Empty;

    public string ToTextChunk() =>
        $"[Order] OrderId: {OrderId} | Contract: {Symbol} | PM: {PmName} ({PmType}) | " +
        $"Side: {Side} | Status: {Status} | Qty: {Quantity} | Price: {Price:F4} | " +
        $"OrderDate: {OrderDate:yyyy-MM-dd HH:mm}" +
        (Status == "Cancelled" && CancelReason != null ? $" | CancelReason: {CancelReason}" : "") +
        (Notes != null ? $" | Notes: {Notes}" : "");
}
