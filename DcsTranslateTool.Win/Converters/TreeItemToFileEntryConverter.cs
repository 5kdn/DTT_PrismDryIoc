using DcsTranslateTool.Core.Models;

using Octokit;

namespace DcsTranslateTool.Win.Converters;
public static class TreeItemToFileEntryConverter {
    public static FileEntry Convert( TreeItem item ) =>
        new RepoFileEntry(
            item.Path.Split( "/" )[^1],
            item.Path,
            item.Type == TreeType.Tree,
            item.Sha
        );
}