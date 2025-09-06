using System.Globalization;
using System.Windows.Data;

using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.Converters;

/// <summary>
/// ファイル種別と変更種別から表示色を取得するコンバーターである。
/// </summary>
public class FileChangeTypeToBrushConverter : IMultiValueConverter {
    /// <summary>
    /// ファイル種別と変更種別からブラシを返す。
    /// </summary>
    /// <param name="values">[0] が変更種別、[1] がPage判別フラグである。</param>
    /// <param name="targetType">変換後の型</param>
    /// <param name="parameter">未使用</param>
    /// <param name="culture">カルチャ</param>
    /// <returns>変換結果</returns>
    public object Convert( object[] values, Type targetType, object? parameter, CultureInfo culture ) {
        if(
            values.Length == 2 &&
            values[0] is FileChangeType changeType &&
            values[1] is ChangeTypeMode mode
            ) {
            var key = (changeType, mode) switch
            {
                // DL済みで変更なし
                (FileChangeType.Unchanged, _ ) => "Brush.Filter.Text3",
                // リポジトリに存在し、ローカルに無い
                (FileChangeType.RepoOnly, ChangeTypeMode.Download ) => "Brush.Filter.Text2",
                (FileChangeType.RepoOnly, ChangeTypeMode.Upload ) => "Brush.Filter.Text4",
                // リポジトリに存在せず、ローカルに有る
                (FileChangeType.LocalOnly, ChangeTypeMode.Download ) => "Brush.Filter.Text4",
                (FileChangeType.LocalOnly, ChangeTypeMode.Upload ) => "Brush.Filter.Text2",
                // 変更差分有り
                (FileChangeType.Modified, _ ) => "Brush.Filter.Text1",
                // デフォルト・読み込み失敗
                (_, _ ) => "Brush.Filter.Text3",
            };

            var app = App.Current;
            var brush = app.TryFindResource( key );
            return (object?)brush ?? throw new KeyNotFoundException();
        }
        throw new KeyNotFoundException();
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