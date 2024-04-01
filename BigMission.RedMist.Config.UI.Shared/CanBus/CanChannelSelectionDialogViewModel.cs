using BigMission.RedMist.Config.Shared.CanBus;
using BigMission.RedMist.Config.Shared.Channels;
using BigMission.RedMist.Config.UI.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using MsBox.Avalonia;
using System.ComponentModel.DataAnnotations;
using DialogHostAvalonia;
using Newtonsoft.Json;
using System.Text;

namespace BigMission.RedMist.Config.UI.Shared.CanBus;

/// <summary>
/// Allows a user to select and edit a channel assignment when clicking on the Byte Overlay from the CAN table.
/// </summary>
public partial class CanChannelSelectionDialogViewModel : ObservableValidator
{
    private readonly CanChannelAssignmentConfigDto channelConfig;
    private readonly CanMessageConfigDto parentMessageCopy;

    [Range(0, 7)]
    public int Offset
    {
        get => channelConfig.Offset;
        set
        {
            SetProperty(channelConfig.Offset, value, channelConfig, (u, n) => u.Offset = n, validate: true);
            OnPropertyChanged(nameof(Length));
        }
    }

    private static readonly int[] ValidLengths = [1, 2, 4, 8];
    [CustomValidation(typeof(CanChannelSelectionDialogViewModel), nameof(ValidateLength))]
    public int Length
    {
        get => channelConfig.Length;
        set
        {
            SetProperty(channelConfig.Length, value, channelConfig, (u, n) => u.Length = n, validate: true);
            try
            {
                Mask = MaxMask(value).ToString("X");
            }
            catch { }
        }
    }

    [CustomValidation(typeof(CanChannelSelectionDialogViewModel), nameof(ValidateMask))]
    public string Mask
    {
        get => $"{channelConfig.Mask:X}";
        set
        {
            ValidateProperty(value);
            if (ulong.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out ulong mask))
            {
                channelConfig.Mask = mask;
                OnPropertyChanged(nameof(Mask));
            }
        }
    }

    public bool IsSigned
    {
        get => channelConfig.IsSigned;
        set => SetProperty(channelConfig.IsSigned, value, channelConfig, (u, n) => u.IsSigned = n);
    }

    [Range(1, double.MaxValue)]
    public double FormulaMultiplier
    {
        get => channelConfig.FormulaMultiplier;
        set => SetProperty(channelConfig.FormulaMultiplier, value, channelConfig, (u, n) => u.FormulaMultiplier = n, validate: true);
    }

    [Range(1, double.MaxValue)]
    public double FormulaDivider
    {
        get => channelConfig.FormulaDivider;
        set => SetProperty(channelConfig.FormulaDivider, value, channelConfig, (u, n) => u.FormulaDivider = n, validate: true);
    }

    [Range(0, double.MaxValue)]
    public double FormulaConst
    {
        get => channelConfig.FormulaConst;
        set => SetProperty(channelConfig.FormulaConst, value, channelConfig, (u, n) => u.FormulaConst = n, validate: true);
    }

    public ChannelSelectionControlViewModel ChannelSelectionViewModel { get; }

    public CanChannelSelectionDialogViewModel(ChannelProvider channelProvider, CanChannelAssignmentConfigDto channelConfig, CanMessageConfigDto parentMessageCopy)
    {
        var chMappingDto = channelProvider.GetChannel(channelConfig.ChannelId);
        ChannelSelectionViewModel = new ChannelSelectionControlViewModel(channelProvider) { IsSelectable = true, SelectedChannelMapping = chMappingDto };
        this.channelConfig = channelConfig;
        this.parentMessageCopy = parentMessageCopy;
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
        // Make another message copy so validation can be performed on a clean object each time this is called
        var messageJson = JsonConvert.SerializeObject(parentMessageCopy);
        var messageCopy = JsonConvert.DeserializeObject<CanMessageConfigDto>(messageJson)!;
        messageCopy.ChannelAssignments.RemoveAll(x => x.ChannelId == channelConfig.ChannelId);
        messageCopy.ChannelAssignments.Add(channelConfig);
        if (!messageCopy.Validate(out string error))
        {
            var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
            {
                ButtonDefinitions = [new ButtonDefinition { Name = "OK", IsDefault = true }],
                ContentTitle = "Validation Errors",
                ContentMessage = $"Specified values would result in overlapping channels:{Environment.NewLine}{error}",
                Icon = Icon.Error,
                MaxWidth = 500,
            });

            await box.ShowAsync();
            return;
        }

        // Transfer over the channel selection from the nested view model
        channelConfig.ChannelId = ChannelSelectionViewModel.SelectedChannelMapping?.Id ?? 0;

        DialogHost.Close("MainDialogHost", channelConfig);
    }

    public void UnassignChannelAsync()
    {
        channelConfig.ChannelId = 0;
        DialogHost.Close("MainDialogHost", channelConfig);
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

    public static ValidationResult ValidateLength(int length, ValidationContext context)
    {
        var instance = (CanChannelSelectionDialogViewModel)context.ObjectInstance;
        var bytesRemaining = 8 - instance.Offset;
        var availableLengths = ValidLengths.Where(x => x <= bytesRemaining).ToArray();
        if (availableLengths.Contains(length))
        {
            return ValidationResult.Success!;
        }

        return new("Length must be: " + string.Join(",", availableLengths));
    }

    public static ValidationResult ValidateMask(string maskStr, ValidationContext context)
    {
        var instance = (CanChannelSelectionDialogViewModel)context.ObjectInstance;
        if (!ulong.TryParse(maskStr, System.Globalization.NumberStyles.HexNumber, null, out ulong mask))
        {
            return new("The Mask was not a valid integer");
        }
        var max = MaxMask(instance.Length);

        if (mask <= max)
        {
            return ValidationResult.Success!;
        }

        return new($"Mask must be less than 0x{max:X}");
    }

    private static ulong MaxMask(int lengthBytes) => (ulong)(System.Numerics.BigInteger.Pow(2, lengthBytes * 8) - 1);
}
