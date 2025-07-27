namespace DcsTranslateTool.Core.Common;

/// <summary>
/// 操作結果を表すクラス
/// 成功フラグ、値、エラーメッセージを保持
/// </summary>
/// <typeparam name="T">成功時の値の型</typeparam>
/// <param name="isSuccess">操作の成功フラグ</param>
/// <param name="value">成功時の値</param>
/// <param name="error">失敗時のエラーメッセージ</param>
public class Result<T>( bool isSuccess, T? value, string? error ) {
    /// <summary>
    /// 操作の成功フラグ
    /// </summary>
    public bool IsSuccess { get; } = isSuccess;

    /// <summary>
    /// 成功時の値
    /// </summary>
    public T? Value { get; } = value;

    /// <summary>
    /// 失敗時のエラーメッセージ
    /// </summary>
    public string? Error { get; } = error;

    /// <summary>
    /// 指定値での成功結果生成
    /// </summary>
    /// <param name="value">成功時の値</param>
    /// <returns>成功結果の <see cref="Result{T}"/> インスタンス</returns>
    public static Result<T> Success( T value ) => new( true, value, null );

    /// <summary>
    /// 指定エラーメッセージでの失敗結果生成
    /// </summary>
    /// <param name="error">失敗時のエラーメッセージ</param>
    /// <returns>失敗結果の <see cref="Result{T}"/> インスタンス</returns>
    public static Result<T> Failure( string error ) => new( false, default, error );
}
