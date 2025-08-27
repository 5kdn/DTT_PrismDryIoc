using System.Security.Cryptography;
using System.Text;

using Microsoft.Build.Framework;
namespace BuildTasks;
public sealed class EncryptWithAesGcm : ITask {
    public IBuildEngine? BuildEngine { get; set; }
    public ITaskHost? HostObject { get; set; }
    [Required] public string? Plaintext { get; set; }
    [Required] public string? Algo { get; set; }
    [Required] public string? AssemblyName { get; set; }
    [Required] public string? AssemblyVersion { get; set; }
    [Output] public string Result { get; set; } = string.Empty;
    [Output] public string Aad { get; set; } = string.Empty;

    public bool Execute() {
        if(string.IsNullOrEmpty( Plaintext )) throw new ArgumentNullException( nameof( Plaintext ) );
        if(string.IsNullOrEmpty( Algo )) throw new ArgumentNullException( nameof( Algo ) );
        if(string.IsNullOrEmpty( AssemblyName )) throw new ArgumentNullException( nameof( AssemblyName ) );
        if(string.IsNullOrEmpty( AssemblyVersion )) throw new ArgumentNullException( nameof( AssemblyVersion ) );

        try {
            var aad = Encoding.UTF8.GetBytes(Algo + AssemblyName + AssemblyVersion);
            var key = BuildKey();

            // 追加参照なしで動かすため、AesGcm をリフレクションで取得（byte[] オーバーロードを使用）
            var aesGcmType =
                (Type.GetType("System.Security.Cryptography.AesGcm, System.Security.Cryptography.Algorithms")
                ?? Type.GetType("System.Security.Cryptography.AesGcm, System.Security.Cryptography"))
                ?? throw new InvalidOperationException( "AesGcm is not available on this runtime." );
            var nonce = new byte[12];
            RandomNumberGenerator.Fill( nonce );

            var plaintextBytes = Encoding.UTF8.GetBytes(Plaintext);
            var ciphertext = new byte[plaintextBytes.Length];
            var tag = new byte[16];

            var aesObj = Activator.CreateInstance(aesGcmType, [key])!;
            try {
                var encrypt = aesGcmType.GetMethod(
                    "Encrypt",
                    [typeof(byte[]), typeof(byte[]), typeof(byte[]), typeof(byte[]), typeof(byte[])]) ??
                    throw new MissingMethodException( "AesGcm.Encrypt(byte[], byte[], byte[], byte[], byte[]) not found." );
                encrypt.Invoke( aesObj, [nonce, plaintextBytes, ciphertext, tag, aad] );
            }
            finally {
                (aesObj as IDisposable)?.Dispose();
            }

            var buffer = new byte[2 + nonce.Length + ciphertext.Length + tag.Length];
            buffer[0] = (byte)'v';
            buffer[1] = (byte)'1';
            Buffer.BlockCopy( nonce, 0, buffer, 2, nonce.Length );
            Buffer.BlockCopy( ciphertext, 0, buffer, 2 + nonce.Length, ciphertext.Length );
            Buffer.BlockCopy( tag, 0, buffer, 2 + nonce.Length + ciphertext.Length, tag.Length );

            Result = $"{Base64UrlEncode( nonce )}.{Base64UrlEncode( ciphertext )}.{Base64UrlEncode( tag )}";
            Aad = Base64UrlEncode( aad );
            return true;
        }
        catch {
            return false;
        }
    }

    private static byte[] BuildKey() {
        Span<byte> part1 = [
            0x10,0x22,0x33,0x44,0x55,0x66,0x77,0x88,
            0x99,0xaa,0xbb,0xcc,0xdd,0xee,0xff,0x00,
            0x11,0x22,0x33,0x44,0x55,0x66,0x77,0x88,
            0x99,0xaa,0xbb,0xcc,0xdd,0xee,0xff,0x00 ];
        Span<byte> part2 = [
            0xff,0xee,0xdd,0xcc,0xbb,0xaa,0x99,0x88,
            0x77,0x66,0x55,0x44,0x33,0x22,0x11,0x00,
            0xff,0xee,0xdd,0xcc,0xbb,0xaa,0x99,0x88,
            0x77,0x66,0x55,0x44,0x33,0x22,0x11,0x00 ];
        var key = new byte[32];
        for(var i = 0; i < 32; i++)
            key[i] = (byte)(part1[i] ^ part2[i]);
        return key;
    }

    private static string Base64UrlEncode( byte[] data ) =>
        Convert.ToBase64String( data ).TrimEnd( '=' ).Replace( '+', '-' ).Replace( '/', '_' );
}