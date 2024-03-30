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
        }
    }

    [ObservableProperty]
    private GeneralViewModel? generalViewModel;
    [ObservableProperty]
    private ChannelsViewModel? channelsViewModel;
    [ObservableProperty]
    private CanBusViewModel? canBusViewModel;

    public MainViewModel(CanBusViewModel canBusViewModel, ChannelsViewModel channelsViewModel, GeneralViewModel generalViewModel)
    {
        Data = new MasterDeviceConfigDto();
        CanBusViewModel = canBusViewModel;
        ChannelsViewModel = channelsViewModel;
        GeneralViewModel = generalViewModel;

        // TODO: Remove test data
        ChannelsViewModel?.Channels.Add(new ChannelMappingRowViewModel { Data = new ChannelMappingDto { Name = "test" }, ParentVm = ChannelsViewModel });
        ChannelsViewModel?.Channels.Add(new ChannelMappingRowViewModel { Data = new ChannelMappingDto { Name = "sefasrdf", IsReserved = true }, ParentVm = ChannelsViewModel });

        CanBusViewModel.Messages.Add(new CanMessageViewModel { Data = new CanMessageConfigDto { CanId = 0x01 } });
    }
}
