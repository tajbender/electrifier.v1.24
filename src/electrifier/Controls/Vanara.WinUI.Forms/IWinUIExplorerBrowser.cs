using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using static Vanara.PInvoke.Shell32;

namespace electrifier.Controls.Vanara.WinUI.Forms;

/// <summary>
/// Interface for the (WinUI3 based) <see cref="ExplorerBrowser"/> control.
/// It is a clone of <a href="https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nn-shobjidl_core-iexplorerbrowser">Win32API - ExplorerBrowser</a>.
/// </summary>
[SupportedOSPlatform("windows")]
public interface IWinUIExplorerBrowser
{
    /// <summary>Flags specifying the folder to be browsed.</summary>
    [Flags]
    public enum ExplorerBrowserNavigationItemCategory : uint
    {
        /// <summary>An absolute PIDL, relative to the desktop.</summary>
        Absolute = SBSP.SBSP_ABSOLUTE,

        /// <summary>Windows Vista and later. Navigate without the default behavior of setting focus into the new view.</summary>
        ActivateNoFocus = SBSP.SBSP_ACTIVATE_NOFOCUS,

        /// <summary>Enable auto-navigation.</summary>
        AllowAutoNavigate = SBSP.SBSP_ALLOW_AUTONAVIGATE,

        /// <summary>
        /// Microsoft Internet Explorer 6 Service Pack 2 (SP2) and later. The navigation was possibly initiated by a web page with scripting
        /// code already present on the local system.
        /// </summary>
        CallerUntrusted = SBSP.SBSP_CALLERUNTRUSTED,

        /// <summary>
        /// Windows 7 and later. Do not add a new entry to the travel log. When the user enters a search term in the search box and
        /// subsequently refines the query, the browser navigates forward but does not add an additional travel log entry.
        /// </summary>
        CreateNoHistory = SBSP.SBSP_CREATENOHISTORY,

        /// <summary>
        /// Use default behavior, which respects the view option (the user setting to create new windows or to browse in place). In most
        /// cases, calling applications should use this flag.
        /// </summary>
        Default = SBSP.SBSP_DEFBROWSER,

        /// <summary>Use the current window.</summary>
        UseCurrentWindow = SBSP.SBSP_DEFMODE,

        /// <summary>
        /// Specifies a folder tree for the new browse window. If the current browser does not match the SBSP.SBSP_EXPLOREMODE of the browse
        /// object call, a new window is opened.
        /// </summary>
        ExploreMode = SBSP.SBSP_EXPLOREMODE,

        /// <summary>
        /// Windows Internet Explorer 7 and later. If allowed by current registry settings, give the browser a destination to navigate to.
        /// </summary>
        FeedNavigation = SBSP.SBSP_FEEDNAVIGATION,

        /// <summary>Windows Vista and later. Navigate without clearing the search entry field.</summary>
        KeepSearchText = SBSP.SBSP_KEEPWORDWHEELTEXT,

        /// <summary>Navigate back, ignore the PIDL.</summary>
        NavigateBack = SBSP.SBSP_NAVIGATEBACK,

        /// <summary>Navigate forward, ignore the PIDL.</summary>
        NavigateForward = SBSP.SBSP_NAVIGATEFORWARD,

        /// <summary>Creates another window for the specified folder.</summary>
        NewWindow = SBSP.SBSP_NEWBROWSER,

        /// <summary>Suppress selection in the history pane.</summary>
        NoHistorySelect = SBSP.SBSP_NOAUTOSELECT,

        /// <summary>Do not transfer the browsing history to the new window.</summary>
        NoTransferHistory = SBSP.SBSP_NOTRANSFERHIST,

        /// <summary>
        /// Specifies no folder tree for the new browse window. If the current browser does not match the SBSP.SBSP_OPENMODE of the browse
        /// object call, a new window is opened.
        /// </summary>
        NoFolderTree = SBSP.SBSP_OPENMODE,

        /// <summary>Browse the parent folder, ignore the PIDL.</summary>
        ParentFolder = SBSP.SBSP_PARENT,

