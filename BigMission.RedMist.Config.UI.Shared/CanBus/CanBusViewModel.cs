using BigMission.Avalonia.Utilities;
using BigMission.RedMist.Config.Shared.CanBus;
using BigMission.RedMist.Config.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;
using System.ComponentModel.DataAnnotations;

namespace BigMission.RedMist.Config.UI.Shared.CanBus;

/// <summary>
/// View Model for an overall CAN bus configuration, such as can0.
/// </summary>
public partial class CanBusViewModel : ObservableValidator
{
    public CanBusConfigDto Data { get; }

    [ObservableProperty]
    private string name = string.Empty;
    private readonly ChannelProvider channelProvider;

    [MinLength(3)]
    [MaxLength(50)]
    public string InterfaceName
    {
        get => Data.InterfaceName;
        set { SetProperty(Data.InterfaceName, value, Data, (u, n) => u.InterfaceName = n, validate: true); }
    }

    public bool IsSilentOnCanBus
    {
        get => Data.SilentOnCanBus;
        set => SetProperty(Data.SilentOnCanBus, value, Data, (u, n) => u.SilentOnCanBus = n);
    }

    public BitRateViewModel[] BitRates { get; } = [new("1 Mbs", 1000000), new("800 Kbs", 800000), new("500 Kbs", 500000), new("250 Kbs", 250000), new("125 Kbs", 125000), new("100 Kbs", 100000), new("50 Kbs", 50000), new("20 Kbs", 20000), new("10 Kbs", 10000)];
    public BitRateViewModel? SelectedBitRate
    {
        get => BitRates.FirstOrDefault(x => x.Value == Data.BitRate);
        set => SetProperty(Data.BitRate, value?.Value, Data, (u, n) => u.BitRate = n ?? 0);
    }

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

    public class BitRateViewModel(string name, int value)
    {
        public string Name { get; set; } = name;
        public int Value { get; set; } = value;
    }
}
