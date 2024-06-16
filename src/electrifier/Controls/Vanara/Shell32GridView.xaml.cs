using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.WinUI.Collections;
using Vanara.PInvoke;

namespace electrifier.Controls.Vanara;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public sealed partial class Shell32GridView : UserControl
{
    public GridView NativeGridView => GridView;

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
        var acv = new AdvancedCollectionView(items, true);
        acv.SortDescriptions.Add(new SortDescription("IsFolder", SortDirection.Descending));
        acv.SortDescriptions.Add(new SortDescription("DisplayName", SortDirection.Ascending));

        NativeGridView.ItemsSource = acv;
    }

    private string GetDebuggerDisplay()
    {
        return nameof(Shell32GridView) + ToString();
    }
}
