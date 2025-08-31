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
    /// <param name="values">[0] がディレクトリかどうか、[1] が変更種別である。</param>
    /// <param name="targetType">変換後の型</param>
    /// <param name="parameter">未使用</param>
    /// <param name="culture">カルチャ</param>
    /// <returns>アイコン種別</returns>
    public object Convert( object[] values, Type targetType, object? parameter, CultureInfo culture ) {
        if(values.Length == 2 && values[0] is bool isDirectory && values[1] is FileChangeType changeType) {
            return (isDirectory, changeType) switch
            {
                // DL済みで変更なし
                (true, FileChangeType.Unchanged ) => PackIconMaterialKind.FolderCheckOutline,
                (false, FileChangeType.Unchanged ) => PackIconMaterialKind.FileCheckOutline,
                // 未DL
                (true, FileChangeType.RepoOnly ) => PackIconMaterialKind.FolderDownloadOutline,
                (false, FileChangeType.RepoOnly ) => PackIconMaterialKind.FileDownloadOutline,
                // リポジトリに存在せず、ローカルに有る
                (true, FileChangeType.LocalOnly ) => PackIconMaterialKind.FolderRemoveOutline,
                (false, FileChangeType.LocalOnly ) => PackIconMaterialKind.FileRemoveOutline,
                // 変更差分有り
                (true, FileChangeType.Modified ) => PackIconMaterialKind.FolderAlertOutline,
                (false, FileChangeType.Modified ) => PackIconMaterialKind.FileAlertOutline,
                // デフォルト・読み込み失敗
                (true, _ ) => PackIconMaterialKind.FolderQuestion,
                (false, _ ) => PackIconMaterialKind.FileQuestion,
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