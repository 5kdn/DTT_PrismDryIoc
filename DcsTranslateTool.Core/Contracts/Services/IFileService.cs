namespace DcsTranslateTool.Core.Contracts.Services;

/// <summary>
/// ファイル操作の契約を提供する
/// </summary>
public interface IFileService
{
    /// <summary>
    /// JSON ファイルからオブジェクトを読み込む
    /// </summary>
    /// <typeparam name="T">戻り値の型</typeparam>
    /// <param name="folderPath">フォルダパス</param>
    /// <param name="fileName">ファイル名</param>
    /// <returns>読み込んだオブジェクト。存在しない場合は default</returns>
    T Read<T>( string folderPath, string fileName );

    /// <summary>
    /// オブジェクトを JSON ファイルとして保存する
    /// </summary>
    /// <typeparam name="T">保存対象の型</typeparam>
    /// <param name="folderPath">フォルダパス</param>
    /// <param name="fileName">ファイル名</param>
    /// <param name="content">保存する内容</param>
    void Save<T>( string folderPath, string fileName, T content );

    /// <summary>
    /// 指定ファイルを削除する
    /// </summary>
    /// <param name="folderPath">フォルダパス</param>
    /// <param name="fileName">ファイル名</param>
    void Delete( string folderPath, string fileName );
}
