namespace DcsTranslateTool.Win.Contracts.Securities;
/// <summary>
/// 環境メッセージ復号器のインターフェースである。
/// </summary>
public interface IDecrypter {
    /// <summary>
    /// 対応するアルゴリズム名を取得する。
    /// </summary>
    string Algorithm { get; }

    /// <summary>
    /// 復号処理を実行する。
    /// </summary>
    /// <param name="payload">暗号化されたペイロード</param>
    /// <param name="aad">追加認証データ</param>
    /// <returns>復号された文字列</returns>
    string Decrypt( string payload, byte[] aad );
}