        /// <summary>Windows 7 and later. Do not make the navigation complete sound for each keystroke in the search box.</summary>
        PlayNoSound = SBSP.SBSP_PLAYNOSOUND,

        /// <summary>Enables redirection to another URL.</summary>
        Redirect = SBSP.SBSP_REDIRECT,

        /// <summary>A relative PIDL, relative to the current folder.</summary>
        Relative = SBSP.SBSP_RELATIVE,

        /// <summary>Browse to another folder with the same Windows Explorer window.</summary>
        SameWindow = SBSP.SBSP_SAMEBROWSER,

        /// <summary>Microsoft Internet Explorer 6 Service Pack 2 (SP2) and later. The navigate should allow ActiveX prompts.</summary>
        TrustedForActiveX = SBSP.SBSP_TRUSTEDFORACTIVEX,

        /// <summary>
        /// Microsoft Internet Explorer 6 Service Pack 2 (SP2) and later. The new window is the result of a user initiated action. Trust the
        /// new window if it immediately attempts to download content.
        /// </summary>
        TrustFirstDownload = SBSP.SBSP_TRUSTFIRSTDOWNLOAD,

        /// <summary>
        /// Microsoft Internet Explorer 6 Service Pack 2 (SP2) and later. The window is navigating to an untrusted, non-HTML file. If the
        /// user attempts to download the file, do not allow the download.
        /// </summary>
        UntrustedForDownload = SBSP.SBSP_UNTRUSTEDFORDOWNLOAD,

        /// <summary>Write no history of this navigation in the history Shell folder.</summary>
        WriteNoHistory = SBSP.SBSP_WRITENOHISTORY
    }
    /// <summary>The navigation log is a history of the locations visited by the <see cref="ExplorerBrowser"/>.
    /// TODO: There's an single class in Vanara that holds an NavigationLog
    /// </summary>
    public class NavigationLog
    {
        /// <summary>A navigation traversal request.</summary>
        private class PendingNavigation
        {
            internal int Index
            {
                get; set;
            }

            internal ShellItem Location
            {
                get; set;
            }

            internal PendingNavigation(ShellItem location, int index)
            {
                Location = location;
                Index = index;
            }
        }

        private readonly ExplorerBrowser parent;

        // <summary>The pending navigation log action. null if the user is not navigating via the navigation log.</summary>
        private PendingNavigation? pendingNavigation;

        /// <summary>Indicates the presence of locations in the log that can be reached by calling Navigate(Backward).</summary>
        public bool CanNavigateBackward => CurrentLocationIndex > 0;

        // <summary>Indicates the presence of locations in the log that can be reached by calling Navigate(Forward).</summary>
        public bool CanNavigateForward => CurrentLocationIndex < Locations.Count - 1;

        // <summary>Gets the shell object in the Locations collection pointed to by CurrentLocationIndex.</summary>
        public ShellItem? CurrentLocation
        {
            get
            {
                if (CurrentLocationIndex >= 0)
                {
                    return Locations[CurrentLocationIndex];
                }

                return null;
            }
        }

        // <summary>Gets or sets the current location index, an index into the Locations collection.
        // The ShellItem pointed to by this index is the current location of the ExplorerBrowser.
        // <value>The current location index.</value>
        // <remarks>Setting this property will cause the <see cref="NavigationLogChanged"/> event to fire.</remarks>
        // </summary>
        public int CurrentLocationIndex { get; set; } = -1;



        // <summary>The navigation log</summary>
        public List<ShellItem> Locations { get; } = new List<ShellItem>();


        //
        // Summary:
        //     Fires when the navigation log changes or the current navigation position changes
        public event EventHandler<NavigationLogEventArgs>? NavigationLogChanged;

        internal NavigationLog(IWinUIExplorerBrowser parent)
        {
            this.parent = (ExplorerBrowser)(parent ?? throw new ArgumentNullException(nameof(parent)));
            //this.parent.Navigated += OnNavigated;
            //this.parent.NavigationFailed += OnNavigationFailed;
        }

