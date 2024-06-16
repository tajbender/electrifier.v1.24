using Microsoft.UI.Xaml.Controls;

namespace electrifier.Controls.Vanara;

// TODO: For EnumerateChildren-Calls, add HWND handle
public sealed partial class Shell32TreeView : UserControl
{
    public object ItemsSource => NativeTreeView.ItemsSource;
    public TreeView NativeTreeView => TreeView;

    public Shell32TreeView()
    {
        InitializeComponent();
        DataContext = this;
    }
}
