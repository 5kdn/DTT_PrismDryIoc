namespace DcsTranslateTool.Win.Exceptions;
/// <summary>
/// 秘密情報の復号に失敗した場合の例外。
/// </summary>
public sealed class SecretDecryptException : Exception {
    /// <summary>
    /// 例外を生成する。
    /// </summary>

    public SecretDecryptException() : base() { }
    public SecretDecryptException( string? message ) : base( message ) { }
    public SecretDecryptException( string message, Exception? inner = null ) : base( message, inner ) { }
}