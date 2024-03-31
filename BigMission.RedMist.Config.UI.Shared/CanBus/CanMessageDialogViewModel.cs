using BigMission.RedMist.Config.Shared.CanBus;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using MsBox.Avalonia;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BigMission.RedMist.Config.UI.Shared.CanBus;

public partial class CanMessageDialogViewModel : ObservableValidator
{
    public CanMessageConfigDto CanMessageConfigDto { get; }

    public bool IsExtended
    {
        get { return CanMessageConfigDto.IsExtended; }
        set
        {
            if (SetProperty(CanMessageConfigDto.IsExtended, value, CanMessageConfigDto, (u, n) => u.IsExtended = n))
            {
                OnPropertyChanged(nameof(CanIdStr));
            }
        }
    }

    [CustomValidation(typeof(CanMessageDialogViewModel), nameof(ValidateCanId))]
    public string CanIdStr
    {
        get { return $"{CanMessageConfigDto.CanId:X}"; }
        set
        {
            ValidateProperty(value);
            if (int.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out int canId))
            {
                CanMessageConfigDto.CanId = canId;
            }
        }
    }

    [Range(1, 8)]
    public int Length
    {
        get { return CanMessageConfigDto.Length; }
        set
        {
            SetProperty(CanMessageConfigDto.Length, value, CanMessageConfigDto, (u, n) => u.Length = n, true);
        }
    }

    public bool IsBigEndian
    {
        get { return CanMessageConfigDto.IsBigEndian; }
        set { SetProperty(CanMessageConfigDto.IsBigEndian, value, CanMessageConfigDto, (u, n) => u.IsBigEndian = n); }
    }

    public bool IsReceive
    {
        get { return CanMessageConfigDto.IsReceive; }
        set
        {
            if (SetProperty(CanMessageConfigDto.IsReceive, value, CanMessageConfigDto, (u, n) => u.IsReceive = n))
            {
                OnPropertyChanged(nameof(IsTransmit));
            }
        }
    }

    public bool IsTransmit => !IsReceive;

    [Range(50, 5000)]
    public int TransmitFrequencyMs
    {
        get { return (int)CanMessageConfigDto.TransmitRate.TotalMilliseconds; }
        set 
        { 
            ValidateProperty(value);
            CanMessageConfigDto.TransmitRate = TimeSpan.FromMilliseconds(value); 
        }
    }


    public CanMessageDialogViewModel(CanMessageConfigDto canMessageConfigDto)
    {
        CanMessageConfigDto = canMessageConfigDto;
    }

    public async Task OKClickAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            var message = GetErrorMessage();
            var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
            {
                ButtonDefinitions = [new ButtonDefinition { Name = "OK", IsDefault = true }],
                ContentTitle = "Validation Errors",
                ContentMessage = $"Fix validation errors before continuing.{Environment.NewLine}{message}",
                Icon = Icon.Error,
                MaxWidth = 500,
            });

            await box.ShowAsync();
            return;
        }

        // Check for overlapping channels
        if (!CanMessageConfigDto.Validate(out string error))
        {
            var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
            {
                ButtonDefinitions = [new ButtonDefinition { Name = "OK", IsDefault = true }],
                ContentTitle = "Validation Errors",
                ContentMessage = $"Specified length would be invalid given the current channels:{Environment.NewLine}{error}",
                Icon = Icon.Error,
                MaxWidth = 500,
            });

            await box.ShowAsync();
            return;
        }

        DialogHost.Close("MainDialogHost", this);
    }

    private string GetErrorMessage()
    {
        var sb = new StringBuilder();
        foreach (var error in GetErrors())
        {
            sb.AppendLine(error.ErrorMessage);
        }
        return sb.ToString();
    }

    public static ValidationResult ValidateCanId(string idStr, ValidationContext context)
    {
        var instance = (CanMessageDialogViewModel)context.ObjectInstance;

        if (!int.TryParse(idStr, System.Globalization.NumberStyles.HexNumber, null, out int canId) || canId < 0)
        {
            return new("The ID was not a valid integer");
        }

        if (instance.CanMessageConfigDto.IsExtended)
        {
            if (canId > 0x1FFFFFFF)
            {
                return new("The ID was too large for an extended message 0x1FFFFFFF");
            }
        }
        else
        {
            if (canId > 0x7FF)
            {
                return new("The ID was too large for a standard message 0x7FF");
            }
        }

        return ValidationResult.Success!;
    }
}
