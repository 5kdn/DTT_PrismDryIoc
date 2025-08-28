using System.IO.Compression;

using DcsTranslateTool.Core.Contracts.Services;

using FluentResults;

namespace DcsTranslateTool.Core.Services;

/// <summary>
/// zip ファイル操作を提供するサービスを実装する
/// </summary>
public class ZipService : IZipService {
    /// <inheritdoc />
    public Result<IReadOnlyList<string>> GetEntries( string zipFilePath ) {
        if(string.IsNullOrWhiteSpace( zipFilePath )) {
            return Result.Fail( $"zip ファイルパスが null または空です: {zipFilePath}" );
        }
        if(!File.Exists( zipFilePath )) Result.Fail( $"ファイルが存在しません{zipFilePath}" );
        try {
            using FileStream fs = new(zipFilePath, FileMode.Open, FileAccess.Read);
            using ZipArchive archive = new(fs, ZipArchiveMode.Read);
            return Result.Ok<IReadOnlyList<string>>( [.. archive.Entries.Select( e => e.FullName )] );
        }
        catch(Exception ex) {
            return Result.Fail( ex.Message );
        }
    }

    /// <inheritdoc />
    public Result AddEntry( string zipFilePath, string entryPath, string filePath ) {
        if(string.IsNullOrWhiteSpace( zipFilePath ))
            return Result.Fail( $"zip ファイルパスが null または空です: {zipFilePath}" );
        if(!File.Exists( zipFilePath ))
            return Result.Fail( $"ファイルが存在しません: {zipFilePath}" );
        if(string.IsNullOrWhiteSpace( filePath ))
            return Result.Fail( $"追加するファイルパスが null または空です: {filePath}" );
        if(!File.Exists( filePath ))
            return Result.Fail( $"ファイルが存在しません: {filePath}" );
        if(string.IsNullOrWhiteSpace( entryPath ))
            return Result.Fail( $"値が null または空です: {nameof( entryPath )}" );

        try {
            using FileStream fs = new(zipFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using ZipArchive archive = new(fs, ZipArchiveMode.Update);
            archive.GetEntry( entryPath )?.Delete();
            archive.CreateEntryFromFile( filePath, entryPath, CompressionLevel.NoCompression );
            return Result.Ok();
        }
        catch(InvalidDataException ex) {
            return Result.Fail( "zip ファイルが壊れている可能性があります" ).WithError( ex.Message );
        }
        catch(IOException ex) {
            return Result.Fail( "zip ファイル書き込み中に入出力エラーが発生しました" ).WithError( ex.Message );
        }
    }

    /// <inheritdoc />
    public Result AddEntry( string zipFilePath, string entryPath, byte[] data ) {
        if(string.IsNullOrWhiteSpace( zipFilePath ))
            return Result.Fail( "zip ファイルパスが null または空です" );
        if(!File.Exists( zipFilePath ))
            return Result.Fail( $"ファイルが存在しません: {zipFilePath}" );
        if(string.IsNullOrWhiteSpace( entryPath ))
            return Result.Fail( "エントリーが null または空です" );
        if(data == null || data.Length == 0)
            return Result.Fail( "追加するデータが null または空です" );

        try {
            using FileStream fs = new(zipFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using ZipArchive archive = new(fs, ZipArchiveMode.Update);
            archive.GetEntry( entryPath )?.Delete();
            ZipArchiveEntry entry = archive.CreateEntry(entryPath, CompressionLevel.NoCompression);
            using Stream entryStream = entry.Open();
            entryStream.Write( data );
            return Result.Ok();
        }
        catch(InvalidDataException ex) {
            return Result.Fail( $"zip ファイルが壊れている可能性があります: {ex.Message}" );
        }
        catch(IOException ex) {
            throw new IOException( $"zip ファイル書き込み中に入出力エラーが発生した: {ex.Message}" );
        }
    }

    /// <inheritdoc />
    public Result DeleteEntry( string zipFilePath, string entryPath ) {
        if(string.IsNullOrWhiteSpace( zipFilePath ))
            return Result.Fail( "zip ファイルパスが空です" );
        if(!File.Exists( zipFilePath ))
            return Result.Fail( $"ファイルが存在しません: {zipFilePath}" );
        if(string.IsNullOrWhiteSpace( entryPath ))
            return Result.Fail( "zip ファイルパスが空です" );

        try {
            using FileStream stream = new(zipFilePath, FileMode.Open, FileAccess.ReadWrite);
            using ZipArchive archive = new(stream, ZipArchiveMode.Update);
            List<ZipArchiveEntry> targets = [.. archive.Entries.Where(e => e.FullName == entryPath || e.FullName.StartsWith(entryPath.TrimEnd('/') + '/'))];
            targets.ForEach( entry => entry.Delete() );
            return Result.Ok();
        }
        catch(InvalidDataException ex) {
            return Result.Fail( $"zip ファイル書き込み中に入出力エラーが発生した: {ex.Message}" );
        }
        catch(IOException ex) {
            return Result.Fail( $"zip ファイル書き込み中に入出力エラーが発生した: {ex.Message}" );
        }
    }
}