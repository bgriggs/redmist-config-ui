using BigMission.Avalonia.Utilities;
using BigMission.RedMist.Config.Shared.CanBus;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BigMission.RedMist.Config.UI.Shared.CanBus;

public partial class CanBusViewModel : ObservableObject
{
    private CanBusConfigDto data = new();
    public CanBusConfigDto Data
    {
        get => data;
        set
        {
            data = value;
            Messages.SetRange(data.Messages.Select(m => new CanMessageViewModel { Data = m }));
        }
    }

    public LargeObservableCollection<CanMessageViewModel> Messages { get; set; } = [];

}
