using Avalonia.Controls;
using BigMission.RedMist.Config.Shared.CanBus;

namespace BigMission.RedMist.Config.UI.Shared.CanBus
{
    public partial class CanBusView : UserControl
    {
        private readonly CanBusViewModel viewModel = new();
        public CanBusView()
        {
            InitializeComponent();
            var dto = new CanBusConfigDto();
            // TODO: remove test data
            dto.Messages.Add(new CanMessageConfigDto { CanId = 0x01 });
            dto.Messages.Add(new CanMessageConfigDto { CanId = 0xFF1 });
            viewModel.Data = dto;
            DataContext = viewModel;
        }
    }
}
