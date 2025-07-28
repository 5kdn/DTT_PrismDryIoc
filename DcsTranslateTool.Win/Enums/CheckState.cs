namespace DcsTranslateTool.Win.Enums;

/// <summary>
/// TreeViewの各ノードの選択状態を管理するEnum
/// </summary>
public enum CheckState {
    /// <summary>
    /// 子要素も含め全て選択されていない状態
    /// </summary>
    Unchecked,

    /// <summary>
    /// 子要素も含めて全て選択されている状態
    /// </summary>
    Checked,

    /// <summary>
    /// 子要素の一部が選択されている状態
    /// </summary>
    Indeterminate,
}

