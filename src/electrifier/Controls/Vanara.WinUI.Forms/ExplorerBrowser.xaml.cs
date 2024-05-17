using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using System;
using Vanara.Collections;
using Vanara.Extensions;
using Vanara.InteropServices;
using Vanara.PInvoke;
using Vanara.Windows.Forms.Design;
using Vanara.Windows.Shell;

using static electrifier.Controls.Vanara.WinUI.Forms.IWinUIExplorerBrowser;
using static Vanara.PInvoke.Shell32;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236
// and more about our project templates, see: http://aka.ms/winui-project-info.

//public static readonly DependencyProperty SelectedPathProperty =
//    DependencyProperty.Register("SelectedPath", typeof(string), typeof(ExplorerBrowser), new PropertyMetadata(null, OnSelectedPathChanged));
//private static void OnSelectedPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
//    => throw new NotImplementedException();
//public string SelectedPath;

namespace electrifier.Controls.Vanara.WinUI.Forms;

/// <summary>
/// ExplorerBrowser is a WinUI UserControl that displays a Windows Explorer-like view of the file system.
/// </summary>
public partial class ExplorerBrowser : UserControl, IWinUIExplorerBrowser
{
    private IWinUIExplorerBrowser.NavigationLog History
    {
        get;
    }
    private ShellItemCollection Items
    {
        get; 
        //set; // TODO: Remove set?
    }

    private List<ShellTreeViewItem> m_TreeViewItems = new();

    private readonly ShellFolder m_CurrentFolder;

    public ExplorerBrowser() : base()
    {
        InitializeComponent();

        m_CurrentFolder = ShellFolder.Desktop;

        History = new IWinUIExplorerBrowser.NavigationLog(this);
        Items = new ShellItemCollection(this, SVGIO.SVGIO_ALLVIEW);

        m_TreeViewItems = new List<ShellTreeViewItem>();
        var root = new ShellTreeViewItem(m_CurrentFolder);
        m_TreeViewItems.Add(root);
        root.EnumerateChildren();
    }

    /// <summary>Let ExplorerBrowser navigate to the specified folder.</summary>
    /// <param name="shellItem">Folder to navigate to</param>
    /// <remarks>TODO: if shellItem is a file, trigger event to open the file with the caller's default application</remarks>
    public void Navigate(ShellItem shellItem, ExplorerBrowserNavigationItemCategory category = ExplorerBrowserNavigationItemCategory.Default)
    {
        if (shellItem == null)
        {
            //Items.Clear();
            return;
        }

        //        Items = GetItemsArray(SVGIO.SVGIO_ALLVIEW);


        ///// Vanara code
        // if (explorerBrowserControl is null)
        // {
        //     antecreationNavigationTarget = (shellItem, category);
        // }
        // else if (shellItem is not null)
        // {
        //     var hr = explorerBrowserControl.BrowseToObject(shellItem.IShellItem, (SBSP)category);
        //     if (hr == HRESULT_RESOURCE_IN_USE || hr == HRESULT_CANCELLED)
        //         OnNavigationFailed(new NavigationFailedEventArgs { FailedLocation = shellItem });
        //     else if (hr.Failed)
        //         throw new ArgumentException("Unable to browse to this shell item.", nameof(shellItem), hr.GetException());
        // }
    }

    public ShellItemArray? GetItemsArray(SVGIO opt)
    {
        // 	public ShellItemArray(IEnumerable<ShellItem> shellItems) : this(shellItems.Select(i => (IntPtr)i.PIDL).ToArray())
        List<ShellItem> items = new();

        var list = m_CurrentFolder.EnumerateChildren(FolderItemFilter.Folders);

        // FolderItemFilter filter /*= FolderItemFilter.Folders | FolderItemFilter.IncludeHidden | FolderItemFilter.NonFolders | FolderItemFilter.IncludeSuperHidden */

        foreach (var item in list)
        {
            items.Add(item);
        }

        // see TreeViewItem.HasUnrealizedChildren:
        // https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.treeviewitem?view=windows-app-sdk-1.5

        var result = new ShellItemArray(items);

        return result;
    }
}
