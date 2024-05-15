using BigMission.Avalonia.Utilities;
using BigMission.ChannelManagement.Shared;
using BigMission.RedMist.Config.Shared.Channels;
using BigMission.RedMist.Config.Shared.Logging;
using BigMission.RedMist.Config.UI.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;
using System.Collections.Specialized;

namespace BigMission.RedMist.Config.UI.Shared.Logging;

/// <summary>
/// Overall view model for logging configuration.
/// </summary>
public class LoggingViewModel : ObservableObject
{
    private readonly ChannelProvider channelProvider;

    public LoggingConfigDto Data { get; }

    public bool EnableRollingLog
    {
        get => Data.EnableRollingLog;
        set => SetProperty(Data.EnableRollingLog, value, Data, (u, n) => u.EnableRollingLog = n);
    }

    public LargeObservableCollection<LogEntryViewModel> LogEntries { get; } = [];

    public LoggingViewModel(LoggingConfigDto data, ChannelProvider channelProvider)
    {
        Data = data;
        this.channelProvider = channelProvider;
        LogEntries.AddRange(data.LogEntries.Select(l => new LogEntryViewModel(l, channelProvider)));
        LogEntries.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) =>
        {
            Data.LogEntries.Clear();
            Data.LogEntries.AddRange(LogEntries.Select(l => l.LogEntry));
        };
    }

    public async void AddMessageClick()
    {
        var usedChannelIds = LogEntries.Select(l => l.LogEntry.ChannelId).ToArray();
        var vm = new ChannelSelectionDialogViewModel(channelProvider, usedChannelIds) { HideUsedChannels = false };
        var result = await DialogHost.Show(vm, "MainDialogHost");
        if (result is ChannelMappingDto ch)
        {
            var logEntry = new LogEntry { ChannelId = ch.Id, Rate = LoggingRate.OneHz };
            var logEntryVm = new LogEntryViewModel(logEntry, channelProvider);
            LogEntries.Add(logEntryVm);
        }
    }

    public void RemoveLogEntry(object source)
    {
        if (source is LogEntryViewModel logEntry)
        {
            LogEntries.Remove(logEntry);
        }
    }
}
