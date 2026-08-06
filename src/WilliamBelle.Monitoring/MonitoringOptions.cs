namespace WilliamBelle.Monitoring;

/// <summary>
/// Where to report to, and as whom. The application id and signing key are
/// issued by William Belle LLC; keep the key in configuration or a secret
/// store, never in source.
/// </summary>
public class MonitoringOptions
{
    /// <summary>Ingest endpoint, e.g. https://portal.williambelle.co/ingest/sensor.</summary>
    public required string IngestUrl { get; set; }
    /// <summary>The monitored app's id, issued by William Belle LLC.</summary>
    public required string AppId { get; set; }
    /// <summary>Per-app signing key, issued by William Belle LLC. Store in Key Vault
    /// or environment configuration, never in source.</summary>
    public required string SigningKey { get; set; }
    /// <summary>How often a snapshot is reported. Default: 24 hours.</summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(24);
}
