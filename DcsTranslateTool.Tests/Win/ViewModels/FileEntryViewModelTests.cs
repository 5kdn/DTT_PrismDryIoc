using System.ComponentModel;

using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.ViewModels;

using Xunit;

namespace DcsTranslateTool.Tests.Win.ViewModels;
public class FileEntryViewModelTests {
    #region Property: ChangeType
    [Theory]
    [InlineData( "a.txt", "root/a.txt", false, "abc", "abc", FileChangeType.Unchanged )]
    [InlineData( "a.txt", "root/a.txt", false, "abc", "def", FileChangeType.Modified )]
    [InlineData( "a.txt", "root/a.txt", false, "abc", null, FileChangeType.Added )]
    [InlineData( "a.txt", "root/a.txt", false, null, "def", FileChangeType.Deleted )]
    [InlineData( "a.txt", "root/a.txt", false, null, null, FileChangeType.Unchanged )]
    public void ChangeTypeはファイルのときShaによってFileChangeTypeが決定する
        ( string name, string path, bool isDir, string? localSha, string? repoSha, FileChangeType expected ) {
        // Arrange & Act
        var model = new FileEntry(name, path, isDir, localSha, repoSha);
        using var vm = new FileEntryViewModel(model);

        // Assert
        Assert.Equal( expected, vm.ChangeType );
    }

