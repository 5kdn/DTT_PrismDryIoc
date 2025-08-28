using DcsTranslateTool.Core.Models;

using FluentResults;

namespace DcsTranslateTool.Core.Contracts.Services;
public interface IFileEntryService {
    /// <summary>
    /// ディレクトリの子要素を取得する。
    /// ディレクトリでない場合や取得に失敗した場合はエラー情報を返却する。
    /// </summary>
    /// <param name="entry">取得したい親ディレクトリ</param>
    /// <returns>子要素の<see cref="Entry"/>配列を含むResult、失敗時はエラー情報を返す</returns>
    Result<IEnumerable<Entry>> GetChildren( Entry entry );
}