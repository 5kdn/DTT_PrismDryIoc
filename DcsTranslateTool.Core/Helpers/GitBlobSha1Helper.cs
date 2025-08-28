using System.Security.Cryptography;
using System.Text;

namespace DcsTranslateTool.Core.Helpers;

/// <summary>
/// GitのBlobオブジェクトにおけるSHA1を計算するヘルパーである。
/// </summary>
public static class GitBlobSha1Helper {
    /// <summary>
    /// 指定したファイルのBlob-SHA1を計算する。
    /// ファイルがロックされている場合は<see langword="null"/>を返す。
    /// </summary>
    /// <param name="filePath">対象ファイルのパス</param>
    /// <returns>計算されたSHA1。読み取り不可の場合は<see langword="null"/></returns>
    public static string? Calculate( string filePath ) {
        try {
            using FileStream stream = new( filePath, FileMode.Open, FileAccess.Read, FileShare.Read );
            byte[] content = new byte[stream.Length];
            stream.ReadExactly( content );
            string header = $"blob {content.Length}\0";
            byte[] data = Encoding.UTF8.GetBytes( header ).Concat( content ).ToArray();
            using SHA1 sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash( data );
            return Convert.ToHexString( hash ).ToLowerInvariant();
        }
        catch( IOException ) {
            return null;
        }
    }
}
