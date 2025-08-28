namespace DcsTranslateTool.Core.Models;

/// <summary>
/// ファイルまたはディレクトリのエントリを表現するクラスである。
/// </summary>
public class Entry( string name, string absolutePath, bool isDirectory ) {
    /// <summary>
    /// ファイルまたはディレクトリの名称を取得する。
    /// </summary>
    public string Name => name;

    /// <summary>
    /// ファイルまたはディレクトリの絶対パスを取得する。
    /// </summary>
    public string AbsolutePath => absolutePath;

    /// <summary>
    /// ディレクトリかどうかを取得する。
    /// ディレクトリの場合は <see langword="true"/>
    /// ファイルの場合は <see langword="false"/> とする。
    /// </summary>
    public bool IsDirectory => isDirectory;
}
