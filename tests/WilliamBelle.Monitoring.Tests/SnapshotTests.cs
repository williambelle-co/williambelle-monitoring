using WilliamBelle.Monitoring;

namespace WilliamBelle.Monitoring.Tests;

/// <summary>
/// What the agent collects is a promise made in the README. These tests hold
/// it to that promise: the snapshot describes the running process and nothing
/// about the people using it.
/// </summary>
public class SnapshotTests
{
    private static MonitoringSnapshot Collect() =>
        MonitoringService.Collect("test-app", "Production");

    [Fact]
    public void A_snapshot_describes_the_running_process()
    {
        var snapshot = Collect();

        Assert.Equal("test-app", snapshot.AppId);
        Assert.Equal("Production", snapshot.EnvironmentName);
        Assert.False(string.IsNullOrWhiteSpace(snapshot.RuntimeVersion));
        Assert.NotEmpty(snapshot.Packages);
    }

    [Fact]
    public void Packages_are_reported_with_the_versions_actually_loaded()
    {
        var snapshot = Collect();

        Assert.All(snapshot.Packages, p =>
        {
            Assert.False(string.IsNullOrWhiteSpace(p.Name));
            Assert.False(string.IsNullOrWhiteSpace(p.Version));
        });

        // The point of the whole package: what is running, not what a
        // repository claims should be running.
        Assert.Contains(snapshot.Packages, p => p.Name.Contains("WilliamBelle.Monitoring"));
    }

    [Fact]
    public void Packages_are_ordered_so_snapshots_can_be_compared()
    {
        var names = Collect().Packages.Select(p => p.Name).ToList();

        Assert.Equal(names.OrderBy(n => n, StringComparer.Ordinal).ToList(), names);
    }

    [Fact]
    public void Collection_is_timestamped_in_utc()
        => Assert.Equal(TimeSpan.Zero, Collect().CollectedAt.Offset);
}
