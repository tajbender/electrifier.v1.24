using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using static Vanara.PInvoke.Shell32.PIDL;

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
            IconExtractor = new ShellIconExtractor(value, IconSize);
            IconExtractor.Start();
        }
    }

    public ShellIconExtractor IconExtractor;
    public int IconSize = 32;

    public ObservableCollection<ExplorerBrowserItem2> Items;
    private Shell32GridView GridView => ShellGridView;

    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;
        _currentFolder = ShellFolder.Desktop;
        Items = [];

        Loading += ExplorerBrowser_Loading;
        Loaded += ExplorerBrowser_Loaded;

        IconExtractor = new ShellIconExtractor(CurrentFolder, bmpSize: IconSize);
        IconExtractor.IconExtracted += (sender, args) =>
        {
            //lock(_items)
            //{
                Items.Add(new ExplorerBrowserItem2(IconExtractor, args.ImageListIndex, args.ItemID));
            //}
        };
        IconExtractor.Complete += IconExtractor_Complete;
        IconExtractor.Start();
    }

    private void ExplorerBrowser_Loading(FrameworkElement sender, object args)
    {
        Debug.Print($"{nameof(ExplorerBrowser)} is Loading, current items {Items.Count}");
    }

    private void ExplorerBrowser_Loaded(object sender, RoutedEventArgs e)
    {
        Debug.Print($"{nameof(ExplorerBrowser)} has been Loaded, current items {Items.Count}");

        //GridView.SetItemsSource(_items);
    }

    private void IconExtractor_Complete(object? sender, EventArgs e)
    {
        Debug.Print($"{nameof(IconExtractor)} {TaskStatus.RanToCompletion.ToString()}: {Items.Count} items" );

        //GridView.SetItemsSource(_items);
    }

    private void NativeTreeView_SelectionChanged(TreeView _, TreeViewSelectionChangedEventArgs args)
    {
        if (args.AddedItems.Count > 0)
        {
            Debug.Print($"{nameof(NativeTreeView_SelectionChanged)}: {args}");
        }
    }

    private void NativeGridView_ItemClick(object _, ItemClickEventArgs e)
    {
        if (e.ClickedItem is ExplorerBrowserItem2 ebItem)
        {
            Debug.Print($"{nameof(NativeGridView_ItemClick)}: {ebItem}");
        }
    }
}

public record ExplorerBrowserItem2
{
    public readonly Shell32.PIDL ItemId = Null;
    public readonly string DisplayName;
    public readonly BitmapImage? BitmapImage;

    public ExplorerBrowserItem2(ShellIconExtractor iconExtractor, int imageListIndex, Shell32.PIDL itemId)
    {
        Debug.Assert(itemId != Null);
        if (itemId == Null)
        {
            Debug.Print($"{nameof(ItemId)} is null"); // TODO: throw new ArgumentNullException(nameof(itemId));
        }

        ItemId = itemId; //new Shell32.PIDL(itemId);
        DisplayName = ItemId.ToString(Shell32.SIGDN.SIGDN_NORMALDISPLAY);
        BitmapImage = null;

        //if (imageListIndex < 0)
        //{
        //    BitmapImage = null;  // Todo: Load default image
        //}

        //BitmapImage = new BitmapImage();
        //using (MemoryStream stream = new MemoryStream())
        //{
        //    iconExtractor.ImageList[imageListIndex].Save(stream, ImageFormat.Bmp);
        //    stream.Position = 0;
        //    BitmapImage.SetSource(stream.AsRandomAccessStream());
        //}
        //image.Source = bitmapImage;
        

        // INFO: https://stackoverflow.com/questions/76640972/convert-system-drawing-icon-to-microsoft-ui-xaml-imagesource

//        SoftwareBitmap softwareBitmap = ...; // Your HBITMAP
//// Create a SoftwareBitmapSource
//        SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
//        await bitmapSource.SetBitmapAsync(softwareBitmap);
//// Set the ImageSource for your WinUI3 Image control
//        qrCodeImage.Source = bitmapSource;
    }

    public override string? ToString() => base.ToString() + ItemId;
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
