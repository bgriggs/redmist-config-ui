using BigMission.RedMist.Config.Shared.General;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BigMission.RedMist.Config.UI.Shared.General;

[NotifyDataErrorInfo]
public partial class GeneralViewModel : ObservableValidator
{
    private readonly GeneralConfigDto data;

    [Required(ErrorMessage = "Car is required.")]
    [MinLength(1)]
    [MaxLength(6)]
    public string? Car
    {
        get => data.Car;
        set => SetProperty(data.Car, value, data, (u, n) => u.Car = n, validate: true);
    }

    [Range(0, 10000)]
    public int DeviceAppId
    {
        get => data.DeviceAppId;
        set => SetProperty(data.DeviceAppId, value, data, (u, n) => u.DeviceAppId = n, validate: true);
    }

    [MinLength(1)]
    [MaxLength(300)]
    public string? ApiUrl
    {
        get => data.ApiUrl;
        set => SetProperty(data.ApiUrl, value, data, (u, n) => u.ApiUrl = n, validate: true);
    }

    [MinLength(1)]
    [MaxLength(150)]
    public string? ApiKey
    {
        get => data.ApiKey;
        set => SetProperty(data.ApiKey, value, data, (u, n) => u.ApiKey = n, validate: true);
    }

    public GeneralViewModel(GeneralConfigDto data)
    {
        this.data = data;
    }
}
