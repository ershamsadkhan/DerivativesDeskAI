using DerivativesDesk.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DerivativesDesk.ETL;

public class Worker(
    IEtlPipeline pipeline,
    IConfiguration configuration,
    ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var refreshHour = configuration.GetValue<int>("ETL_REFRESH_HOUR", 6);
        logger.LogInformation("ETL Worker started. Daily refresh scheduled at {Hour:D2}:00 UTC.", refreshHour);

        // Run immediately on startup to populate vector DB
        await RunPipelineAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextRun = now.Date.AddHours(refreshHour);
            if (nextRun <= now) nextRun = nextRun.AddDays(1);

            var delay = nextRun - now;
            logger.LogInformation("Next ETL refresh at {NextRun:yyyy-MM-dd HH:mm} UTC (in {Hours:F1}h).",
                nextRun, delay.TotalHours);

            await Task.Delay(delay, stoppingToken);
            await RunPipelineAsync(stoppingToken);
        }
    }

    private async Task RunPipelineAsync(CancellationToken ct)
    {
        try
        {
            logger.LogInformation("ETL pipeline starting...");
            var result = await pipeline.RunAsync(ct);

            if (result.Error is null)
                logger.LogInformation(
                    "ETL succeeded: {Contracts} contracts, {Rollovers} rollovers, {Orders} orders in {Duration}.",
                    result.ContractsProcessed, result.RolloversProcessed, result.OrdersProcessed, result.Duration);
            else
                logger.LogError("ETL failed: {Error}", result.Error);
        }
        catch (OperationCanceledException) { /* shutdown */ }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in ETL pipeline.");
        }
    }
}
