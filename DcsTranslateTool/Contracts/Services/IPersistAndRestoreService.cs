namespace DcsTranslateTool.Contracts.Services;

/// <summary>
/// 設定の保存と復元を行うサービスの契約
/// </summary>
public interface IPersistAndRestoreService
{
    /// <summary>
    /// 設定を保存する
    /// </summary>
    void PersistData();

    /// <summary>
    /// 保存した設定を復元する
    /// </summary>
    void RestoreData();
}
