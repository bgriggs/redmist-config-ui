using BigMission.RedMist.Config.Shared.CanBus;

namespace BigMission.RedMist.Config.Shared.Tests.CanBus;

[TestClass]
public class CanMessageConfigDtoTests
{
    [TestMethod]
    public void Validate_LengthLessThanOne_ReturnsFalse()
    {
        // Arrange
        var config = new CanMessageConfigDto();
        config.Length = 0;

        // Act
        var result = config.Validate(out var error);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNotNull(error);
    }

    [TestMethod]
    public void Validate_LengthGreaterThanEight_ReturnsFalse()
    {
        // Arrange
        var config = new CanMessageConfigDto();
        config.Length = 9;

        // Act
        var result = config.Validate(out var error);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNotNull(error);
    }

    [TestMethod]
    public void Validate_NoChannelAssignments_ReturnsTrue()
    {
        // Arrange
        var config = new CanMessageConfigDto();
        config.ChannelAssignments.Clear();

        // Act
        var result = config.Validate(out var error);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(string.Empty, error);
    }

    [TestMethod]
    public void Validate_ChannelAssignmentsExceedLength_ReturnsFalse()
    {
        // Arrange
        var config = new CanMessageConfigDto();
        config.Length = 4;
        config.ChannelAssignments.Clear();
        config.ChannelAssignments.Add(new CanChannelAssignmentConfigDto { Offset = 0, Length = 3 });
        config.ChannelAssignments.Add(new CanChannelAssignmentConfigDto { Offset = 3, Length = 3 });

        // Act
        var result = config.Validate(out var error);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNotNull(error);
    }

    [TestMethod]
    public void Validate_OverlappingChannelAssignments_ReturnsFalse()
    {
        // Arrange
        var config = new CanMessageConfigDto();
        config.Length = 8;
        config.ChannelAssignments.Clear();
        config.ChannelAssignments.Add(new CanChannelAssignmentConfigDto { Offset = 0, Length = 4 });
        config.ChannelAssignments.Add(new CanChannelAssignmentConfigDto { Offset = 3, Length = 4 });

        // Act
        var result = config.Validate(out var error);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNotNull(error);
    }
}
