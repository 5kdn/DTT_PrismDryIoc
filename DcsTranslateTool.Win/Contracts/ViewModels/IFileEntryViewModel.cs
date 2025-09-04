﻿using System.Collections.ObjectModel;

using DcsTranslateTool.Core.Models;

using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.Contracts.ViewModels;

/// <summary>
/// ファイルまたはディレクトリを表す ViewModel のインターフェース
/// </summary>
public interface IFileEntryViewModel : IDisposable {
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
    /// このエントリーがディレクトリかどうかを示す値を取得する
    /// </summary>
    bool IsDirectory { get; }

    /// <summary>
    /// この ViewModel が表すエントリーのモデルを取得する。
    /// </summary>
    FileEntry Model { get; }

    /// <summary>
    /// エントリーの変更種別を取得する。
    /// </summary>
    FileChangeType? ChangeType { get; }

    /// <summary>
    /// このエントリーがチェック可能かどうかを示す値を取得する。
    /// </summary>
    bool CanCheck { get; }

    /// <summary>
    /// チェック状態を取得または設定する
    /// </summary>
    bool? CheckState { get; set; }

    /// <summary>
    /// UI 上で選択されているかどうか（チェック状態とは別）を取得または設定する
    /// </summary>
    bool IsSelected { get; set; }

    /// <summary>
    /// 現在の展開状態を取得または設定する
    /// </summary>
    bool IsExpanded { get; set; }

    /// <summary>
    /// フィルタ適用時に表示するかどうかを取得または設定するプロパティである。
    /// </summary>
    bool IsVisible { get; set; }

    /// <summary>
    /// 子ノードのコレクションを取得または設定する
    /// </summary>
    ObservableCollection<IFileEntryViewModel> Children { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// <see cref="IsSelected"/>の状態を再帰的に設定する
    /// </summary>
    /// <param name="value">状態</param>
    void SetSelectRecursive( bool value );

    /// <summary>
    /// 自身および子孫ノードのうち、チェック状態が選択系の要素の <see cref="FileEntry"/> を再帰的に取得する。
    /// </summary>
    /// <param name="fileOnly">true のときファイルのみを対象にする。</param>
    /// <returns>チェック状態が選択系の <see cref="FileEntry"/> のコレクション。</returns>

    List<FileEntry> GetCheckedModelRecursive( bool fileOnly = false );

    /// <summary>
    /// 自身および子孫ノードのうち、チェック状態が選択系の <see cref="IFileEntryViewModel"/> を再帰的に取得する。
    /// </summary>
    /// <param name="fileOnly">true のときファイルのみを対象にする。</param>
    /// <returns>チェック状態が選択系の <see cref="IFileEntryViewModel"/> のコレクション。</returns>
    List<IFileEntryViewModel> GetCheckedViewModelRecursive( bool fileOnly = false );
    #endregion

    #region Events

    /// <summary>
    /// チェック状態が変更されたときに通知されるイベント
    /// </summary>
    event EventHandler<bool?>? CheckStateChanged;

    #endregion

}