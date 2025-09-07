using DcsTranslateTool.Win.Contracts.Providers;
using DcsTranslateTool.Win.Contracts.Securities;
using DcsTranslateTool.Win.Exceptions;
using DcsTranslateTool.Win.Services;

using Moq;

using Xunit;

namespace DcsTranslateTool.Tests.Win.Services;

/// <summary>
/// <see cref="DecryptService"/> のテストである。
/// </summary>
public class DecryptServiceTests {
    [Fact]
    public void GetMessageは復号結果を返す() {
        // Arrange
        var metadata = new Mock<IMetadataProvider>();
        metadata.Setup( m => m.GetMetadata( "Algo" ) ).Returns( "ALG" );
        metadata.Setup( m => m.GetMetadata( "EncryptedValue" ) ).Returns( "PAY" );
        var decrypter = new Mock<IDecrypter>();
        decrypter.SetupGet( d => d.Algorithm ).Returns( "ALG" );
        decrypter.Setup( d => d.Decrypt( "PAY", It.IsAny<byte[]>() ) ).Returns( "PLAIN" );
        var service = new DecryptService( metadata.Object, [decrypter.Object] );

        // Act
        var message = service.GetMessage();

        // Assert
        Assert.Equal( "PLAIN", message );
    }

    [Fact]
    public void GetMessageは一度のみ復号処理を実行する() {
        // Arrange
        var metadata = new Mock<IMetadataProvider>();
        metadata.Setup( m => m.GetMetadata( "Algo" ) ).Returns( "ALG" );
        metadata.Setup( m => m.GetMetadata( "EncryptedValue" ) ).Returns( "PAY" );
        var decrypter = new Mock<IDecrypter>();
        decrypter.SetupGet( d => d.Algorithm ).Returns( "ALG" );
        decrypter.Setup( d => d.Decrypt( "PAY", It.IsAny<byte[]>() ) ).Returns( "PLAIN" );
        var service = new DecryptService( metadata.Object, [decrypter.Object] );

        // Act
        var message1 = service.GetMessage();
        var message2 = service.GetMessage();

        // Assert
        Assert.Equal( "PLAIN", message1 );
        Assert.Equal( "PLAIN", message2 );
        decrypter.Verify( d => d.Decrypt( "PAY", It.IsAny<byte[]>() ), Times.Once );
    }

    [Fact]
    public void GetMessageはメタデータ取得に失敗したときSecretNotFoundExceptionが送出される() {
        // Arrange
        var metadata = new Mock<IMetadataProvider>();
        metadata.Setup( m => m.GetMetadata( It.IsAny<string>() ) ).Throws<InvalidOperationException>();
        var service = new DecryptService( metadata.Object, [] );

        // Act & Assert
        Assert.Throws<SecretNotFoundException>( () => service.GetMessage() );
    }

    [Fact]
    public void GetMessageは未知のアルゴリズムでSecretDecryptExceptionが送出される() {
        // Arrange
        var metadata = new Mock<IMetadataProvider>();
        metadata.Setup( m => m.GetMetadata( "Algo" ) ).Returns( "UNKNOWN" );
        metadata.Setup( m => m.GetMetadata( "EncryptedValue" ) ).Returns( "PAY" );
        var service = new DecryptService( metadata.Object, [] );

        // Act & Assert
        Assert.Throws<SecretDecryptException>( () => service.GetMessage() );
    }
}