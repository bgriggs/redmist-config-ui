using BigMission.Avalonia.Utilities;
using BigMission.RedMist.Config.Shared;
using BigMission.RedMist.Config.Shared.CanBus;
using BigMission.RedMist.Config.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;

namespace BigMission.RedMist.Config.UI.Shared.CanBus;

/// <summary>
/// View Model for an overall CAN bus configuration, such as can0.
/// </summary>
public partial class CanBusViewModel : ObservableObject
{
    public CanBusConfigDto Data { get; }

    [ObservableProperty]
    private string name = string.Empty;
    private readonly ChannelProvider channelProvider;

    public LargeObservableCollection<CanMessageViewModel> Messages { get; } = [];

    public CanBusViewModel(CanBusConfigDto dto, ChannelProvider channelProvider)
    {
        Messages.SetRange(dto.Messages.Select(m => new CanMessageViewModel(m, this, channelProvider)));
        Messages.CollectionChanged += Messages_CollectionChanged;
        Data = dto;
        this.channelProvider = channelProvider;
    }

    private void Messages_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Data.Messages.Clear();
        Data.Messages.AddRange(Messages.Select(d => d.Data));
    }

    public async Task AddMessageClick()
    {
        var dto = new CanMessageConfigDto();
        var vm = new CanMessageDialogViewModel(dto);
        var result = await DialogHost.Show(vm, "MainDialogHost");
        if (result is CanMessageDialogViewModel)
        {
            var msgVm = new CanMessageViewModel(dto, this, channelProvider);
            Messages.Add(msgVm);
        }
    }
}
