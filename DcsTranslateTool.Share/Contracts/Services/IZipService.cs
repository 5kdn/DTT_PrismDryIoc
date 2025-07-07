namespace DcsTranslateTool.Share.Contracts.Services;

/// <summary>
/// zip ファイル操作を提供するサービスインターフェースである
/// </summary>
public interface IZipService
{
    /// <summary>
    /// ファイルを zip に追加する
    /// </summary>
    /// <param name="zipFilePath">対象の zip ファイルパス</param>
    /// <param name="filePath">追加するファイルパス</param>
    /// <param name="entryPath">zip 内でのパス</param>
    /// <exception cref="ArgumentException">引数が null または空の場合</exception>
    /// <exception cref="FileNotFoundException">追加するファイルが存在しない場合</exception>
    /// <exception cref="InvalidDataException">zip ファイルが壊れている場合</exception>
    void AddFile(string zipFilePath, string filePath, string entryPath);

    /// <summary>
    /// バイト列を zip に追加する
    /// </summary>
    /// <param name="zipFilePath">対象の zip ファイルパス</param>
    /// <param name="data">追加するデータ</param>
    /// <param name="entryPath">zip 内でのパス</param>
    /// <exception cref="ArgumentException">引数が null または空の場合</exception>
    /// <exception cref="InvalidDataException">zip ファイルが壊れている場合</exception>
    void AddBytes(string zipFilePath, byte[] data, string entryPath);

    /// <summary>
    /// zip からエントリを削除する
    /// </summary>
    /// <param name="zipFilePath">対象の zip ファイルパス</param>
    /// <param name="targetPath">削除するファイルまたはディレクトリパス</param>
    /// <exception cref="ArgumentException">引数が null または空の場合</exception>
    /// <exception cref="InvalidDataException">zip ファイルが壊れている場合</exception>
    void DeleteEntry(string zipFilePath, string targetPath);

    /// <summary>
    /// zip のエントリ一覧を取得する
    /// </summary>
    /// <param name="zipFilePath">対象の zip ファイルパス</param>
    /// <returns>エントリパスの一覧</returns>
    /// <exception cref="ArgumentException">引数が null または空の場合</exception>
    /// <exception cref="InvalidDataException">zip ファイルが壊れている場合</exception>
    IReadOnlyList<string> GetEntryNames(string zipFilePath);
}
