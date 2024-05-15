using Avalonia.Controls;

namespace BigMission.RedMist.Config.UI.Shared.Channels
{
    public partial class ChannelSelectionControl : UserControl
    {
        //public static readonly DirectProperty<ChannelSelection, int> ItemsProperty =
        //    AvaloniaProperty.RegisterDirect<ChannelSelection, int>(nameof(SelectedChannelId), o => o.SelectedChannelId, (o, v) => o.SelectedChannelId = v);
        //private int selectedChannelId;

        //public int SelectedChannelId
        //{
        //    get { return selectedChannelId; }
        //    set { SetAndRaise(ItemsProperty, ref selectedChannelId, value); }
        //}

        public ChannelSelectionControl()
        {
            InitializeComponent();
        }
    }
}
