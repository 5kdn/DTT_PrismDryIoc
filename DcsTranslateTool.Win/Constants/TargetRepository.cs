namespace DcsTranslateTool.Win.Constants;
/// <summary>
/// GitHub リポジトリに関する固定情報を保持するクラス。
/// <para>
/// このクラスは DcsTranslateTool が利用する GitHub リポジトリのオーナー名、リポジトリ名を提供する。
/// </para>
/// </summary>
public static class TargetRepository {
    private const string owner = "5kdn";
    private const string repo = "test_DCS";

    /// <summary>
    /// GitHub リポジトリの所有者名。
    /// </summary>
    /// <returns>
    /// リポジトリの所有者。
    /// </returns>
    public static string Owner { get => owner; }

    /// <summary>
    /// GitHub リポジトリ名。
    /// </summary>
    /// <returns>
    /// リポジトリ名。
    /// </returns>
    public static string Repo { get => repo; }
}
