using Vanara.Windows.Shell;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Drawing;
using Microsoft.UI.Xaml.Controls;
using Vanara.PInvoke;

namespace electrifier.Controls.Vanara;


// TODO: See also https://github.com/dahall/Vanara/blob/ac0a1ac301dd4fdea9706688dedf96d596a4908a/Windows.Shell.Common/StockIcon.cs

public sealed partial class ExplorerBrowser : UserControl
{
    private readonly ShellFolder _currentFolder;
    public ShellFolder CurrentFolder
    {
        get => _currentFolder;
        set
        {
            var ext = new ShellIconExtractor(value, IconSize);
            ext.Start();
        }
    }

    public ShellIconExtractor IconExtractor;
    public int IconSize = 32;

    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;
        _currentFolder = ShellFolder.Desktop;
        IconExtractor = new ShellIconExtractor(CurrentFolder, bmpSize: IconSize);
        IconExtractor.IconExtracted += (sender, args) =>
        {
            var newItem = new ExplorerBrowserItem(args.ImageListIndex, args.ItemID);
        };

        this.Loading += ExplorerBrowser_Loading;
        this.Loaded += ExplorerBrowser_Loaded;
    }

    private void ExplorerBrowser_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        _ = IconExtractor.ImageList;
    }

    private void ExplorerBrowser_Loading(Microsoft.UI.Xaml.FrameworkElement sender, object args)
    {
        _= IconExtractor.ImageList;
    }

    public record ExplorerBrowserItem
    {
        public readonly int ImageListIndex = -1;
        public readonly Shell32.PIDL ItemId;

        public ExplorerBrowserItem(int imageListIndex, Shell32.PIDL itemId)
        {
            ImageListIndex = imageListIndex;
            ItemId = itemId;
        }
    }

    private void NativeTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        if (args.AddedItems.Count > 0)
        {
            Debug.Print($"{nameof(NativeTreeView_SelectionChanged)}: {args.ToString()}");
        }
    }

    private void NativeGridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is ExplorerBrowserItem ebItem)
        {
            Debug.Print($"{nameof(NativeGridView_ItemClick)}: {ebItem.ToString()}");
        }
    }


    #region The following is original copy & paste from Vanara
    /// <summary>Event argument for The Navigated event</summary>
    public class NavigatedEventArgs : EventArgs
    {
        /// <summary>The new location of the explorer browser</summary>
        public ShellItem? NewLocation { get; set; }
    }

    /// <summary>Event argument for The Navigating event</summary>
    public class NavigatingEventArgs : EventArgs
    {
        /// <summary>Set to 'True' to cancel the navigation.</summary>
        public bool Cancel { get; set; }

        /// <summary>The location being navigated to</summary>
        public ShellItem? PendingLocation { get; set; }
    }

    /// <summary>Event argument for the NavigatinoFailed event</summary>
    public class NavigationFailedEventArgs : EventArgs
    {
        /// <summary>The location the browser would have navigated to.</summary>
        public ShellItem? FailedLocation { get; set; }
    }
    #endregion

}
