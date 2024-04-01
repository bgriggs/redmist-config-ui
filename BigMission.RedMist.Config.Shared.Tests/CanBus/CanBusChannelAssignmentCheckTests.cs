using BigMission.RedMist.Config.Shared.CanBus;
using Moq;

namespace BigMission.RedMist.Config.Shared.Tests.CanBus;

[TestClass]
public class CanBusChannelAssignmentCheckTests
{
    [TestMethod]
    public void GetChannelAssignments_ShouldReturnCorrectChannelAssignments()
    {
        // Arrange
        var configurationProviderMock = new Mock<IDriverSyncConfigurationProvider>();
        var configuration = new MasterDriverSyncConfig { CanBusConfigs = [new()] };
        configuration.CanBusConfigs[0].Messages.Add(new CanMessageConfigDto { CanId = 0x123, IsReceive = true });
        configuration.CanBusConfigs[0].Messages[0].ChannelAssignments.Add(new CanChannelAssignmentConfigDto { ChannelId = 1, Offset = 0, Length = 4 });
        configuration.CanBusConfigs[0].Messages[0].ChannelAssignments.Add(new CanChannelAssignmentConfigDto { ChannelId = 3, Offset = 3, Length = 4 });
        
        // Should ignore this message sense it is sending and not assigning any channels
        configuration.CanBusConfigs[0].Messages.Add(new CanMessageConfigDto { CanId = 0x22, IsReceive = false });
        configuration.CanBusConfigs[0].Messages[1].ChannelAssignments.Add(new CanChannelAssignmentConfigDto { ChannelId = 4, Offset = 1, Length = 4 });
        configurationProviderMock.Setup(provider => provider.GetConfiguration()).Returns(configuration);

        var channelAssignmentCheck = new CanBusChannelDependencyCheck(configurationProviderMock.Object);

        // Act
        var channelAssignments = channelAssignmentCheck.GetChannelAssignments().ToArray();

        // Assert
        Assert.IsNotNull(channelAssignments);
        Assert.AreEqual(2, channelAssignments.Length);

        Assert.AreEqual(1, channelAssignments[0].ChannelId);
        Assert.AreEqual("CAN Bus", channelAssignments[0].Area);
        Assert.AreEqual("CAN-::0x123::offset=0", channelAssignments[0].Description);
        Assert.AreEqual(3, channelAssignments[1].ChannelId);
        Assert.AreEqual("CAN Bus", channelAssignments[1].Area);
        Assert.AreEqual("CAN-::0x123::offset=3", channelAssignments[1].Description);
    }

    [TestMethod]
    public void GetChannelDependencies_ShouldReturnCorrectChannelDependencies()
    {
        // Arrange
        var configurationProviderMock = new Mock<IDriverSyncConfigurationProvider>();
        var configuration = new MasterDriverSyncConfig { CanBusConfigs = [new()] };

        // Should ignore this message sense it is receiving and not assigning any channels
        configuration.CanBusConfigs[0].Messages.Add(new CanMessageConfigDto { CanId = 0x123, IsReceive = true });
        configuration.CanBusConfigs[0].Messages[0].ChannelAssignments.Add(new CanChannelAssignmentConfigDto { ChannelId = 1, Offset = 0, Length = 4 });
        configuration.CanBusConfigs[0].Messages[0].ChannelAssignments.Add(new CanChannelAssignmentConfigDto { ChannelId = 3, Offset = 3, Length = 4 });

        // Should use this message sense it is sending and not assigning any channels
        configuration.CanBusConfigs[0].Messages.Add(new CanMessageConfigDto { CanId = 0x22, IsReceive = false });
        configuration.CanBusConfigs[0].Messages[1].ChannelAssignments.Add(new CanChannelAssignmentConfigDto { ChannelId = 4, Offset = 1, Length = 4 });
        configurationProviderMock.Setup(provider => provider.GetConfiguration()).Returns(configuration);

        var channelAssignmentCheck = new CanBusChannelDependencyCheck(configurationProviderMock.Object);

        // Act
        var channelAssignments = channelAssignmentCheck.GetChannelDependencies().ToArray();

        // Assert
        Assert.IsNotNull(channelAssignments);
        Assert.AreEqual(1, channelAssignments.Length);

        Assert.AreEqual(4, channelAssignments[0].ChannelId);
        Assert.AreEqual("CAN Bus", channelAssignments[0].Area);
        Assert.AreEqual("CAN-::0x22::offset=1", channelAssignments[0].Description);
    }
}
