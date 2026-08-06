namespace WilliamBelle.Monitoring;

/// <summary>
/// Everything the sensor reports. This is the complete data surface: the
/// sensor is read-only and one-way — it accepts no inbound commands, no
/// remote configuration, and collects no logs or request payloads.
/// </summary>
public class SensorSnapshot
{
    public required string AppId { get; set; }
    /// <summary>e.g. ".NET 10.0.4" — the runtime servicing level actually running.</summary>
    public required string RuntimeVersion { get; set; }
    /// <summary>ASPNETCORE_ENVIRONMENT as the process sees it — catches
    /// Development running in production.</summary>
    public required string EnvironmentName { get; set; }
    /// <summary>Loaded assemblies with versions — what is actually deployed,
    /// as opposed to what the repository manifest says.</summary>
    public required List<PackageInfo> Packages { get; set; }
    public DateTimeOffset CollectedAt { get; set; } = DateTimeOffset.UtcNow;

    public class PackageInfo
    {
        public required string Name { get; set; }
        public required string Version { get; set; }
    }
}
