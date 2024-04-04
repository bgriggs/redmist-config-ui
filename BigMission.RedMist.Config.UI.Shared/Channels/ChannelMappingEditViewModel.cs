using ActiproSoftware.Properties.Shared;
using BigMission.Avalonia.Utilities;
using BigMission.ChannelManagement.Shared;
using BigMission.RedMist.Config.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Reactive.Linq;
using System.Text;
using UnitsNet;

namespace BigMission.RedMist.Config.UI.Shared.Channels;

/// <summary>
/// Used for adding or editing a channel mapping.
/// </summary>
[NotifyDataErrorInfo]
public partial class ChannelMappingEditViewModel : ObservableValidator
{
    private readonly ChannelMappingDto data;
    public ChannelMappingDto Data => data;

    /// <summary>
    /// Available channels to use in validation.
    /// </summary>
    public ImmutableArray<ChannelMappingRowViewModel> ParentChannels { get; }

    public ObservableCollection<DataTypeViewModel> DataTypes { get; } = [];
    public ObservableCollection<KeyValueViewModel> Units { get; } = [];

    /// <summary>
    /// Gets whether the user can edit the name and input parameters of the channel.
    /// </summary>
    public bool IsReserved => Data.IsReserved;

    [MinLength(1)]
    [MaxLength(30)]
    [CustomValidation(typeof(ChannelMappingEditViewModel), nameof(DuplicateNameValidate))]
    public string? Name
    {
        get => Data.Name;
        set => SetProperty(Data.Name, value, Data, (u, n) => u.Name = n, validate: true);
    }

    [CustomValidation(typeof(ChannelMappingEditViewModel), nameof(DuplicateAbbreviationValidate))]
    public string? Abbreviation
    {
        get => Data.Abbreviation;
        set => SetProperty(Data.Abbreviation, value, Data, (u, n) => u.Abbreviation = n, validate: true);
    }

    public bool IsStringValue
    {
        get => Data.IsStringValue;
        set => SetProperty(Data.IsStringValue, value, Data, (u, n) => u.IsStringValue = n, validate: false);
    }

    [Required]
    public DataTypeViewModel? SelectedDataType
    {
        get => DataTypes.FirstOrDefault(d => d.Value == data.DataType);
        set
        {
            ValidateProperty(value, nameof(SelectedDataType));
            if (data.DataType != value?.Value)
            {
                data.DataType = value?.Value;
                OnPropertyChanged(nameof(SelectedDataType));
                UpdateUnits();
            }
            //if (SetProperty(data.DataType, value?.Value, data, (u, n) => u.DataType = n, validate: true))
            //{
            //    UpdateUnits();
            //}
        }
    }

    [Required]
    public KeyValueViewModel? SelectedBaseUnits
    {
        get => Units.FirstOrDefault(d => d.Value == Data.BaseUnitType);
        set
        {
            //SetProperty(Data.BaseUnitType, value?.Value, Data, (u, n) => u.BaseUnitType = n, validate: true);
            ValidateProperty(value, nameof(SelectedBaseUnits));
            if (data.BaseUnitType != value?.Value)
            {
                data.BaseUnitType = value?.Value;
                OnPropertyChanged(nameof(SelectedBaseUnits));
            }
        }
    }

    [Range(0, 6)]
    public int BaseDecimalPlaces
    {
        get => Data.BaseDecimalPlaces;
        set => SetProperty(Data.BaseDecimalPlaces, value, Data, (u, n) => u.BaseDecimalPlaces = n, validate: true);
    }

    [Required]
    public KeyValueViewModel? SelectedDisplayUnits
    {
        get => Units.FirstOrDefault(d => d.Value == Data.DisplayUnitType);
        set
        {
            ValidateProperty(value, nameof(SelectedDisplayUnits));
            if (data.DisplayUnitType != value?.Value)
            {
                data.DisplayUnitType = value?.Value;
                OnPropertyChanged(nameof(SelectedDisplayUnits));
            }
            //SetProperty(Data.DisplayUnitType, value?.Value, Data, (u, n) => u.DisplayUnitType = n, validate: true);
        }
    }

    [Range(0, 6)]
    public int DisplayDecimalPlaces
    {
        get => Data.DisplayDecimalPlaces;
        set => SetProperty(Data.DisplayDecimalPlaces, value, Data, (u, n) => u.DisplayDecimalPlaces = n, validate: true);
    }

    

