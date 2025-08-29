using System.Collections.ObjectModel;

using DcsTranslateTool.Core.Models;

using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.Contracts.ViewModels;

/// <summary>
/// ファイルまたはディレクトリを表す ViewModel のインターフェース
/// </summary>
public interface IFileEntryViewModel {
    #region Properties

    /// <summary>
    /// ファイルまたはディレクトリの名称を取得する
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 翻訳ルートからのパスを取得する
    /// </summary>
    string Path { get; }

    /// <summary>
    /// エントリがディレクトリかどうかを示す値を取得する
    /// </summary>
    bool IsDirectory { get; }

    /// <summary>
    /// このViewModelが表すエントリのモデルを取得する。
    /// </summary>
    FileEntry Model { get; }

    /// <summary>
    /// エントリーの変更種別を取得する。
    /// </summary>
    FileChangeType? ChangeType { get; }

    /// <summary>
    /// チェック状態を取得または設定する
    /// </summary>
    CheckState CheckState { get; set; }

    /// <summary>
    /// チェックボックス選択されているかどうかを取得または設定する
    /// </summary>
    bool IsSelected { get; set; }

    /// <summary>
    /// 現在の展開状態を取得または設定する
    /// </summary>
    bool IsExpanded { get; set; }

    /// <summary>
    /// 子エントリのコレクションを取得する
    /// </summary>
    ObservableCollection<IFileEntryViewModel?> Children { get; }

    /// <summary>
    /// 親ノード（CheckStateの伝播用）
    /// </summary>
    IFileEntryViewModel? Parent { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// <see cref="IsSelected"/>の状態を再帰的に設定する
    /// </summary>
    /// <param name="value">状態</param>
    void SetSelectRecursive( bool value );

    /// <summary>
    /// 選択状態の子要素の <see cref="FileEntry"/> を再帰的に取得する。
    /// </summary>
    /// <param name="fileOnly">true のときファイルだけを対象に取る</param>
    /// <returns>選択状態の <see cref="FileEntry"/> のコレクション</returns>

    List<FileEntry> GetCheckedModelRecursice( bool fileOnly = false );

    /// <summary>
    /// 子要素の状態を確認して自身と親のCheckStateを更新する
    /// </summary>
    void UpdateCheckStateFromChildren();

    #endregion
}