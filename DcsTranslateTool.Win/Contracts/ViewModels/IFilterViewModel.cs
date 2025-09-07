using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.Contracts.ViewModels;

/// <summary>
/// ファイルの変更種別によるフィルタ状態を表す ViewModel のインターフェースである。
/// </summary>
public interface IFilterViewModel {
    /// <summary>すべての項目を対象とするかどうかを取得または設定するプロパティである。</summary>
    bool All { get; set; }

    /// <summary>変更なしの項目を含めるかどうかを取得または設定するプロパティである。</summary>
    bool Unchanged { get; set; }

    /// <summary>リポジトリ上にしか無い項目を含めるかどうかを取得または設定するプロパティである。</summary>
    bool RepoOnly { get; set; }

    /// <summary>ローカル上にしか無いを含めるかどうかを取得または設定するプロパティである。</summary>
    bool LocalOnly { get; set; }

    /// <summary>変更された項目を含めるかどうかを取得または設定するプロパティである。</summary>
    bool Modified { get; set; }

    /// <summary>フィルタ状態が変更されたときに通知するイベントである。</summary>
    event EventHandler? FiltersChanged;

    /// <summary>有効なフィルタを列挙するメソッドである。</summary>
    /// <returns>選択されている<see cref="FileChangeType"/>の列挙。</returns>
    IEnumerable<FileChangeType?> GetActiveTypes();
}