    [Fact]
    public void ChangeTypeはディレクトリかつ子ノードが一つのときに子のChangeTypeを反映する() {
        // Arrange & Act
        var parent = new FileEntry("root", "root", true, null, null);
        using var child = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, "x", "y"));
        using var vm = new FileEntryViewModel(parent){Children = [child]};

        // Assert
        Assert.Equal( FileChangeType.Modified, vm.ChangeType );
    }

    [Fact]
    public void ChangeTypeはディレクトリかつ子ノードが存在しないときにUnchangedになる() {
        // Arrange & Act
        var parent = new FileEntry("root", "root", true, null, null);
        using var vm = new FileEntryViewModel(parent);

        // Assert
        Assert.Equal( FileChangeType.Unchanged, vm.ChangeType );
    }

    [Theory]
    //// 全て同じ
    [InlineData( "x", "x", "y", "y", FileChangeType.Unchanged )]
    [InlineData( "x", "a", "y", "b", FileChangeType.Modified )]
    [InlineData( "x", null, "y", null, FileChangeType.Added )]
    [InlineData( null, "x", null, "y", FileChangeType.Deleted )]
    //// Modified が混ざっていれば Modified が優先される
    [InlineData( "x", "a", "y", "y", FileChangeType.Modified )]
    [InlineData( "x", "a", "y", null, FileChangeType.Modified )]
    [InlineData( "x", "a", null, "y", FileChangeType.Modified )]

    //// Added となにかが混ざっていれば Modified になる
    [InlineData( "x", null, "y", "y", FileChangeType.Modified )]
    [InlineData( "x", null, null, "y", FileChangeType.Modified )]

    //// Deleted と何かが混ざっていれば Modified になる
    [InlineData( null, "x", "y", "y", FileChangeType.Modified )]
    public void ChangeTypeはディレクトリのとき子ノードによって決定する( string? l1, string? r1, string? l2, string? r2, FileChangeType expected ) {
        // Arrange & Act
        var parent = new FileEntry("root", "root", true, null, null);
        var ch1 = new FileEntryViewModel(new("child1.txt", "root/child1.txt", false, l1, r1));
        var ch2 = new FileEntryViewModel(new("child2.txt", "root/child2.txt", false, l2, r2));

        using var vm = new FileEntryViewModel(parent);
        vm.Children = [ch1, ch2];

        // Assert
        Assert.Equal( expected, vm.ChangeType );
    }

    [Fact]
    public void ChangeTypeは子のChangeTypeが変わったときにPropertyChangedを発火する() {
        // Arrange
        var parent = new FileEntry("root", "root", true, null, null);
        using var child = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, "x", "x")); // Unchanged
        using var vm = new FileEntryViewModel(parent) { Children = [child] };

        var notified = false;
        ((INotifyPropertyChanged)vm).PropertyChanged += ( _, e ) => {
            if(e.PropertyName == nameof( FileEntryViewModel.ChangeType )) notified = true;
        };

        // Act
        using var changedChild = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, "x", "y")); // Modified
        vm.Children[0] = changedChild;

        // Assert
        Assert.True( notified );
        Assert.Equal( FileChangeType.Modified, vm.ChangeType );
    }
    #endregion

    #region Property: CheckState
    [Fact]

    public void CheckStateは親にtrueを設定したときに子に伝播する() {
        // Arrange
        var dir = new FileEntry("root", "root", true, null, null);
        using var c1 = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, null, null));
        using var c2 = new FileEntryViewModel(new FileEntry("b.txt", "root/b.txt", false, null, null));
        using var parent = new FileEntryViewModel(dir) { Children = [c1, c2] };

        // Act
        parent.CheckState = true;

        // Assert
        Assert.Equal( true, c1.CheckState );
        Assert.Equal( true, c2.CheckState );
    }

    [Fact]
    public void CheckStateは子が変更されたときに親でCheckStateChangedを発火する() {
        // Arrange
        var dir = new FileEntry("root", "root", true, null, null);
        using var c1 = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, null, null)) { CheckState = false };
        using var parent = new FileEntryViewModel(dir) { Children = [c1] };

        var raised = false;
        parent.CheckStateChanged += ( _, __ ) => raised = true;

        // Act
        c1.CheckState = true;

        // Assert
        Assert.True( raised );
        Assert.Equal( true, parent.CheckState );
    }

    [Theory]
    [InlineData( true, true, true )]
    [InlineData( false, false, false )]
    [InlineData( true, null, null )]
    public void CheckStateは子のCheckStateが親に伝播する( bool? c1State, bool? c2State, bool? expected ) {
        // Arrange
        var dir = new FileEntry("root", "root", true, null, null);
        using var c1 = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, null, null)) { CheckState = c1State };
        using var c2 = new FileEntryViewModel(new FileEntry("b.txt", "root/b.txt", false, null, null)) { CheckState = c2State };
        using var parent = new FileEntryViewModel(dir) { Children = [c1, c2] };

        // Assert
        Assert.Equal( expected, parent.CheckState );
    }

    [Fact]
    public void CheckStateは親にnullを設定したときに子には伝播しない() {
        // Arrange
        var dir = new FileEntry("root", "root", true, null, null);
        using var c1 = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, null, null)) { CheckState = false };
        using var parent = new FileEntryViewModel(dir) { Children = [c1] };

        // Act
        parent.CheckState = null;

        // Assert
        Assert.Equal( false, c1.CheckState );
    }

    #endregion


    #region Property: IsSelected / Method: SetSelectRecursive

    [Fact]
    public void IsSelectedは親にtrueを設定したときに子に伝播する() {
        // Arrange
        var dir = new FileEntry("root", "root", true, null, null);
        using var c1 = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, null, null));
        using var c2 = new FileEntryViewModel(new FileEntry("b.txt", "root/b.txt", false, null, null));
        using var parent = new FileEntryViewModel(dir) { Children = [c1, c2] };

        // Act
        parent.IsSelected = true;

        // Assert
        Assert.True( c1.IsSelected );
        Assert.True( c2.IsSelected );
    }

    // --- エッジケース ---
    [Fact]
    public void SetSelectRecursiveは呼び出したときに全ての子孫を選択状態にする() {
        // Arrange
        var dir = new FileEntry("root", "root", true, null, null);
        using var file = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, null, null));
        using var subDir = new FileEntryViewModel(new FileEntry("sub", "root/sub", true, null, null));
        using var subFile = new FileEntryViewModel(new FileEntry("b.txt", "root/sub/b.txt", false, null, null));
        subDir.Children = [subFile];
        using var parent = new FileEntryViewModel(dir) { Children = [file, subDir] };

        // Act
        parent.SetSelectRecursive( true );

        // Assert
        Assert.True( file.IsSelected );
        Assert.True( subDir.IsSelected );
        Assert.True( subFile.IsSelected );
    }

    #endregion

    #region Method: GetCheckedModelRecursive

    [Fact]
    public void GetCheckedModelRecursiveはチェックの付いた自身と子ノードを返す() {
        // Arrange
        var root = new FileEntry("root", "root", true, null, null);
        using var file1 = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, null, null))
        {CheckState = true};
        using var file2 = new FileEntryViewModel(new FileEntry("b.txt", "root/b.txt", false, null, null))
        {CheckState = false};
        using var subDir = new FileEntryViewModel(new FileEntry("sub", "root/sub", true, null, null))
        {CheckState = true};
        using var subFile = new FileEntryViewModel(new FileEntry("c.txt", "root/sub/c.txt", false, null, null))
        { CheckState = true  };
        subDir.Children = [subFile];

        using var parent = new FileEntryViewModel(root) { CheckState = true, Children = [file1, file2, subDir] };

        // Act
        var models = parent.GetCheckedModelRecursive(fileOnly: false);

        // Assert
        Assert.Contains( models, m => m.IsDirectory && m.Name == "root" );
        Assert.Contains( models, m => !m.IsDirectory && m.Name == "a.txt" );
        Assert.Contains( models, m => m.IsDirectory && m.Name == "sub" );
        Assert.Contains( models, m => !m.IsDirectory && m.Name == "c.txt" );
        Assert.DoesNotContain( models, m => m.Name == "b.txt" ); // Unchecked は含まれない
    }

    [Fact]
    public void GetCheckedModelRecursiveはディレクトリがCheckedかつfileOnlyがfalseのときにディレクトリを含む() {
        // Arrange
        var dir = new FileEntry("root", "root", true, null, null);
        using var parent = new FileEntryViewModel(dir) { CheckState = true };

        // Act
        var models = parent.GetCheckedModelRecursive(fileOnly: false);

        // Assert
        Assert.Contains( models, m => m.IsDirectory && m.Name == "root" );
    }

    [Fact]
    public void GetCheckedModelRecursiveはファイルがCheckedのときに自身を返す() {
        // Arrange
        using var file = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, null, null)) { CheckState = true };

        // Act
        var models = file.GetCheckedModelRecursive();

        // Assert
        Assert.Contains( models, m => m.Name == "a.txt" && !m.IsDirectory );
    }

    [Fact]
    public void GetCheckedModelRecursiveはディレクトリがCheckedかつfileOnlyがtrueのときにディレクトリを含まない() {
        // Arrange
        var dir = new FileEntry("root", "root", true, null, null);
        using var child = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, null, null)) { CheckState = true };
        using var parent = new FileEntryViewModel(dir) { CheckState = true, Children = [child] };

        // Act
        var models = parent.GetCheckedModelRecursive(fileOnly: true);

        // Assert
        Assert.DoesNotContain( models, m => m.IsDirectory );
        Assert.Contains( models, m => !m.IsDirectory && m.Name == "a.txt" );
    }

    #endregion

    #region Collection / Dispose

    [Fact]
    public void Childrenは追加や削除をしたときにChangeTypeとCheckStateをイベント発火で再計算する() {
        // Arrange
        var dir = new FileEntry("root", "root", true, null, null);
        using var parent = new FileEntryViewModel(dir);

        using var c1 = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, null, null))
        {CheckState = false};
        using var c2 = new FileEntryViewModel(new FileEntry("b.txt", "root/b.txt", false, "x", "y"))
        {CheckState = true};

        var changeTypeChangedCount = 0;
        ((INotifyPropertyChanged)parent).PropertyChanged += ( _, e ) => {
            if(e.PropertyName == nameof( FileEntryViewModel.ChangeType )) changeTypeChangedCount++;
        };

        var checkStateChangedCount = 0;
        parent.CheckStateChanged += ( _, __ ) => checkStateChangedCount++;

        // Act 1: 子1を追加（ChangeType 再評価発火。CheckState は既定=falseなら非発火の可能性）
        parent.Children.Add( c1 );
        // Act 2: 子2を追加（ChangeType 発火）。子が混在 → 親は null へ（CheckStateChanged 発火）
        parent.Children.Add( c2 );
        // Act 3: 子1を削除（ChangeType 発火）。親は true へ（CheckStateChanged 発火）
        parent.Children.Remove( c1 );

        // Assert
        // ChangeType はコレクション変更ごとに OnChildrenCollectionChanged で必ず RaisePropertyChanged される
        Assert.Equal( 3, changeTypeChangedCount );

        // CheckState は 2 回（混在になったとき、単一子になって Checked になったとき）
        Assert.Equal( 2, checkStateChangedCount );
    }

    [Fact]
    public void Disposeは呼び出した後に子イベントが親へ伝播しない() {
        // Arrange
        var dir = new FileEntry("root", "root", true, null, null);
        var parent = new FileEntryViewModel(dir);
        var child = new FileEntryViewModel(new FileEntry("a.txt", "root/a.txt", false, null, null)) { CheckState = false };
        parent.Children.Add( child );

        // Act
        parent.Dispose();
        child.CheckState = true;

        // Assert
        Assert.NotEqual( child.CheckState, parent.CheckState );
    }

    #endregion
}