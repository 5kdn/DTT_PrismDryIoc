using System.Collections.ObjectModel;

using DcsTranslateTool.Core.Models;

using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.Contracts.ViewModels;

/// <summary>
/// ファイルまたはディレクトリを表す ViewModel のインターフェース
/// </summary>
public interface IFileEntryViewModel {
    /// <summary>
    /// ファイルまたはディレクトリの名称を取得する
    /// </summary>
    string Name { get; }

    /// <summary>
    /// ファイルまたはディレクトリの絶対パスを取得する
    /// </summary>
    string AbsolutePath { get; }

    /// <summary>
    /// エントリがディレクトリかどうかを示す値を取得する
    /// </summary>
    bool IsDirectory { get; }

    /// <summary>
    /// このViewModelが表すファイルエントリのモデルを取得する
    /// </summary>
    FileEntry Model { get; }

    /// <summary>
    /// チェック状態を取得または設定する
    /// </summary>
    CheckState CheckState { get; set; }

    /// <summary>
    /// 展開状態かどうかを取得または設定する
    /// </summary>
    bool IsExpanded { get; set; }

    /// <summary>
    /// 子エントリの読み込みが完了しているかどうかを取得または設定する
    /// </summary>
    bool IsChildrenLoaded { get; set; }

    /// <summary>
    /// 子エントリのコレクションを取得する
    /// </summary>
    ObservableCollection<IFileEntryViewModel?> Children { get; }

    /// <summary>
    /// 子エントリを読み込む
    /// </summary>
    /// <remarks>
    /// ディレクトリの場合のみ子エントリを読み込む。既に読み込まれている場合は処理しない。
    /// </remarks>
    /// <exception cref="System.Exception">
    /// 子エントリの取得中にエラーが発生した場合にスローされる可能性がある
    /// </exception>
    void LoadChildren();

    /// <summary>
    /// 選択状態の子要素の <see cref="FileEntry"/> を再帰的に取得する
    /// </summary>
    /// <returns>選択状態の <see cref="FileEntry"/> の一覧</returns>
    List<FileEntry> GetCheckedModelRecursice();
}