using DcsTranslateTool.Core.Models;

using Octokit;

namespace DcsTranslateTool.Win.Converters;
public static class TreeItemToEntryConverter {
    public static Entry Convert( TreeItem item ) =>
        new(
            item.Path.Split( "/" )[^1],
            item.Path,
            item.Type == TreeType.Tree,
            null,
            item.Sha
        );
}