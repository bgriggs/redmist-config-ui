using BigMission.ChannelManagement.Shared;
using BigMission.RedMist.Config.Shared;
using BigMission.RedMist.Config.Shared.CanBus;
using System;

namespace BigMission.RedMist.Config.UI;

public class DriverSyncConfigurationProvider : IDriverSyncConfigurationProvider
{
    private readonly MasterDriverSyncConfig config;
    public event Action? ConfigurationLoaded;

    public DriverSyncConfigurationProvider()
    {
        config = new MasterDriverSyncConfig();

        config.ChannelConfig.ChannelMappings.Add(new ChannelMappingDto { Id = 1, Name = "Channel 1" });
        config.ChannelConfig.ChannelMappings.Add(new ChannelMappingDto { Id = 2, Name = "Channel 2", IsReserved = true });
        config.ChannelConfig.ChannelMappings.Add(new ChannelMappingDto { Id = 3, Name = "Channel 3 Test", IsReserved = true });

        config.CanBusConfigs.Add(new CanBusConfigDto { InterfaceName = "can0", BitRate = 1000000 });
        config.CanBusConfigs.Add(new CanBusConfigDto { InterfaceName = "can1", BitRate = 100000, SilentOnCanBus = true });

        config.CanBusConfigs[0].Messages.Add(new CanMessageConfigDto { IsEnabled = true, CanId = 0x01, Length = 8 });
        config.CanBusConfigs[0].Messages[0].ChannelAssignments.Add(new CanChannelAssignmentConfigDto { ChannelId = 1, Offset = 0, Length = 2 });
        config.CanBusConfigs[0].Messages[0].ChannelAssignments.Add(new CanChannelAssignmentConfigDto { ChannelId = 2, Offset = 2, Length = 4 });
        config.CanBusConfigs[0].Messages.Add(new CanMessageConfigDto { CanId = 0xFF1, Length = 6 });
    }

    

    public MasterDriverSyncConfig GetConfiguration()
    {
        return config;
    }
}
