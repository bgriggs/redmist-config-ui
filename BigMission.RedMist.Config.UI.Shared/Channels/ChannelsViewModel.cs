using BigMission.Avalonia.Utilities;
using BigMission.ChannelManagement;
using BigMission.RedMist.Config.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;
using System.Collections.Specialized;

namespace BigMission.RedMist.Config.UI.Shared.Channels;

[NotifyDataErrorInfo]
public partial class ChannelsViewModel : ObservableValidator
{
    private readonly ChannelConfigDto data;
    private readonly ChannelProvider channelProvider;

    public LargeObservableCollection<ChannelMappingRowViewModel> Channels { get; } = [];

    private string searchText = string.Empty;
    public string SearchText
    {
        get => searchText;
        set
        {
            SetProperty(ref searchText, value);
            FilterRows(searchText);
        }
    }

    public ChannelProvider ChannelProvider => channelProvider;

    public ChannelsViewModel(ChannelConfigDto channelConfigDto, ChannelProvider channelProvider)
    {
        data = channelConfigDto;
        this.channelProvider = channelProvider;
        InitializeChannelViewModels();
        Channels.CollectionChanged += Channels_CollectionChanged;
    }

    private void InitializeChannelViewModels()
    {
        Channels.BeginBulkOperation();
        try
        {
            Channels.Clear();
            foreach (var channel in data.ChannelMappings)
            {
                Channels.Add(new ChannelMappingRowViewModel(channel, this));
            }
        }
        finally
        {
            // Suppress the event notification since we do not need to sync back to the source data
            Channels.EndBulkOperation(false);
        }
    }

    /// <summary>
    /// When the channels collection changes, update the source data model.
    /// </summary>
    private void Channels_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        data.ChannelMappings.Clear();
        data.ChannelMappings.AddRange(Channels.Select(c => c.Data));
    }

    public async Task AddChannelClick()
    {
        var dto = new ChannelMappingDto { DataType = "Temperature", BaseUnitType = "DegreesFahrenheit", DisplayUnitType = "DegreesFahrenheit" };
        var result = await DialogHost.Show(new ChannelMappingEditViewModel(dto, [.. Channels], channelProvider), "MainDialogHost");
        if (result is ChannelMappingEditViewModel map)
        {
            map.Data.Id = data.IncNextId();
            var rowVm = new ChannelMappingRowViewModel(map.Data, this);
            Channels.Add(rowVm);
        }
    }

    public void UpdateChannelMapping(ChannelMappingRowViewModel row, ChannelMappingDto updatedMapping)
    {
        var index = Channels.IndexOf(row);
        Channels.BeginBulkOperation();
        try
        {
            Channels.RemoveAt(index);
            var rowVm = new ChannelMappingRowViewModel(updatedMapping, this);
            Channels.Insert(index, rowVm);
        }
        finally { Channels.EndBulkOperation(); }
    }

    public void DeleteChannelMapping(ChannelMappingRowViewModel row)
    {
        Channels.Remove(row);
    }

    public void FilterRows(string term)
    {
        ChannelMappingRowViewModel[] remove = [];
        if (!string.IsNullOrWhiteSpace(term))
        {
            remove = Channels.Where(c => !c.Name.Contains(term, StringComparison.CurrentCultureIgnoreCase)).ToArray();
            foreach (var row in remove)
            {
                row.IsVisible = false;
            }
        }

        var matches = Channels.Except(remove);
        foreach (var row in matches)
        {
            row.IsVisible = true;
        }
    }

    public bool ClearSearch()
    {
        SearchText = string.Empty;
        return true;
    }

    public void RefreshIsUsed()
    {
        var channelDependencies = ChannelProvider.GetChannelDependencies();
        foreach (var row in Channels)
        {
            var used = channelDependencies.Any(c => c.ChannelId == row.Data.Id);
            row.IsUsed = used;
        }
    }
}
