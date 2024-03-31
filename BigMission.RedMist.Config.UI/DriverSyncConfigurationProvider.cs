using BigMission.ChannelManagement.Shared;
using BigMission.RedMist.Config.Shared;
using BigMission.RedMist.Config.Shared.CanBus;
using System;

namespace BigMission.RedMist.Config.UI;

public class DriverSyncConfigurationProvider : IDriverSyncConfigurationProvider
{
    private MasterDriverSyncConfig config;
    public event Action? ConfigurationChanged;

    public DriverSyncConfigurationProvider()
    {
        config = new MasterDriverSyncConfig();

        config.ChannelConfig.ChannelMappings.Add(new ChannelMappingDto { Id = 1, Name = "Channel 1" });
        config.ChannelConfig.ChannelMappings.Add(new ChannelMappingDto { Id = 2, Name = "Channel 2", IsReserved = true });

        config.CanBusConfigs.Add(new CanBusConfigDto { Id = 1, InterfaceName = "can0" });
        config.CanBusConfigs.Add(new CanBusConfigDto { Id = 2, InterfaceName = "can1" });

        for (int i = 0; i < config.CanBusConfigs.Count; i++)
        {
            config.CanBusConfigs[i].Messages.Add(new CanMessageConfigDto { IsEnabled = true, CanId = 0x01, Length = 8 });
            config.CanBusConfigs[i].Messages[0].ChannelAssignments.Add(new CanChannelAssignmentConfigDto { ChannelId = 1, Offset = 0, Length = 2 });
            config.CanBusConfigs[i].Messages[0].ChannelAssignments.Add(new CanChannelAssignmentConfigDto { ChannelId = 2, Offset = 2, Length = 4 });
            config.CanBusConfigs[i].Messages.Add(new CanMessageConfigDto { CanId = 0xFF1, Length = 6 });
            //CanBusViewModels.Add(new CanBusViewModel(Data.CanBusConfigs[i], configurationProvider) { Name = $"CAN {i + 1}" });
        }
    }

    

    public MasterDriverSyncConfig GetConfiguration()
    {
        return config;
    }
}
