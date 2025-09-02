using System.Globalization;
using System.Windows.Data;

using DcsTranslateTool.Win.Enums;

using MahApps.Metro.IconPacks;

namespace DcsTranslateTool.Win.Converters;

/// <summary>
/// ファイルの種類と変更種別からアイコンを取得するコンバーターである。
/// </summary>
public class FileEntryToIconKindConverter : IMultiValueConverter {
    /// <summary>
    /// ファイル種別と変更種別からアイコンを返す。
    /// </summary>
    /// <param name="values">[0] がディレクトリかどうか、[1] が変更種別、[2] がページ判定フラグである。</param>
    /// <param name="targetType">変換後の型</param>
    /// <param name="parameter">未使用</param>
    /// <param name="culture">カルチャ</param>
    /// <returns>アイコン種別</returns>
    public object Convert( object[] values, Type targetType, object? parameter, CultureInfo culture ) {
        if(
            values.Length == 3 &&
            values[0] is bool isDirectory &&
            values[1] is FileChangeType changeType &&
            values[2] is ChangeTypeMode mode
            ) {
            return (isDirectory, changeType, mode) switch
            {
                // DL済みで変更なし
                (true, FileChangeType.Unchanged, _ ) => PackIconMaterialKind.FolderCheckOutline,
                (false, FileChangeType.Unchanged, _ ) => PackIconMaterialKind.FileCheckOutline,
                // 未DL
                (true, FileChangeType.RepoOnly, ChangeTypeMode.Download ) => PackIconMaterialKind.FolderDownloadOutline,
                (false, FileChangeType.RepoOnly, ChangeTypeMode.Download ) => PackIconMaterialKind.FileDownloadOutline,
                (true, FileChangeType.RepoOnly, ChangeTypeMode.Upload ) => PackIconMaterialKind.FolderRemoveOutline,
                (false, FileChangeType.RepoOnly, ChangeTypeMode.Upload ) => PackIconMaterialKind.FileRemoveOutline,
                // リポジトリに存在せず、ローカルに有る
                (true, FileChangeType.LocalOnly, ChangeTypeMode.Download ) => PackIconMaterialKind.FolderRemoveOutline,
                (false, FileChangeType.LocalOnly, ChangeTypeMode.Download ) => PackIconMaterialKind.FileRemoveOutline,
                (true, FileChangeType.LocalOnly, ChangeTypeMode.Upload ) => PackIconMaterialKind.FolderUploadOutline,
                (false, FileChangeType.LocalOnly, ChangeTypeMode.Upload ) => PackIconMaterialKind.FileUploadOutline,
                // 変更差分有り
                (true, FileChangeType.Modified, _ ) => PackIconMaterialKind.FolderAlertOutline,
                (false, FileChangeType.Modified, _ ) => PackIconMaterialKind.FileAlertOutline,
                // デフォルト・読み込み失敗
                (true, _, _ ) => PackIconMaterialKind.FolderQuestion,
                (false, _, _ ) => PackIconMaterialKind.FileQuestion,
            };
        }
        return PackIconMaterialKind.FileQuestion;
    }

    /// <summary>
    /// 変換結果から元の値を取得する。
    /// </summary>
    /// <param name="value">変換後の値</param>
    /// <param name="targetTypes">変換前の型配列</param>
    /// <param name="parameter">未使用</param>
    /// <param name="culture">カルチャ</param>
    /// <returns>変換結果</returns>
    public object[] ConvertBack( object value, Type[] targetTypes, object? parameter, CultureInfo culture ) {
        throw new NotImplementedException();
    }
}