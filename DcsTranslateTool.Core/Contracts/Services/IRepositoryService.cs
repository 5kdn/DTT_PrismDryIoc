using DcsTranslateTool.Core.Models;

using FluentResults;

namespace DcsTranslateTool.Core.Contracts.Services;

/// <summary>
/// GitHub リポジトリ操作のサービスを提供するインターフェース
/// </summary>
public interface IRepositoryService {
    /// <summary>
    /// リポジトリのファイルリスト<see cref="FileEntry"/>を取得する
    /// </summary>
    /// <returns><see cref="FileEntry"/> の列挙結果</returns>
    Task<Result<IEnumerable<FileEntry>>> GetFileEntriesAsync();

    /// <summary>
    /// ファイルをバイナリ形式で取得する
    /// </summary>
    /// <param name="path">ファイルパス</param>
    /// <returns>ファイルのバイト列</returns>
    Task<Result<byte[]>> GetFileAsync( string path );

    /// <summary>
    /// ブランチを作成する
    /// </summary>
    /// <param name="branchName">作成するブランチ名</param>
    Task<Result> CreateBranchAsync( string branchName );

    /// <summary>
    /// ファイルをコミットする
    /// </summary>
    /// <param name="branchName">コミット先ブランチ名</param>
    /// <param name="files">コミットする <see cref="CommitFile"/> のローカルパス一覧</param>
    /// <param name="message">コミットメッセージ</param>
    Task<Result> CommitAsync( string branchName, IEnumerable<CommitFile> files, string message );

    /// <summary>
    /// ファイルをコミットする
    /// </summary>
    /// <param name="branchName">コミット先ブランチ名</param>
    /// <param name="file">コミットする <see cref="CommitFile"/> のローカルパス</param>
    /// <param name="message">コミットメッセージ</param>
    Task<Result> CommitAsync( string branchName, CommitFile file, string message );

    /// <summary>
    /// プルリクエストを作成する
    /// </summary>
    /// <param name="branchName">PR のブランチ名</param>
    /// <param name="title">PR タイトル</param>
    /// <param name="message">PR 本文</param>
    /// <returns>作成されたプルリクエスト</returns>
    Task<Result> CreatePullRequestAsync( string branchName, string title, string message );
}