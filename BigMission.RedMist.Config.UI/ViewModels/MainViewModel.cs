using BigMission.RedMist.Config.Shared;
using BigMission.RedMist.Config.Shared.Channels;
using BigMission.RedMist.Config.UI.Shared.CanBus;
using BigMission.RedMist.Config.UI.Shared.Channels;
using BigMission.RedMist.Config.UI.Shared.General;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BigMission.RedMist.Config.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private GeneralViewModel? generalViewModel;
    [ObservableProperty]
    private ChannelsViewModel? channelsViewModel;
    [ObservableProperty]
    private ObservableCollection<CanBusViewModel> canBusViewModels;
    private readonly IDriverSyncConfigurationProvider configurationProvider;
    private readonly ChannelProvider channelProvider;

    public MainViewModel(IDriverSyncConfigurationProvider configurationProvider, ChannelProvider channelProvider)
    {
        this.configurationProvider = configurationProvider;
        this.channelProvider = channelProvider;
        CanBusViewModels = [];
        InitializeConfiguration();
        configurationProvider.ConfigurationChanged += ConfigurationProvider_ConfigurationChanged;
    }

    private void ConfigurationProvider_ConfigurationChanged()
    {
        InitializeConfiguration();
    }

    private void InitializeConfiguration()
    {
        var config = configurationProvider.GetConfiguration();
        GeneralViewModel = new GeneralViewModel(config.GeneralConfig);
        ChannelsViewModel = new ChannelsViewModel(config.ChannelConfig);

        CanBusViewModels.Clear();
        for (int i = 0; i < config.CanBusConfigs.Count; i++)
        {
            CanBusViewModels.Add(new CanBusViewModel(config.CanBusConfigs[i], channelProvider) { Name = $"CAN {i + 1}" });
        }
    }
}
