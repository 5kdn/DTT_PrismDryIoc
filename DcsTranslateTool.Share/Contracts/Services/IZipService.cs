namespace DcsTranslateTool.Share.Contracts.Services;

/// <summary>
/// zip ファイル操作を提供するサービスインターフェースである
/// </summary>
public interface IZipService {
    /// <summary>
    /// zip のエントリ一覧を取得する
    /// </summary>
    /// <param name="zipFilePath">対象の zip ファイルパス</param>
    /// <returns>エントリパスの一覧</returns>
    /// <exception cref="ArgumentException">引数が null または空の場合</exception>
    /// <exception cref="FileNotFoundException">zip ファイルが存在しない場合</exception>
    /// <exception cref="InvalidDataException">zip ファイルが壊れている場合</exception>
    /// <exception cref="IOException">ファイル入出力に失敗した場合</exception>
    IReadOnlyList<string> GetEntries( string zipFilePath );

    /// <summary>
    /// ファイルを zip に追加する
    /// </summary>
    /// <param name="zipFilePath">対象の zip ファイルパス</param>
    /// <param name="entryPath">zip 内でのパス</param>
    /// <param name="filePath">追加するファイルパス</param>
    /// <exception cref="FileNotFoundException">zip ファイルまたは追加するファイルが存在しない場合</exception>
    /// <exception cref="ArgumentException">引数が null または空の場合</exception>
    /// <exception cref="InvalidDataException">zip ファイルが壊れている場合</exception>
    /// <exception cref="IOException">ファイル入出力に失敗した場合</exception>
    void AddEntry( string zipFilePath, string entryPath, string filePath );

    /// <summary>
    /// バイト列を zip に追加する
    /// </summary>
    /// <param name="zipFilePath">対象の zip ファイルパス</param>
    /// <param name="entryPath">zip 内でのパス</param>
    /// <param name="data">追加するbyte[]データ</param>
    /// <exception cref="FileNotFoundException">zip ファイルが存在しない場合</exception>
    /// <exception cref="ArgumentException">引数が null または空の場合</exception>
    /// <exception cref="InvalidDataException">zip ファイルが壊れている場合</exception>
    /// <exception cref="IOException">ファイル入出力に失敗した場合</exception>
    void AddEntry( string zipFilePath, string entryPath, byte[] data );

    /// <summary>
    /// zip からエントリを削除する
    /// </summary>
    /// <param name="zipFilePath">対象の zip ファイルパス</param>
    /// <param name="targetPath">削除するファイルまたはディレクトリパス</param>
    /// <exception cref="FileNotFoundException">zip ファイルが存在しない場合</exception>
    /// <exception cref="ArgumentException">引数が null または空の場合</exception>
    /// <exception cref="InvalidDataException">zip ファイルが壊れている場合</exception>
    /// <exception cref="IOException">ファイル入出力に失敗した場合</exception>
    void DeleteEntry( string zipFilePath, string targetPath );
}
