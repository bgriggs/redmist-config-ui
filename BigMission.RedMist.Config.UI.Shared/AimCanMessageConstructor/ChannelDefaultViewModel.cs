using BigMission.RedMist.Config.Shared.AimCanMessageConstructor;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BigMission.RedMist.Config.UI.Shared.AimCanMessageConstructor;

[NotifyDataErrorInfo]
public partial class ChannelDefaultViewModel : ObservableValidator
{
    public ChannelDefault Data { get; set; } = new();

    public string Name => Data.Name;

    public bool Include
    {
        get => Data.Include;
        set => SetProperty(Data.Include, value, Data, (u, n) => u.Include = n, validate: true);
    }

    [Range(1, 4)]
    public int Length
    {
        get => Data.Length;
        set => SetProperty(Data.Length, value, Data, (u, n) => u.Length = n, validate: true);
    }

    [Range(1, 1000)]
    public int Multiplier
    {
        get => Data.Multiplier;
        set => SetProperty(Data.Multiplier, value, Data, (u, n) => u.Multiplier = n, validate: true);
    }

    [CustomValidation(typeof(AimCanViewModel), nameof(AimCanViewModel.FrequencyValidate))]
    public int FrequencyHz
    {
        get => Data.FrequencyHz;
        set => SetProperty(Data.FrequencyHz, value, Data, (u, n) => u.FrequencyHz = n, validate: true);
    }
}
