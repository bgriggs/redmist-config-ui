using BigMission.ChannelManagement.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace BigMission.RedMist.Config.UI.Shared.Channels;

/// <summary>
/// Represents a row in the Channels data grid.
/// </summary>
public partial class ChannelMappingRowViewModel : ObservableObject
{
    public ChannelMappingDto Data { get; }
    public ChannelsViewModel? ParentVm { get; }

    [ObservableProperty]
    private bool isVisible = true;

    public string Name => Data.Name;
    public string Category => Data.Category;
    public string Abbreviation => Data.Abbreviation;
    public string DataType
    {
        get
        {
            if (Data.IsStringValue) return "String";
            return Data.DataType;
        }
    }

    [ObservableProperty]
    private bool isUsed;

    public string? Source { get; set; }
    public bool IsReserved => Data.IsReserved;

    public ChannelMappingRowViewModel(ChannelMappingDto data, ChannelsViewModel? parentVm)
    {
        Data = data;
        ParentVm = parentVm;
    }

    public async Task EditChannelClick(object obj)
    {
        if (obj is ChannelMappingDto map)
        {
            var copyMap = map.Copy();
            var chVm = new ChannelMappingEditViewModel(copyMap, [.. ParentVm?.Channels], ParentVm?.ChannelProvider);
            var result = await DialogHost.Show(chVm, "MainDialogHost");
            if (result is ChannelMappingEditViewModel editedMap)
            {
                ParentVm?.UpdateChannelMapping(this, editedMap.Data);
            }
        }
    }

    public async Task DeleteChannel(object obj)
    {
        if (obj is ChannelMappingDto)
        {
            if (IsReserved)
            {
                var m = MessageBoxManager.GetMessageBoxStandard(
                    "Channel Delete", "Cannot delete reserved channel.", ButtonEnum.Ok, Icon.Forbidden);
                await m.ShowAsync();
                return;
            }

            var box = MessageBoxManager.GetMessageBoxStandard(
                "Channel Delete", "Are you sure you would like to delete this channel?", ButtonEnum.YesNo, Icon.Question);

            var result = await box.ShowAsync();
            if (result.HasFlag(ButtonResult.Yes))
            {
                ParentVm?.DeleteChannelMapping(this);
            }
        }
    }
}
