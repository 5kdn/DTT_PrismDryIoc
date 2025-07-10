using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// aircraft ディレクトリに対してファイルパターンでフィルタを適用する <see cref="IFileService"/> の実装である。
/// </summary>
public class FilteringFileService : IFileService {
    private const string ResourceName = "DcsTranslateTool.Win.Resources.AircraftFilePatterns.txt";

    private readonly IFileService _inner;
    private readonly IAppSettingsService _settings;
    private readonly Matcher _matcher = new(StringComparison.OrdinalIgnoreCase);
    private readonly List<string> _matchedFiles = new();
    private string _currentRoot = string.Empty;

    /// <summary>
    /// 新しいインスタンスを生成する。
    /// </summary>
    /// <param name="inner">内部で使用する <see cref="IFileService"/></param>
    /// <param name="settings">アプリ設定サービス</param>
    public FilteringFileService( IFileService inner, IAppSettingsService settings ) {
        _inner = inner;
        _settings = settings;
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream( ResourceName );
        if(stream != null) {
            using var reader = new StreamReader( stream );
            foreach(string line in reader.ReadToEnd().Split( new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries )) {
                _matcher.AddInclude( line.Trim() );
            }
        }
    }

    /// <inheritdoc/>
    public T ReadFromJson<T>( string folderPath, string fileName )
        => _inner.ReadFromJson<T>( folderPath, fileName );

    /// <inheritdoc/>
    public void SaveToJson<T>( string folderPath, string fileName, T content )
        => _inner.SaveToJson( folderPath, fileName, content );

    /// <inheritdoc/>
    public void Delete( string folderPath, string fileName )
        => _inner.Delete( folderPath, fileName );

    /// <inheritdoc/>
    public FileTree GetFileTree( string directoryPath ) {
        FileTree tree = _inner.GetFileTree( directoryPath );
        if(IsUnderAircraftDir( directoryPath )) {
            EnsureMatches();
            tree.Children = tree.Children
                .Where( child => ShouldInclude( child.AbsolutePath, child.IsDirectory ) )
                .ToList();
        }
        return tree;
    }

    /// <inheritdoc/>
    public Task SaveAsync( string path, string content )
        => _inner.SaveAsync( path, content );

    /// <inheritdoc/>
    public Task SaveAsync( string path, byte[] content )
        => _inner.SaveAsync( path, content );

    private bool IsUnderAircraftDir( string path ) {
        var root = _settings.SourceAircraftDir;
        if(string.IsNullOrEmpty( root )) return false;
        var fullRoot = Path.GetFullPath( root );
        var fullPath = Path.GetFullPath( path );
        return fullPath.StartsWith( fullRoot, StringComparison.OrdinalIgnoreCase );
    }

    private void EnsureMatches() {
        var root = _settings.SourceAircraftDir;
        if(_currentRoot == root) return;
        _currentRoot = root ?? string.Empty;
        _matchedFiles.Clear();
        if(string.IsNullOrEmpty( root ) || !Directory.Exists( root )) return;
        var result = _matcher.Execute( new DirectoryInfoWrapper( new DirectoryInfo( root ) ) );
        foreach(var file in result.Files) {
            _matchedFiles.Add( Path.GetFullPath( Path.Combine( root, file.Path ) ) );
        }
    }

    private bool ShouldInclude( string path, bool isDirectory ) {
        string fullPath = Path.GetFullPath( path );
        if(isDirectory) {
            return _matchedFiles.Any( f => f.StartsWith( fullPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase ) );
        }
        else {
            return _matchedFiles.Contains( fullPath );
        }
    }
}
