using DcsTranslateTool.Core.Models;

using FluentResults;

namespace DcsTranslateTool.Core.Contracts.Services;
public interface IFileEntryService {
    /// <summary>
    /// 指定ディレクトリ以下の全てのファイルを再帰的に取得する。
    /// </summary>
    /// <param name="path">探索するルートパス</param>
    /// <returns>フラットな <see cref="FileEntry"/> のコレクション</returns>
    Result<IEnumerable<FileEntry>> GetChildrenRecursive( string path );
}