        //
        // Summary:
        //     Clears the contents of the navigation log.
        public void Clear()
        {
            if (Locations.Count != 0)
            {
                var canNavigateBackward = CanNavigateBackward;
                var canNavigateForward = CanNavigateForward;
                Locations.Clear();
                CurrentLocationIndex = -1;
                var e = new NavigationLogEventArgs
                {
                    LocationsChanged = true,
                    CanNavigateBackwardChanged = (canNavigateBackward != CanNavigateBackward),
                    CanNavigateForwardChanged = (canNavigateForward != CanNavigateForward)
                };
                NavigationLogChanged?.Invoke(this, e);
            }
        }

        internal bool NavigateLog(NavigationLogDirection direction)
        {
            int index;
            if (direction != 0)
            {
                if (direction != NavigationLogDirection.Backward || !CanNavigateBackward)
                {
                    goto IL_002f;
                }

                index = CurrentLocationIndex - 1;
            }
            else
            {
                if (!CanNavigateForward)
                {
                    goto IL_002f;
                }

                index = CurrentLocationIndex + 1;
            }

            var shellItem = Locations[index];
            pendingNavigation = new PendingNavigation(shellItem, index);
            parent.Navigate(shellItem);
            return true;
        IL_002f:
            return false;
        }

        internal bool NavigateLog(int index)
        {
            if (index >= Locations.Count || index < 0)
            {
                return false;
            }

            if (index == CurrentLocationIndex)
            {
                return false;
            }

            var shellItem = Locations[index];
            pendingNavigation = new PendingNavigation(shellItem, index);
            // TODO:            parent?.Navigate(shellItem);
            return true;
        }

        private void OnNavigated(object? sender, NavigatedEventArgs args)
        {
            var navigationLogEventArgs = new NavigationLogEventArgs();
            var canNavigateBackward = CanNavigateBackward;
            var canNavigateForward = CanNavigateForward;
            if (pendingNavigation != null)
            {
                if (pendingNavigation.Location.IShellItem.Compare(args.NewLocation?.IShellItem, Shell32.SICHINTF.SICHINT_ALLFIELDS) != 0)
                {
                    if (CurrentLocationIndex < Locations.Count - 1)
                    {
                        Locations.RemoveRange(CurrentLocationIndex + 1, Locations.Count - (CurrentLocationIndex + 1));
                    }

                    if (args.NewLocation != null)
                    {
                        Locations.Add(args.NewLocation);
                    }

                    CurrentLocationIndex = Locations.Count - 1;
                    navigationLogEventArgs.LocationsChanged = true;
                }
                else
                {
                    CurrentLocationIndex = pendingNavigation.Index;
                    navigationLogEventArgs.LocationsChanged = false;
                }

                pendingNavigation = null;
            }
            else
            {
                if (CurrentLocationIndex < Locations.Count - 1)
                {
                    Locations.RemoveRange(CurrentLocationIndex + 1, Locations.Count - (CurrentLocationIndex + 1));
                }

                if (args.NewLocation != null)
                {
                    Locations.Add(args.NewLocation);
                }

                CurrentLocationIndex = Locations.Count - 1;
                navigationLogEventArgs.LocationsChanged = true;
            }

            navigationLogEventArgs.CanNavigateBackwardChanged = canNavigateBackward != CanNavigateBackward;
            navigationLogEventArgs.CanNavigateForwardChanged = canNavigateForward != CanNavigateForward;
            NavigationLogChanged?.Invoke(this, navigationLogEventArgs);
        }

        private void OnNavigationFailed(object? sender, /* Vanara.WinUI.Forms.ExplorerBrowser. */ NavigationFailedEventArgs args)
        {
            pendingNavigation = null;
        }
    }

    /// <summary>The event argument for NavigationLogChangedEvent</summary>
    public class NavigationLogEventArgs : EventArgs
    {
        /// <summary>Indicates CanNavigateBackward has changed</summary>
        public bool CanNavigateBackwardChanged
        {
            get; set;
        }

        /// <summary>Indicates CanNavigateForward has changed</summary>
        public bool CanNavigateForwardChanged
        {
            get; set;
        }

        /// <summary>Indicates the Locations collection has changed</summary>
        public bool LocationsChanged
        {
            get; set;
        }
    }

