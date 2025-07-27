using DcsTranslateTool.Core.Common;
using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Core.Contracts.Services;
public interface IFileEntryService {
    /// <summary>
    /// ディレクトリの子要素を取得する
    /// ディレクトリでない場合や取得に失敗した場合はエラー情報を返却する
    /// </summary>
    /// <param name="entry">取得したい親ディレクトリ</param>
    /// <returns>子要素の<see cref="FileEntry"/>インスタンス配列を含むResult、失敗時はエラー情報を返す</returns>
    Result<IEnumerable<FileEntry>> GetChildren( FileEntry entry );
}
