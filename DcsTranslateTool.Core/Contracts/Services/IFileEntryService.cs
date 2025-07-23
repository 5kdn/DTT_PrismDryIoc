using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Core.Contracts.Services;
public interface IFileEntryService {
    /// <summary>
    /// ディレクトリの子要素を取得する
    /// </summary>
    /// <param name="entry">取得したい親ディレクトリ</param>
    /// <returns>子要素の<see cref="FileEntry"/>インスタンス配列</returns>
    IEnumerable<FileEntry> GetChildren( FileEntry entry );
}