    public ChannelMappingEditViewModel(ChannelMappingDto data, ImmutableArray<ChannelMappingRowViewModel> parentChannels, ChannelProvider? channelProvider)
    {
        this.data = data;
        ParentChannels = parentChannels;

        DataTypes.Add(new DataTypeViewModel { DisplayName = "Temperature", Value = nameof(Temperature) });
        DataTypes.Add(new DataTypeViewModel { DisplayName = "Length", Value = nameof(Length) });
        DataTypes.Add(new DataTypeViewModel { DisplayName = "Volume", Value = nameof(Volume) });
        DataTypes.Add(new DataTypeViewModel { DisplayName = "Volume Flow", Value = nameof(VolumeFlow) });
        DataTypes.Add(new DataTypeViewModel { DisplayName = "Duration", Value = nameof(Duration) });
        DataTypes.Add(new DataTypeViewModel { DisplayName = "Speed", Value = nameof(Speed) });
        DataTypes.Add(new DataTypeViewModel { DisplayName = "Pressure", Value = nameof(Pressure) });
        DataTypes.Add(new DataTypeViewModel { DisplayName = "Force", Value = nameof(Force) });
        DataTypes.Add(new DataTypeViewModel { DisplayName = "Voltage", Value = nameof(ElectricPotential) });
        DataTypes.Add(new DataTypeViewModel { DisplayName = "Mass", Value = nameof(Mass) });
        DataTypes.Add(new DataTypeViewModel { DisplayName = "Ratio", Value = nameof(Ratio) });
        DataTypes.Add(new DataTypeViewModel { DisplayName = "Current", Value = nameof(ElectricCurrent) });
        DataTypes.Add(new DataTypeViewModel { DisplayName = "Resistance", Value = nameof(ElectricResistance) });

        SelectedDataType = DataTypes.FirstOrDefault(d => d.Value == data.DataType);
        UpdateUnits();
    }

    /// <summary>
    /// Validate duplicate channel name.
    /// </summary>
    public static ValidationResult DuplicateNameValidate(string name, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(name)) return new ValidationResult(null);
        if (context.ObjectInstance is not ChannelMappingEditViewModel map) { return new ValidationResult(null); }
        name = name.Trim();

        // Remove the current channel from the list of existing names
        var existingNames = map.ParentChannels.Where(c => c.Data.Id != map.Data.Id).Select(c => c.Name);
        var match = existingNames.FirstOrDefault(c => string.Compare(c, name, StringComparison.CurrentCultureIgnoreCase) == 0);
        if (match is not null)
        {
            return new ValidationResult("There is already a channel with this name.");
        }

        return ValidationResult.Success!;
    }

    /// <summary>
    /// Validate duplicate channel abbreviation.
    /// </summary>
    public static ValidationResult DuplicateAbbreviationValidate(string abbreviation, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(abbreviation)) return ValidationResult.Success!;
        if (context.ObjectInstance is not ChannelMappingEditViewModel map) { return new ValidationResult(null); }
        abbreviation = abbreviation.Trim();
        if (abbreviation.Length > 5) return new ValidationResult("Abbreviation must be 5 characters or less.");

        var existingAbbreviations = map.ParentChannels.Where(c => c.Data.Id != map.Data.Id).Select(c => c.Abbreviation);
        var match = existingAbbreviations.FirstOrDefault(c => string.Compare(c, abbreviation, StringComparison.CurrentCultureIgnoreCase) == 0);
        if (match is not null)
        {
            return new ValidationResult("There is already a channel with this abbreviation.");
        }

        return ValidationResult.Success!;
    }

    private void UpdateUnits()
    {
        if (SelectedDataType is not null)
        {
            Units.Clear();
            var q = Quantity.ByName[SelectedDataType.Value!];
            foreach (var unit in q.UnitInfos)
            {
                Units.Add(new KeyValueViewModel(unit.PluralName, unit.PluralName));
            }

            SelectedBaseUnits = Units.FirstOrDefault(u => u.Value == data.BaseUnitType);
            SelectedDisplayUnits = Units.FirstOrDefault(u => u.Value == data.DisplayUnitType);
        }
        else
        {
            Units.Clear();
        }
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

    public class KeyValueViewModel(string displayName, string value)
    {
        public string DisplayName { get; set; } = displayName;
        public string Value { get; set; } = value;
    }
}
