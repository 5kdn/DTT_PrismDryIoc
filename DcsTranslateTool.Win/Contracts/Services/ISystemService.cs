namespace DcsTranslateTool.Win.Contracts.Services;

/// <summary>
/// システム操作を提供するサービスの契約
/// </summary>
public interface ISystemService {
    /// <summary>
    /// 既定のブラウザーで指定 URL を開く
    /// </summary>
    /// <param name="url">URL</param>
    void OpenInWebBrowser( string url );

    /// <summary>
    /// 指定されたパスをエクスプローラーで開く。
    /// パスがディレクトリでない場合は、ファイルが存在する場合にその親ディレクトリを開く。
    /// </summary>
    /// <param name="path">開く対象のディレクトリまたはファイルのパス。</param>
    /// <exception cref="DirectoryNotFoundException">
    /// 指定されたパスが存在せず、またファイルの親ディレクトリも取得できなかった場合に発生する。
    /// </exception>
    /// <exception cref="System.ComponentModel.Win32Exception">
    /// エクスプローラーの起動に失敗した場合に発生する。
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// プロセスオブジェクトが既に破棄されている場合に発生する。
    /// </exception>
    void OpenDirectory( string path );
}