    /// <summary>Event argument for The Navigated event</summary>
    public class NavigatedEventArgs : EventArgs
    {
        /// <summary>The new location of the explorer browser</summary>
        public ShellItem? NewLocation
        {
            get; set;
        }
    }

    /// <summary>Event argument for The Navigating event</summary>
    public class NavigatingEventArgs : EventArgs
    {
        /// <summary>Set to 'True' to cancel the navigation.</summary>
        public bool Cancel
        {
            get; set;
        }

        /// <summary>The location being navigated to</summary>
        public ShellItem? PendingLocation
        {
            get; set;
        }
    }

    /// <summary>Event argument for the NavigationFailed event</summary>
    public class NavigationFailedEventArgs : EventArgs
    {
        /// <summary>The location the browser would have navigated to.</summary>
        public ShellItem? FailedLocation
        {
            get; set;
        }
    }

    /// <summary>Gets the items in the ExplorerBrowser as an IShellItemArray</summary>
    /// <returns>An <see cref="IShellItemArray"/> instance or <see langword="null"/> if not available.</returns>
    public abstract ShellItemArray? GetItemsArray(SVGIO opt);

    /// <summary>Represents a collection of <see cref="ShellItem"/> attached to an <see cref="ExplorerBrowser"/>.</summary>
    public class ShellItemCollection : IReadOnlyList<ShellItem>
    {
        private readonly IWinUIExplorerBrowser eb;
        private readonly SVGIO option;
        private readonly ShellItemArray items;

        internal ShellItemCollection(IWinUIExplorerBrowser eb, SVGIO opt)
        {
            this.eb = eb;
            option = opt;

            items = eb.GetItemsArray(option);
        }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <value>Returns a <see cref="int"/> value.</value>
        public int Count => items.Count;

        private /*ShellItemCollection */ ShellItemArray Items => items;
        //var array = eb.GetItemsArray(option);
        //try
        //{
        //    return array is null ? Enumerable.Empty<IShellItem>() : array;
        //}

        /// <summary>Gets the <see cref="ShellItem"/> at the specified index.</summary>
        /// <value>The <see cref="ShellItem"/>.</value>
        /// <param name="index">The zero-based index of the element to get.</param>
        public ShellItem this[int index] => items[index];

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ShellItem> GetEnumerator()
            //=> Items.Select(ShellItem.Open).GetEnumerator();
            => items.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}


// TreeViewNode
public class ShellTreeViewItem
{
    public ShellTreeViewItem(ShellItem shellItem)
    {
        ShellItem = shellItem ?? throw new ArgumentNullException(nameof(shellItem));
        Children = new List<ShellTreeViewItem>();
    }

    public ShellItem ShellItem
    {
        get;
    }

    public string Name => ShellItem.GetDisplayName(ShellItemDisplayString.NormalDisplay);

    //public ShellTreeViewItem Parent
    //{
    //    get;
    //}

    public List<ShellTreeViewItem> Children
    {
        get; private set;
    }

    internal void EnumerateChildren()
    {
        if (Children.Count == 0)
        {
            if (ShellItem is ShellFolder folder)
            {
                var children = folder.EnumerateChildren(FolderItemFilter.Folders);

                foreach (var child in children)
                {
                    Children.Add(new ShellTreeViewItem(child));
                }
            }
        }
    }
}

public class ShellTreeViewItemCollection : IReadOnlyList<ShellTreeViewItem>
{
    private readonly IWinUIExplorerBrowser eb;
    private readonly SVGIO option;
    //private readonly ShellItemArray items;
    private readonly List<ShellTreeViewItem> items;

    internal ShellTreeViewItemCollection(IWinUIExplorerBrowser eb, SVGIO opt)
    {
        this.eb = eb;
        option = opt;

        var newItems = eb.GetItemsArray(option);
        items = newItems.Select(i => new ShellTreeViewItem(i)).ToList();
    }

    public int Count => items.Count;

    private /*ShellItemCollection */ List<ShellTreeViewItem> Items => items;
    public ShellTreeViewItem this[int index] => items[index];

    public IEnumerator<ShellTreeViewItem> GetEnumerator()
        //=> Items.Select(ShellItem.Open).GetEnumerator();
        => items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
