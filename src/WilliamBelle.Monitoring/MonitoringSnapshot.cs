namespace WilliamBelle.Monitoring;

/// <summary>
/// Everything the sensor reports. This is the complete data surface: the
/// sensor is read-only and one-way — it accepts no inbound commands, no
/// remote configuration, and collects no logs or request payloads.
/// </summary>
public class MonitoringSnapshot
{
    /// <summary>The monitored application's id, issued by William Belle LLC.</summary>
    public required string AppId { get; set; }
    /// <summary>e.g. ".NET 10.0.4" — the runtime servicing level actually running.</summary>
    public required string RuntimeVersion { get; set; }
    /// <summary>ASPNETCORE_ENVIRONMENT as the process sees it — catches
    /// Development running in production.</summary>
    public required string EnvironmentName { get; set; }
    /// <summary>Loaded assemblies with versions — what is actually deployed,
    /// as opposed to what the repository manifest says.</summary>
    public required List<PackageInfo> Packages { get; set; }
    /// <summary>When the snapshot was taken, in UTC.</summary>
    public DateTimeOffset CollectedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>One loaded assembly and the version actually in memory.</summary>
    public class PackageInfo
    {
        /// <summary>Assembly name.</summary>
        public required string Name { get; set; }

        /// <summary>Version actually loaded, which may differ from the one the repository declares.</summary>
        public required string Version { get; set; }
    }
}
