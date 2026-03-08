namespace DerivativesDesk.Core.Interfaces;

public record EtlResult(int ContractsProcessed, int RolloversProcessed, int OrdersProcessed, TimeSpan Duration, string? Error = null);

public interface IEtlPipeline
{
    Task<EtlResult> RunAsync(CancellationToken cancellationToken = default);
}
