using System.Collections.Generic;
using System.Linq;

using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Core.Helpers;

/// <summary>
/// エントリの集合を比較し差分情報を付与するヘルパーである。
/// </summary>
public static class EntryComparisonHelper {
    /// <summary>
    /// ローカルとリポジトリのエントリを結合する。
    /// 同じパスのエントリはSHA1情報を統合する。
    /// </summary>
    /// <param name="localEntries">ローカルのエントリ一覧</param>
    /// <param name="repoEntries">リポジトリのエントリ一覧</param>
    /// <returns>統合されたエントリ一覧</returns>
    public static IEnumerable<Entry> Merge( IEnumerable<Entry> localEntries, IEnumerable<Entry> repoEntries ) {
        Dictionary<string, Entry> result = localEntries.ToDictionary(
            e => e.Path,
            e => new Entry( e.Name, e.Path, e.IsDirectory, e.LocalSha, null )
        );

        foreach( Entry repo in repoEntries ) {
            if( result.TryGetValue( repo.Path, out Entry? local ) ) {
                result[repo.Path] = new Entry( repo.Name, repo.Path, repo.IsDirectory, local.LocalSha, repo.RepoSha );
            }
            else {
                result[repo.Path] = new Entry( repo.Name, repo.Path, repo.IsDirectory, null, repo.RepoSha );
            }
        }

        return result.Values;
    }
}
