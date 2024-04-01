using BigMission.RedMist.Config.Shared;
using BigMission.RedMist.Config.Shared.CanBus;
using BigMission.RedMist.Config.Shared.Channels;
using BigMission.RedMist.Config.UI.Shared.CanBus;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace BigMission.RedMist.Config.UI.Shared.Tests.CanBus;

[TestClass]
public class CanChannelSelectionDialogViewModelTests
{
    private ChannelProvider? channelProvider;
    private Mock<IChannelDependencyCheck>? channelDependencyCheckMock;
    private Mock<IDriverSyncConfigurationProvider>? configurationProviderMock;
    private CanChannelAssignmentConfigDto? _channelConfig;
    private CanChannelSelectionDialogViewModel? viewModel;

    [TestInitialize]
    public void SetUp()
    {
        channelDependencyCheckMock = new Mock<IChannelDependencyCheck>();
        configurationProviderMock = new Mock<IDriverSyncConfigurationProvider>();
        configurationProviderMock.Setup(x => x.GetConfiguration()).Returns(new MasterDriverSyncConfig());
        channelProvider = new ChannelProvider([channelDependencyCheckMock.Object], configurationProviderMock.Object);
        _channelConfig = new CanChannelAssignmentConfigDto();
        viewModel = new CanChannelSelectionDialogViewModel(channelProvider, _channelConfig, new CanMessageConfigDto());
    }

    [TestMethod]
    public void Length_SetValue_InvalidLengthValidationFailed()
    {
        // Arrange
        viewModel!.Offset = 2;

        // Act
        viewModel.Length = 3;

        // Assert
        Assert.IsTrue(viewModel.HasErrors);
    }

    [TestMethod]
    public void Length_SetValue_ValidLengthValidationPassed()
    {
        // Arrange
        viewModel!.Offset = 2;

        // Act
        viewModel.Length = 4;

        // Assert
        Assert.IsFalse(viewModel.HasErrors);
    }

    [TestMethod]
    public void ValidateLength_ValidLength_ReturnsSuccess()
    {
        // Arrange
        int length = 4;
        var validationContext = new ValidationContext(viewModel!);

        // Act
        var result = CanChannelSelectionDialogViewModel.ValidateLength(length, validationContext);

        // Assert
        Assert.AreEqual(ValidationResult.Success, result);
    }

    [TestMethod]
    public void ValidateLength_InvalidLength_ReturnsErrorMessage()
    {
        // Arrange
        viewModel!.Length = 3;
        viewModel!.Offset = 2;
        var validationContext = new ValidationContext(viewModel!);

        // Act
        var result = CanChannelSelectionDialogViewModel.ValidateLength(viewModel!.Length, validationContext);

        // Assert
        Assert.AreEqual("Length must be: 1,2,4", result.ErrorMessage);
    }


    [TestMethod]
    public void Mask_SetValue_ValidMaskValidationPassed_1byte()
    {
        // Arrange
        string validMask = "FF";

        // Act
        viewModel!.Mask = validMask;

        // Assert
        Assert.AreEqual(validMask, viewModel.Mask);
    }

    [TestMethod]
    public void Mask_SetValue_ValidMaskValidationPassed_2bytes()
    {
        // Arrange
        string validMask = "FFFF";
        viewModel!.Length = 2;

        // Act
        viewModel!.Mask = validMask;

        // Assert
        Assert.IsFalse(viewModel.HasErrors);
    }

    [TestMethod]
    public void Mask_SetValue_ValidMaskValidationPassed_4bytes()
    {
        // Arrange
        string validMask = "FFFFFFFF";
        viewModel!.Length = 4;

        // Act
        viewModel!.Mask = validMask;

        // Assert
        Assert.IsFalse(viewModel.HasErrors);
    }

    [TestMethod]
    public void Mask_SetValue_ValidMaskValidationPassed_8bytes()
    {
        // Arrange
        string validMask = "FFFFFFFFFFFFFFFF";
        viewModel!.Length = 8;

        // Act
        viewModel!.Mask = validMask;

        // Assert
        Assert.IsFalse(viewModel.HasErrors);
    }

    [TestMethod]
    public void Mask_SetValue_InvalidMaskValidationFailed()
    {
        // Arrange
        string invalidMask = "GG";

        // Act
        viewModel!.Mask = invalidMask;

        // Assert
        Assert.IsTrue(viewModel.HasErrors);
    }
}