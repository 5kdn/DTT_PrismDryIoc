using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Core.Helpers;

/// <summary>
/// エントリの集合を比較し差分情報を付与するヘルパーである。
/// </summary>
public static class FileEntryComparisonHelper {
    /// <summary>
    /// ローカルとリポジトリのエントリを結合する。
    /// 同じパスのエントリはSHA1情報を統合する。
    /// </summary>
    /// <param name="localEntries">ローカルのエントリ一覧</param>
    /// <param name="repoEntries">リポジトリのエントリ一覧</param>
    /// <returns>統合されたエントリ一覧</returns>
    public static IEnumerable<FileEntry> Merge( IEnumerable<FileEntry> localEntries, IEnumerable<FileEntry> repoEntries ) {
        Dictionary<string, FileEntry> result = [];

        foreach(FileEntry repo in repoEntries) result[repo.Path] = repo;

        foreach(FileEntry local in localEntries) {
            // リポジトリに存在する場合はlocalShaを追加する
            if(result.TryGetValue( local.Path, out FileEntry? repo )) {
                if(repo is null) continue;
                result[local.Path].LocalSha = local.LocalSha;
            }
            // リポジトリに存在しない場合はlocalを追加する
            else {
                result[local.Path] = local;
            }
        }

        return result.Values;
    }
}