/*
    Copyright 2024 Thorsten Jung, aka tajbender
            https://www.electrifier.org

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI;
using electrifier.Services;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Storage.Search;
using Windows.Storage;

namespace electrifier.ViewModels;

public partial class FileManagerViewModel : ObservableRecipient
{
    // AdvancedCollectionView can be bound to anything that uses collections.
    public AdvancedCollectionView GridAdvancedCollectionView
    {
        get;
    } = new AdvancedCollectionView(/* TreeViewItemsCollection */);
    public ObservableCollection<DosShellItem> GridViewItemsCollection { get; } = new ObservableCollection<DosShellItem>();
    // AdvancedCollectionView can be bound to anything that uses collections.
    public AdvancedCollectionView TreeAdvancedCollectionView
    {
        get;
    } = new AdvancedCollectionView(/* TreeViewItemsCollection */);
    public ObservableCollection<DosShellItem> TreeViewItemsCollection { get; } = new ObservableCollection<DosShellItem>();
    /// <summary>
    /// Count of Files
    /// </summary>
    public int FileCount
    {
        get;
    }
    /// <summary>
    /// Count of Folders
    /// </summary>
    public int FolderCount
    {
        get;
    }
    /// <summary>
    /// Count of Items
    /// </summary>
    public int ItemCount
    {
        get;
    }

    /// <summary>
    /// FileManagerViewModel
    /// </summary>
    public FileManagerViewModel()
    {
        // Set up the AdvancedCollectionView with live shaping enabled to filter and sort the original list
        TreeAdvancedCollectionView = new AdvancedCollectionView(TreeViewItemsCollection, true);
        GridAdvancedCollectionView = new AdvancedCollectionView(GridViewItemsCollection, true);
        // And sort ascending by the property "Name"
        TreeAdvancedCollectionView.SortDescriptions.Add(new SortDescription("Name", SortDirection.Ascending));
        GridAdvancedCollectionView.SortDescriptions.Add(new SortDescription("Name", SortDirection.Ascending));
        // TODO: TreeView -> OnSelection: _ = ShellGridViewItems_GetItemsAsync(KnownLibraryId.Documents);






        //        GridViewItemsCollection.CollectionChanged += (s, e) => ShellGridViewItems_ContainerContentChanging(this, new C{ });


    }



    
}
