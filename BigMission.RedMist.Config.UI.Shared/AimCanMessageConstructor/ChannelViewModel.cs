using Avalonia.Media.Imaging;
using BigMission.RedMist.Config.Shared.AimCanMessageConstructor;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BigMission.RedMist.Config.UI.Shared.AimCanMessageConstructor;

[NotifyDataErrorInfo]
public partial class ChannelViewModel : ObservableValidator
{
    public ImageText Data { get; set; } = new();

    public string Name => Data.Text;
    public Bitmap? NameImage { get; set; }
    public string Group { get; set; } = string.Empty;
    public Bitmap? GroupImage { get; set; }
    public int GroupIndex { get; set; }

    [Required]
    [ObservableProperty]
    private bool include;

    [ObservableProperty]
    [Range(1, 4)]
    private int length;

    [ObservableProperty]
    [Range(1, 1000)]
    private int multiplier;

    [ObservableProperty]
    [CustomValidation(typeof(AimCanViewModel), nameof(AimCanViewModel.FrequencyValidate))]
    private int frequencyHz;
}