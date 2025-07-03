using DcsTranslateTool.Providers;
using Xunit;

namespace DcsTranslateTool.Tests.Providers;

public class EnvironmentProviderTests
{
    [Fact(DisplayName = "環境変数の取得ができる" )]
    public void GetEnvironmentVariable_ShouldReturnValue()
    {
        // Arrange
        var provider = new EnvironmentProvider();
        Environment.SetEnvironmentVariable("TEST_ENV", "VALUE");

        // Act
        var result = provider.GetEnvironmentVariable("TEST_ENV");

        // Assert
        Assert.Equal("VALUE", result);

        Environment.SetEnvironmentVariable("TEST_ENV", null);
    }

    [Fact(DisplayName = "UserProfileを取得できる" )]
    public void GetUserProfilePath_ShouldReturnValue()
    {
        // Arrange
        var provider = new EnvironmentProvider();
        Environment.SetEnvironmentVariable("UserProfile", "C:/Users/Test");

        // Act
        var result = provider.GetUserProfilePath();

        // Assert
        Assert.Equal("C:/Users/Test", result);

        Environment.SetEnvironmentVariable("UserProfile", null);
    }
}
