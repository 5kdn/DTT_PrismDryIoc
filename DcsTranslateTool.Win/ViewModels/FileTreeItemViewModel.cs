using System;
using System.Collections.ObjectModel;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// ファイルツリー表示用の ViewModel
/// </summary>
public class FileTreeItemViewModel : BindableBase {
    /// <summary>
    /// 名前
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 絶対パス
    /// </summary>
    public string AbsolutePath { get; }

    /// <summary>
    /// ディレクトリかどうか
    /// </summary>
    public bool IsDirectory { get; }

    /// <summary>
    /// .miz または .pdf ファイルかどうかを取得する
    /// </summary>
    public bool IsHighlightedFile =>
        !IsDirectory &&
        (Name.EndsWith( ".miz", StringComparison.OrdinalIgnoreCase ) ||
         Name.EndsWith( ".pdf", StringComparison.OrdinalIgnoreCase ));

    /// <summary>
    /// 子ノード
    /// </summary>
    public ObservableCollection<FileTreeItemViewModel> Children { get; } = new();

    /// <summary>
    /// <see cref="FileTreeItemViewModel"/> の新しいインスタンスを生成する
    /// </summary>
    /// <param name="tree">基となる <see cref="FileTree"/></param>
    public FileTreeItemViewModel( FileTree tree ) {
        Name = tree.Name;
        AbsolutePath = tree.AbsolutePath;
        IsDirectory = tree.IsDirectory;
    }

    /// <summary>
    /// 子要素を更新する
    /// </summary>
    /// <param name="fileService">ファイルサービス</param>
    public void UpdateChildren( IFileService fileService ) {
        if(!IsDirectory) return;
        FileTree tree = fileService.GetFileTree( AbsolutePath );
        Children.Clear();
        foreach(FileTree child in tree.Children) {
            Children.Add( new FileTreeItemViewModel( child ) );
        }
    }
}
