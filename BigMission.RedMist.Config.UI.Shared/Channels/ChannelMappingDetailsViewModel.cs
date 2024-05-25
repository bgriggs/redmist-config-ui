using BigMission.Avalonia.Utilities;
using BigMission.ChannelManagement;
using BigMission.RedMist.Config.Shared.Channels;
using System.Collections.Immutable;

namespace BigMission.RedMist.Config.UI.Shared.Channels;

public partial class ChannelMappingDetailsViewModel : ChannelMappingEditViewModel
{
    // Readonly details
    public LargeObservableCollection<ChannelDependency> Sources { get; } = [];
    public LargeObservableCollection<ChannelDependency> Dependencies { get; } = [];

    public ChannelMappingDetailsViewModel(ChannelMappingDto data, ImmutableArray<ChannelMappingRowViewModel> parentChannels, ChannelProvider? channelProvider) : base(data, parentChannels, channelProvider)
    {
        if (channelProvider is not null && data.Id > 0)
        {
            InitializeChannelMetadata(channelProvider);
        }
    }

    private void InitializeChannelMetadata(ChannelProvider channelProvider)
    {
        Sources.Clear();
        var assignments = channelProvider.GetChannelAssignments(Data.Id);
        Sources.AddRange(assignments);

        Dependencies.Clear();
        var dependencies = channelProvider.GetChannelDependencies(Data.Id);
        Dependencies.AddRange(dependencies);
    }
}
