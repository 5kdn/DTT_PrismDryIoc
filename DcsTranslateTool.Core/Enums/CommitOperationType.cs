namespace DcsTranslateTool.Core.Enums;

/// <summary>
/// コミットするファイルの操作種別を表す。
/// GitHubへコミットする際、ファイルの追加・更新、または削除を指定する。
/// </summary>
public enum CommitOperationType {
    AddOrUpdate,
    Delete,
}
