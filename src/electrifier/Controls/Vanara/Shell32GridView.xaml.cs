using System.Collections.ObjectModel;
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
    //public object ItemsSource => NativeGridView.ItemsSource;

    public ObservableCollection<ExplorerBrowserItem2> GridShellItems
    {
        get => (ObservableCollection<ExplorerBrowserItem2>)GetValue(GridShellItemsProperty);
        set => SetValue(GridShellItemsProperty, value);
    }

    public static readonly DependencyProperty GridShellItemsProperty = DependencyProperty.Register(nameof(GridShellItems), typeof(List<ExplorerBrowserItem2>), typeof(Shell32GridView), new PropertyMetadata(default(List<ExplorerBrowserItem2>)));

    public Shell32GridView()
    {
        InitializeComponent();
        DataContext = this;

        NativeGridView.IsItemClickEnabled = true;
        NativeGridView.SelectionMode = ListViewSelectionMode.Single;
        NativeGridView.ItemsSource = GridShellItems;

        // INFO: What is this? NativeGridView.IsSynchronizedWithCurrentItem = true;
    }

    private string GetDebuggerDisplay()
    {
        return $"{nameof(Shell32GridView)} + \", \" + {GridShellItems.Count} items: + {ToString()}";
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
