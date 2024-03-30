using BigMission.ChannelManagement.Shared;
using BigMission.RedMist.Config.Shared;
using BigMission.RedMist.Config.Shared.CanBus;
using BigMission.RedMist.Config.UI.Shared.CanBus;
using BigMission.RedMist.Config.UI.Shared.Channels;
using BigMission.RedMist.Config.UI.Shared.General;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BigMission.RedMist.Config.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private MasterDeviceConfigDto? data;
    public MasterDeviceConfigDto? Data
    {
        get => data;
        set
        {
            data = value;
            GeneralViewModel = new GeneralViewModel { Data = data?.GeneralConfig ?? new() };
            ChannelsViewModel = new ChannelsViewModel { Data = data?.ChannelConfig ?? new() };
            CanBusViewModel = new CanBusViewModel { Data = data?.CanBusConfig ?? new() };
        }
    }

    [ObservableProperty]
    private GeneralViewModel? generalViewModel;
    [ObservableProperty]
    private ChannelsViewModel? channelsViewModel;
    [ObservableProperty]
    private CanBusViewModel? canBusViewModel;

    public MainViewModel()
    {
        Data = new MasterDeviceConfigDto();

        // TODO: Remove test data
        ChannelsViewModel?.Channels.Add(new ChannelMappingRowViewModel { Data = new ChannelMappingDto { Name = "test" }, ParentVm = ChannelsViewModel });
        ChannelsViewModel?.Channels.Add(new ChannelMappingRowViewModel { Data = new ChannelMappingDto { Name = "sefasrdf", IsReserved = true }, ParentVm = ChannelsViewModel });

        CanBusViewModel?.Messages.Add(new CanMessageViewModel { Data = new CanMessageConfigDto { CanId = 0x01 } });
    }
}
