namespace BigMission.RedMist.Config.Shared;

public interface IDriverSyncConfigurationProvider
{
    // Configuration changed event
    event Action ConfigurationChanged;

    MasterDriverSyncConfig GetConfiguration();
}
