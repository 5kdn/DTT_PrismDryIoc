using System.Reflection;
using System.Text;

using DcsTranslateTool.Win.Contracts.Providers;
using DcsTranslateTool.Win.Contracts.Securities;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Exceptions;

namespace DcsTranslateTool.Win.Services;
public sealed class DecryptService : IDecryptService {
    private readonly IMetadataProvider _metadataProvider;
    private readonly Dictionary<string, IDecrypter> _decrypters;
    private readonly Lazy<string> _message;

    /// <summary>
    /// コンストラクタである。
    /// </summary>
    public DecryptService( IMetadataProvider metadataProvider, IEnumerable<IDecrypter> decrypters ) {
        _metadataProvider = metadataProvider;
        _decrypters = decrypters.ToDictionary( d => d.Algorithm );
        _message = new Lazy<string>( Decrypt, LazyThreadSafetyMode.ExecutionAndPublication );
    }

    /// <inheritdoc/>
    public string GetMessage() => _message.Value;

    private string Decrypt() {
        string payload;
        string algo;
        string name;
        string version;
        try {
            algo = _metadataProvider.GetMetadata( "Algo" );
            payload = _metadataProvider.GetMetadata( "EncryptedValue" );
            var asm = Assembly.GetExecutingAssembly().GetName();
            name = asm.Name ?? throw new InvalidOperationException( nameof( asm.Name ) );
            version = asm.Version?.ToString() ?? throw new InvalidOperationException( nameof( asm.Version ) );
        }
        catch(InvalidOperationException ex) {
            throw new SecretNotFoundException( "メタデータが見つからない。", ex );
        }
        if(!_decrypters.TryGetValue( algo, out var decrypter )) throw new SecretDecryptException( "アルゴリズムが不正である。" );
        var aad = Encoding.UTF8.GetBytes( algo + name + version );
        return decrypter.Decrypt( payload, aad );
    }
}