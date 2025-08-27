namespace DcsTranslateTool.Win.Exceptions;
/// <summary>
/// 秘密情報が見つからない場合の例外。
/// </summary>
public sealed class SecretNotFoundException : Exception {
    /// <summary>
    /// 例外を生成する。
    /// </summary>

    public SecretNotFoundException() : base() { }
    public SecretNotFoundException( string? message ) : base( message ) { }
    public SecretNotFoundException( string message, Exception? inner = null ) : base( message, inner ) { }
}