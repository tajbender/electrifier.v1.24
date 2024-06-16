using CommunityToolkit.WinUI.Collections;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Vanara.Windows.Shell;
using Windows.UI.Notifications;

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
