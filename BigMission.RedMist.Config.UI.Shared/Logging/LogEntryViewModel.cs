using BigMission.RedMist.Config.Shared.Channels;
using BigMission.RedMist.Config.Shared.Logging;
using BigMission.RedMist.Config.UI.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BigMission.RedMist.Config.UI.Shared.Logging;

/// <summary>
/// View model for a log entry row.
/// </summary>
public partial class LogEntryViewModel : ObservableObject
{
    public LogEntry LogEntry { get; }

    public ObservableCollection<LogFrequencyViewModel> Frequencies { get; }

    private LogFrequencyViewModel selectedFrequency;
    public LogFrequencyViewModel SelectedFrequency
    {
        get => selectedFrequency;
        set => SetProperty(ref selectedFrequency, value);
    }

    public ChannelSelectionControlViewModel ChannelViewModel { get; }

    public LogEntryViewModel(LogEntry logEntry, ChannelProvider channelProvider)
    {
        LogEntry = logEntry;

        Frequencies =
        [
            new LogFrequencyViewModel { Name = "0.25 Hz", Rate = LoggingRate.QuarterHz },
            new LogFrequencyViewModel { Name = "0.5 Hz", Rate = LoggingRate.HalfHz },
            new LogFrequencyViewModel { Name = "1 Hz", Rate = LoggingRate.OneHz },
            new LogFrequencyViewModel { Name = "5 Hz", Rate = LoggingRate.FiftyHz },
            new LogFrequencyViewModel { Name = "10 Hz", Rate = LoggingRate.TenHz },
            new LogFrequencyViewModel { Name = "25 Hz", Rate = LoggingRate.TwentyFiveHz },
            new LogFrequencyViewModel { Name = "50 Hz", Rate = LoggingRate.FiftyHz },
            new LogFrequencyViewModel { Name = "100 Hz", Rate = LoggingRate.HundredHz },
        ];

        selectedFrequency = Frequencies[2];
        SelectedFrequency = Frequencies.FirstOrDefault(f => f.Rate == logEntry.Rate) ?? Frequencies[2];

        ChannelViewModel = new ChannelSelectionControlViewModel(channelProvider)
        {
            SelectedChannelMapping = channelProvider.GetChannel(logEntry.ChannelId),
            IsSelectable = false
        };
    }
}

public class LogFrequencyViewModel
{
    public string Name { get; set; } = string.Empty;
    public LoggingRate Rate { get; set; }
}