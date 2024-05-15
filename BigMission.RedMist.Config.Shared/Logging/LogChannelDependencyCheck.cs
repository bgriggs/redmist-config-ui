using BigMission.RedMist.Config.Shared.Channels;

namespace BigMission.RedMist.Config.Shared.Logging;

/// <summary>
/// Provides what channels are being used in logging.
/// </summary>
public class LogChannelDependencyCheck : IChannelDependencyCheck
{
    private readonly IDriverSyncConfigurationProvider configurationProvider;

    public LogChannelDependencyCheck(IDriverSyncConfigurationProvider configurationProvider)
    {
        this.configurationProvider = configurationProvider;
    }

    public IEnumerable<ChannelDependency> GetChannelAssignments() => [];

    /// <summary>
    /// Channels that are being used by logging.
    /// </summary>
    public IEnumerable<ChannelDependency> GetChannelDependencies()
    {
        var config = configurationProvider.GetConfiguration();
        foreach (var log in config.LoggingConfig.LogEntries)
        {
            var ca = new ChannelDependency(log.ChannelId, "Logging", "Device Logging");
            yield return ca;
        }
    }
}
