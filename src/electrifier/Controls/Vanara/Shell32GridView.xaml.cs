using System.Diagnostics;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

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

    #region Experimental code

    private static readonly Uri _defaultImageUri =
        new("ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File_256x256-32.png");
    private void ImageIcon_Loaded(object sender, RoutedEventArgs e)
    {
        var img = sender as ImageIcon;

        if (img == null)
        {
            return;
        }

        var bitmapImage = new BitmapImage
        {
            //img.Width = bitmapImage.DecodePixelWidth = 280;
            UriSource = _defaultImageUri
        };
        img.Source = bitmapImage;
    }

    #endregion
}
