using BigMission.Avalonia.Utilities;
using BigMission.ChannelManagement.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Reactive.Linq;
using UnitsNet;

namespace BigMission.RedMist.Config.UI.Shared.Channels;

[NotifyDataErrorInfo]
public partial class ChannelMappingViewModel : ObservableValidator
{
    private ChannelMappingDto data = new();
    public ChannelMappingDto Data
    {
        get => data;
        set
        {
            data = value;
            SelectedDataType = DataTypes.FirstOrDefault(d => d.Value == data.DataType);
            SelectedBaseUnits = Units.FirstOrDefault(u => u == data.BaseUnitType);
            SelectedDisplayUnits = Units.FirstOrDefault(u => u == data.DisplayUnitType);
        }
    }

    /// <summary>
    /// Available channels to use in validation.
    /// </summary>
    public LargeObservableCollection<ChannelMappingRowViewModel> ParentChannels { get; set; } = [];

    public ObservableCollection<DataTypeViewModel> DataTypes { get; set; } = [];
    public ObservableCollection<string> Units { get; set; } = [];

    /// <summary>
    /// Gets whether the user can edit the name and input parameters of the channel.
    /// </summary>
    public bool IsReserved => Data.IsReserved;

    [MinLength(1)]
    [MaxLength(30)]
    [CustomValidation(typeof(ChannelMappingViewModel), nameof(DuplicateNameValidate))]
    public string? Name
    {
        get => Data.Name;
        set => SetProperty(Data.Name, value, Data, (u, n) => u.Name = n, validate: true);
    }

    [MinLength(0)]
    [MaxLength(7)]
    [CustomValidation(typeof(ChannelMappingViewModel), nameof(DuplicateAbbreviationValidate))]
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

    private DataTypeViewModel? selectedDataType;
    public DataTypeViewModel? SelectedDataType
    {
        get { return selectedDataType; }
        set
        {
            if (SetProperty(ref selectedDataType, value))
            {
                SetProperty(Data.DataType, value?.Value, Data, (u, n) => u.DataType = n, validate: false);
                UpdateUnits();
            }
        }
    }

    public string? SelectedBaseUnits
    {
        get => Data.BaseUnitType;
        set => SetProperty(Data.BaseUnitType, value, Data, (u, n) => u.BaseUnitType = n, validate: false);
    }

    [Range(0, 6)]
    public int BaseDecimalPlaces
    {
        get => Data.BaseDecimalPlaces;
        set => SetProperty(Data.BaseDecimalPlaces, value, Data, (u, n) => u.BaseDecimalPlaces = n, validate: true);
    }

    public string? SelectedDisplayUnits
    {
        get => Data.DisplayUnitType;
        set => SetProperty(Data.DisplayUnitType, value, Data, (u, n) => u.DisplayUnitType = n, validate: false);
    }

    [Range(0, 6)]
    public int DisplayDecimalPlaces
    {
        get => Data.DisplayDecimalPlaces;
        set => SetProperty(Data.DisplayDecimalPlaces, value, Data, (u, n) => u.DisplayDecimalPlaces = n, validate: true);
    }

    public ChannelMappingViewModel()
    {
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
    }

    /// <summary>
    /// Validate duplicate channel name.
    /// </summary>
    public static ValidationResult DuplicateNameValidate(string name, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(name)) return new ValidationResult(null);
        if (context.ObjectInstance is not ChannelMappingViewModel map) { return new ValidationResult(null); }
        name = name.Trim();
        var match = map.ParentChannels.FirstOrDefault(c => string.Compare(c.Name, name, StringComparison.CurrentCultureIgnoreCase) == 0);
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
        if (string.IsNullOrWhiteSpace(abbreviation)) return new ValidationResult(null);
        if (context.ObjectInstance is not ChannelMappingViewModel map) { return new ValidationResult(null); }
        abbreviation = abbreviation.Trim();
        var match = map.ParentChannels.FirstOrDefault(c => string.Compare(c.Abbreviation, abbreviation, StringComparison.CurrentCultureIgnoreCase) == 0);
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
                Units.Add(unit.PluralName);
            }
        }
        else
        {
            Units.Clear();
        }
    }
}
