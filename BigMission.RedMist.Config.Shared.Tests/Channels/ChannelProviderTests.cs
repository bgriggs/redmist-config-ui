using BigMission.ChannelManagement.Shared;
using BigMission.RedMist.Config.Shared.Channels;
using Moq;

namespace BigMission.RedMist.Config.Shared.Tests.Channels;

[TestClass]
public class ChannelProviderTests
{
    private ChannelProvider? channelProvider;
    private Mock<IChannelDependencyCheck>? channelDependencyCheckMock;
    private Mock<IDriverSyncConfigurationProvider>? configurationProviderMock;

    [TestInitialize]
    public void TestInitialize()
    {
        channelDependencyCheckMock = new Mock<IChannelDependencyCheck>();
        configurationProviderMock = new Mock<IDriverSyncConfigurationProvider>();
        channelProvider = new ChannelProvider([channelDependencyCheckMock.Object], configurationProviderMock.Object);
    }

    [TestMethod]
    public void GetUnusedChannels_ShouldReturnUnusedChannels()
    {
        // Arrange
        var usedChannels = new HashSet<int> { 1, 2, 3 };
        var channelMappings = new[]
        {
            new ChannelMappingDto { Id = 1 },
            new ChannelMappingDto { Id = 2 },
            new ChannelMappingDto { Id = 3 },
            new ChannelMappingDto { Id = 4 },
            new ChannelMappingDto { Id = 5 }
        };
        var configuration = new MasterDriverSyncConfig { ChannelConfig = new ChannelConfigDto() };
        configuration.ChannelConfig.ChannelMappings.AddRange(channelMappings);
        configurationProviderMock?.Setup(c => c.GetConfiguration()).Returns(configuration);
        channelDependencyCheckMock?.Setup(c => c.GetChannelAssignments()).Returns(usedChannels.Select(c => new ChannelDependency(c, "", "")));

        // Act
        var result = channelProvider?.GetUnusedChannels();

        // Assert
        Assert.AreEqual(2, result?.Length);
        Assert.IsTrue(result?.All(c => !usedChannels.Contains(c.Id)));
    }

    [TestMethod]
    public void GetChannelAssignments_WithChannelId_ShouldReturnAssignmentsForSpecificChannel()
    {
        // Arrange
        var channelId = 1;
        var channelDependencies = new[]
        {
            new ChannelDependency(1, "", ""),
            new ChannelDependency(2, "", ""),
            new ChannelDependency(1, "", ""),
            new ChannelDependency(3, "", ""),
        };

        channelDependencyCheckMock?.Setup(c => c.GetChannelAssignments()).Returns(channelDependencies);

        // Act
        var result = channelProvider?.GetChannelAssignments(channelId);

        // Assert
        Assert.AreEqual(2, result?.Length);
        Assert.IsTrue(result?.All(c => c.ChannelId == channelId));
    }

    [TestMethod]
    public void GetChannelAssignments_WithoutChannelId_ShouldReturnAllAssignments()
    {
        // Arrange
        var channelDependencies = new[]
        {
            new ChannelDependency(1, "", ""),
            new ChannelDependency(2, "", ""),
            new ChannelDependency(3, "", ""),
        };

        channelDependencyCheckMock?.Setup(c => c.GetChannelAssignments()).Returns(channelDependencies);

        // Act
        var result = channelProvider?.GetChannelAssignments();

        // Assert
        Assert.AreEqual(3, result?.Length);
    }

    [TestMethod]
    public void GetChannelDependencies_WithChannelId_ShouldReturnDependenciesForSpecificChannel()
    {
        // Arrange
        var channelId = 1;
        var channelDependencies = new[]
        {
            new ChannelDependency(1, "", ""),
            new ChannelDependency(2, "", ""),
            new ChannelDependency(1, "", ""),
            new ChannelDependency(3, "", ""),
        };

        channelDependencyCheckMock?.Setup(c => c.GetChannelDependencies()).Returns(channelDependencies);

        // Act
        var result = channelProvider?.GetChannelDependencies(channelId);

        // Assert
        Assert.AreEqual(2, result?.Length);
        Assert.IsTrue(result?.All(c => c.ChannelId == channelId));
    }

    [TestMethod]
    public void GetChannelDependencies_WithoutChannelId_ShouldReturnAllDependencies()
    {
        // Arrange
        var channelDependencies = new[]
        {
            new ChannelDependency(1, "", ""),
            new ChannelDependency(2, "", ""),
            new ChannelDependency(3, "", ""),
        };

        channelDependencyCheckMock?.Setup(c => c.GetChannelDependencies()).Returns(channelDependencies);

        // Act
        var result = channelProvider?.GetChannelDependencies();

        // Assert
        Assert.AreEqual(3, result?.Length);
    }


    [TestMethod]
    public void GetChannel_ShouldReturnChannel()
    {
        // Arrange
        var channelMappings = new[]
        {
            new ChannelMappingDto { Id = 1 },
            new ChannelMappingDto { Id = 2 },
            new ChannelMappingDto { Id = 3 },
        };
        var configuration = new MasterDriverSyncConfig { ChannelConfig = new ChannelConfigDto() };
        configuration.ChannelConfig.ChannelMappings.AddRange(channelMappings);
        configurationProviderMock?.Setup(c => c.GetConfiguration()).Returns(configuration);

        // Act
        var result = channelProvider?.GetChannel(1);

        // Assert
        Assert.AreEqual(1, result?.Id);
    }

    [TestMethod]
    public void GetChannel_ShouldReturnNull()
    {
        // Arrange
        var channelMappings = new[]
        {
            new ChannelMappingDto { Id = 1 },
            new ChannelMappingDto { Id = 2 },
            new ChannelMappingDto { Id = 3 },
        };
        var configuration = new MasterDriverSyncConfig { ChannelConfig = new ChannelConfigDto() };
        configuration.ChannelConfig.ChannelMappings.AddRange(channelMappings);
        configurationProviderMock?.Setup(c => c.GetConfiguration()).Returns(configuration);

        // Act
        var result = channelProvider?.GetChannel(10);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetAllChannels_ShouldReturnAllThree()
    {
        // Arrange
        var channelMappings = new[]
        {
            new ChannelMappingDto { Id = 1 },
            new ChannelMappingDto { Id = 2 },
            new ChannelMappingDto { Id = 3 },
        };
        var configuration = new MasterDriverSyncConfig { ChannelConfig = new ChannelConfigDto() };
        configuration.ChannelConfig.ChannelMappings.AddRange(channelMappings);
        configurationProviderMock?.Setup(c => c.GetConfiguration()).Returns(configuration);

        // Act
        var result = channelProvider?.GetAllChannels();

        // Assert
        Assert.AreEqual(3, result?.Length);
    }

    [TestMethod]
    public void GetAllChannels_ShouldReturnEmpty()
    {
        // Arrange
        var channelMappings = Array.Empty<ChannelMappingDto>();
        var configuration = new MasterDriverSyncConfig { ChannelConfig = new ChannelConfigDto() };
        configuration.ChannelConfig.ChannelMappings.AddRange(channelMappings);
        configurationProviderMock?.Setup(c => c.GetConfiguration()).Returns(configuration);

        // Act
        var result = channelProvider?.GetAllChannels();

        // Assert
        Assert.AreEqual(0, result?.Length);
    }
}
