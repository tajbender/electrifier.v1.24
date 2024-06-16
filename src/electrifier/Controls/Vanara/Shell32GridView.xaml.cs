using System.Diagnostics;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml.Controls;

namespace electrifier.Controls.Vanara;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public sealed partial class Shell32GridView : UserControl
{
    public GridView NativeGridView => GridView;
    public object ItemsSource => NativeGridView.ItemsSource;

    public Shell32GridView()
    {
        InitializeComponent();
        DataContext = this;

        NativeGridView.IsItemClickEnabled = true;
        NativeGridView.SelectionMode = ListViewSelectionMode.Single;

        // INFO: What is this? NativeGridView.IsSynchronizedWithCurrentItem = true;
    }

    public void SetItemsSource(List<ExplorerBrowserItem2> items)
    {
        NativeGridView.ItemsSource = items;
    }

    private string GetDebuggerDisplay()
    {
        return nameof(Shell32GridView) + ToString();
    }
}
