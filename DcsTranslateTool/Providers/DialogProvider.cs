using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using DcsTranslateTool.Contracts.Providers;

namespace DcsTranslateTool.Providers;

/// <summary>
/// フォルダ選択ダイアログを提供するサービス
/// </summary>
public class DialogProvider : IDialogProvider
{
    /// <inheritdoc />
    public bool ShowFolderPicker( string initialDirectory, out string? selectedPath )
    {
        selectedPath = null;
        try
        {
            using var dialog = new FolderBrowserDialog();
            if(!string.IsNullOrWhiteSpace( initialDirectory ))
            {
                dialog.InitialDirectory = initialDirectory;
            }

            var result = dialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                selectedPath = dialog.SelectedPath;
                return true;
            }

            return false;
        }
        catch(COMException)
        {
            selectedPath = null;
            return false;
        }
        catch(Exception)
        {
            selectedPath = null;
            return false;
        }
    }
}
