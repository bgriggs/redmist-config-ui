using BigMission.RedMist.Config.Shared.General;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BigMission.RedMist.Config.UI.Shared.General;

[NotifyDataErrorInfo]
public partial class GeneralViewModel : ObservableValidator
{
    public GeneralConfigDto Data { get; set; } = new();

    [Required(ErrorMessage = "Car is required.")]
    [MinLength(1)]
    [MaxLength(6)]
    public string? Car
    {
        get => Data.Car;
        set => SetProperty(Data.Car, value, Data, (u, n) => u.Car = n, validate: true);
    }

    [Range(0, 10000)]
    public int DeviceAppId
    {
        get => Data.DeviceAppId;
        set => SetProperty(Data.DeviceAppId, value, Data, (u, n) => u.DeviceAppId = n, validate: true);
    }

    [MinLength(1)]
    [MaxLength(300)]
    public string? ApiUrl
    {
        get => Data.ApiUrl;
        set => SetProperty(Data.ApiUrl, value, Data, (u, n) => u.ApiUrl = n, validate: true);
    }

    [MinLength(1)]
    [MaxLength(150)]
    public string? ApiKey
    {
        get => Data.ApiKey;
        set => SetProperty(Data.ApiKey, value, Data, (u, n) => u.ApiKey = n, validate: true);
    }
}
