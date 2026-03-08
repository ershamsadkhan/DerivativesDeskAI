namespace DerivativesDesk.Core.Models;

public class RolloverPeriod
{
    public int RolloverId { get; set; }
    public int FromContractId { get; set; }
    public int ToContractId { get; set; }
    public DateOnly RollStartDate { get; set; }
    public DateOnly RollDate { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }

    // Joined fields (populated via query)
    public string FromSymbol { get; set; } = string.Empty;
    public string ToSymbol { get; set; } = string.Empty;

    public string ToTextChunk() =>
        $"[RolloverPeriod] From: {FromSymbol} → To: {ToSymbol} | " +
        $"RollWindow: {RollStartDate:yyyy-MM-dd} to {RollDate:yyyy-MM-dd} | " +
        $"Reason: {Reason ?? "Scheduled quarterly roll"}";
}
