﻿using BigMission.Avalonia.Utilities;
using BigMission.ChannelManagement.Shared;
using BigMission.RedMist.Config.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;
using System.Collections.Specialized;

namespace BigMission.RedMist.Config.UI.Shared.Channels;

[NotifyDataErrorInfo]
public partial class ChannelsViewModel : ObservableValidator
{
    private ChannelConfigDto data = new();
    public ChannelConfigDto Data
    {
        get => data;
        set
        {
            data = value;
            Channels.BeginBulkOperation();
            try
            {
                Channels.Clear();
                foreach (var channel in data.ChannelMappings)
                {
                    Channels.Add(new ChannelMappingRowViewModel { Data = channel, ParentVm = this });
                }
            }
            finally
            {
                // Suppress the event notification since we do not need to sync back to the source data
                Channels.EndBulkOperation(false);
            }
        }
    }

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

    public ChannelsViewModel()
    {
        Channels.CollectionChanged += Channels_CollectionChanged;
    }

    /// <summary>
    /// When the channels collection changes, update the source data model.
    /// </summary>
    private void Channels_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Data.ChannelMappings.Clear();
        Data.ChannelMappings.AddRange(Channels.Select(c => c.Data));
    }

    public async Task AddChannelClick()
    {
        var result = await DialogHost.Show(new ChannelMappingViewModel { ParentChannels = Channels }, "MainDialogHost");
        if (result is ChannelMappingViewModel map)
        {
            map.Data.Id = Data.IncNextId();
            var rowVm = new ChannelMappingRowViewModel { Data = map.Data, ParentVm = this };
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
            var rowVm = new ChannelMappingRowViewModel { Data = updatedMapping, ParentVm = this };
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
}
