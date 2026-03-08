using DerivativesDesk.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DerivativesDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EtlController(IEtlPipeline pipeline, ILogger<EtlController> logger) : ControllerBase
{
    private static EtlResult? _lastResult;

    /// <summary>Manually trigger a vector DB refresh from SQL Server.</summary>
    [HttpPost("trigger")]
    public async Task<IActionResult> TriggerAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Manual ETL trigger requested.");
        _lastResult = await pipeline.RunAsync(cancellationToken);

        if (_lastResult.Error is null)
            return Ok(new
            {
                status = "success",
                contracts = _lastResult.ContractsProcessed,
                rollovers = _lastResult.RolloversProcessed,
                orders = _lastResult.OrdersProcessed,
                durationMs = (long)_lastResult.Duration.TotalMilliseconds
            });

        return StatusCode(500, new { status = "failed", error = _lastResult.Error });
    }

    /// <summary>Get the status of the last ETL run.</summary>
    [HttpGet("status")]
    public IActionResult Status()
    {
        if (_lastResult is null)
            return Ok(new { status = "never_run" });

        return Ok(new
        {
            status = _lastResult.Error is null ? "success" : "failed",
            contracts = _lastResult.ContractsProcessed,
            rollovers = _lastResult.RolloversProcessed,
            orders = _lastResult.OrdersProcessed,
            durationMs = (long)_lastResult.Duration.TotalMilliseconds,
            error = _lastResult.Error
        });
    }
}
