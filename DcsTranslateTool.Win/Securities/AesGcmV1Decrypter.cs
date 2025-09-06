using System.Security.Cryptography;
using System.Text;

using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Securities;
using DcsTranslateTool.Win.Exceptions;
using DcsTranslateTool.Win.Helpers;

namespace DcsTranslateTool.Win.Securities;

/// <summary>
/// AES-GCM v1 アルゴリズムで復号するクラス
/// </summary>
public sealed class AesGcmV1Decrypter : IDecrypter {
    /// <inheritdoc/>
    public string Algorithm => "aes-gcm:v1";

    /// <inheritdoc/>
    public string Decrypt( string payload, byte[] aad ) {
        if(string.IsNullOrEmpty( payload )) throw new ArgumentNullException( nameof( payload ) );
        ArgumentNullException.ThrowIfNull( aad );
        if(aad.Length < 1) throw new ArgumentException( "AAD の値が空です。", nameof( aad ) );

        try {
            var parts = payload.Split( '.' );
            if(parts.Length != 3) throw new SecretDecryptException( "ペイロード形式が不正です。" );
            byte[] nonce = Base64Url.Decode( parts[0] );
            byte[] cipher = Base64Url.Decode( parts[1] );
            byte[] tag = Base64Url.Decode( parts[2] );
            if(nonce.Length != 12) throw new SecretDecryptException( "nonceの長さが不正です。" );
            if(tag.Length != 16) throw new SecretDecryptException( "tagの長さが不正です。" );
            byte[] key = CryptKey.GetKey();
            if(key.Length is not (16 or 24 or 32))
                throw new SecretDecryptException( "復号鍵の長さが不正です。16/24/32 バイトのいずれかが必要です。" );
            var plaintext = new byte[cipher.Length];

            using var aes = new AesGcm(key, tag.Length);
            aes.Decrypt( nonce, cipher, tag, plaintext, aad );
            return Encoding.UTF8.GetString( plaintext );
        }
        catch(CryptographicException ex) {
            throw new SecretDecryptException( "復号に失敗した。", ex );
        }
        catch(FormatException ex) {
            throw new SecretDecryptException( "ペイロード形式が不正です。", ex );
        }
    }
}