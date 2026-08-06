using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WilliamBelle.Monitoring;

/// <summary>
/// Collects a snapshot of the running application on a schedule and POSTs it,
/// HMAC-signed, to the ingest endpoint. Failures are logged and
/// swallowed — the sensor must never affect the host application.
/// </summary>
public sealed class SensorService(
    IOptions<MonitoringOptions> options,
    IHttpClientFactory httpClientFactory,
    IHostEnvironment environment,
    ILogger<SensorService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var opts = options.Value;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ReportAsync(opts, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "Monitoring report failed; will retry next interval.");
            }
            await Task.Delay(opts.Interval, stoppingToken);
        }
    }

    private async Task ReportAsync(MonitoringOptions opts, CancellationToken ct)
    {
        var snapshot = Collect(opts.AppId, environment.EnvironmentName);
        var json = JsonSerializer.Serialize(snapshot);
        var signature = PayloadSigner.Sign(json, opts.SigningKey);

        var client = httpClientFactory.CreateClient("bellwether-sensor");
        using var request = new HttpRequestMessage(HttpMethod.Post, opts.IngestUrl)
        {
            Content = JsonContent.Create(new { payload = json, signature }),
        };
        var response = await client.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        logger.LogInformation("Monitoring snapshot reported ({Count} packages).",
            snapshot.Packages.Count);
    }

    internal static SensorSnapshot Collect(string appId, string environmentName) => new()
    {
        AppId = appId,
        RuntimeVersion = RuntimeInformation.FrameworkDescription,
        EnvironmentName = environmentName,
        Packages = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetName())
            .Where(n => n.Name is not null)
            .Select(n => new SensorSnapshot.PackageInfo
            {
                Name = n.Name!,
                Version = n.Version?.ToString() ?? "unknown",
            })
            .OrderBy(p => p.Name)
            .ToList(),
    };
}
