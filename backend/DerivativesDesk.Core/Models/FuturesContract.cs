namespace DerivativesDesk.Core.Models;

public class FuturesContract
{
    public int ContractId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Exchange { get; set; } = string.Empty;
    public string AssetClass { get; set; } = string.Empty;
    public DateOnly ExpiryDate { get; set; }
    public int LotSize { get; set; }
    public decimal TickSize { get; set; }
    public decimal TickValue { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string ToTextChunk() =>
        $"[FuturesContract] Symbol: {Symbol} | Description: {Description} | " +
        $"Exchange: {Exchange} | AssetClass: {AssetClass} | " +
        $"ExpiryDate: {ExpiryDate:yyyy-MM-dd} | LotSize: {LotSize} | " +
        $"TickSize: {TickSize} | TickValue: {TickValue} | " +
        $"Currency: {Currency} | Status: {Status}";
}
