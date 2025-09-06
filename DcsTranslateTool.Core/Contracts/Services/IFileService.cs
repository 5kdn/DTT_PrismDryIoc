namespace DcsTranslateTool.Core.Contracts.Services;

/// <summary>
/// ファイル操作を提供する <see cref="FileService"/> の契約を定義するインターフェース
/// </summary>
public interface IFileService {
    /// <summary>
    /// JSON ファイルからオブジェクトを読み込む
    /// </summary>
    /// <typeparam name="T">戻り値の型</typeparam>
    /// <param name="folderPath">フォルダパス</param>
    /// <param name="fileName">ファイル名</param>
    /// <returns>読み込んだオブジェクト。存在しない場合は <see langword="default"/></returns>
    /// <exception cref="ArgumentException">folderPath または fileName が null もしくは空の場合</exception>
    /// <exception cref="IOException">ファイルの読み込みに失敗した場合</exception>
    /// <exception cref="JsonException">JSON の解析に失敗した場合</exception>
    T? ReadFromJson<T>( string folderPath, string fileName );

    /// <summary>
    /// オブジェクトを JSON ファイルとして保存する
    /// </summary>
    /// <typeparam name="T">保存対象の型</typeparam>
    /// <param name="folderPath">フォルダパス</param>
    /// <param name="fileName">ファイル名</param>
    /// <param name="content">保存する内容</param>
    /// <exception cref="ArgumentException">folderPath または fileName が null もしくは空の場合</exception>
    /// <exception cref="IOException">ファイルの保存に失敗した場合</exception>
    void SaveToJson<T>( string folderPath, string fileName, T content );

    /// <summary>
    /// 指定ファイルを削除する
    /// </summary>
    /// <param name="folderPath">フォルダパス</param>
    /// <param name="fileName">ファイル名</param>
    /// <exception cref="ArgumentException">folderPath または fileName が null もしくは空の場合</exception>
    /// <exception cref="IOException">ファイルの削除に失敗した場合</exception>
    void Delete( string folderPath, string fileName );

    /// <summary>
    /// ファイルに指定した内容を非同期で保存する
    /// </summary>
    /// <param name="path">保存先のファイルパス</param>
    /// <param name="content">保存する内容</param>
    /// <exception cref="ArgumentException">path が null もしくは空の場合</exception>
    /// <exception cref="IOException">ファイルの保存に失敗した場合</exception>
    Task SaveAsync( string path, string content );

    /// <summary>
    /// ファイルに指定した内容を非同期で保存する
    /// </summary>
    /// <param name="path">保存先のファイルパス</param>
    /// <param name="content">保存する内容</param>
    /// <exception cref="ArgumentException">path が null もしくは空の場合</exception>
    /// <exception cref="IOException">ファイルの保存に失敗した場合</exception>
    Task SaveAsync( string path, byte[] content );
}