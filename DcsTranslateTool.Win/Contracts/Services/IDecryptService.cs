namespace DcsTranslateTool.Win.Contracts.Services;
/// <summary>
/// 暗号化された環境メッセージを提供するサービスである。
/// </summary>
public interface IDecryptService {
    /// <summary>
    /// 復号済みの環境メッセージを取得する。
    /// </summary>
    /// <returns>復号されたメッセージ</returns>
    string GetMessage();
}