using Avalonia.Controls;

namespace BigMission.RedMist.Config.UI.Shared.AimCanMessageConstructor
{
    public partial class AimCan : UserControl
    {
        private readonly AimCanViewModel viewModel = new();
        public AimCan()
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
