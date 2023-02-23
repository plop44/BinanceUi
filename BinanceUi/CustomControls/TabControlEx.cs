using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

#nullable disable

namespace BinanceUi.CustomControls;

// source https://stackoverflow.com/questions/9794151/stop-tabcontrol-from-recreating-its-children/9802346#9802346
// we are using this control to have our tab persisted, and not recreated at every SelectedItem change.
// we need that for the search to work as expected, and probably more stuff in the future.
[TemplatePart(Name = "PART_ItemsHolder", Type = typeof(Panel))]
public class TabControlEx : TabControl
{
    private Panel _itemsHolderPanel;

    public TabControlEx()
    {
        // This is necessary so that we get the initial data bound selected item
        ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
    }

    /// <summary>
    ///     If containers are done, generate the selected item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
    {
        if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
        {
            ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
            UpdateSelectedItem();
        }
    }

    /// <summary>
    ///     Get the ItemsHolder and generate any children
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _itemsHolderPanel = GetTemplateChild("PART_ItemsHolder") as Panel;
        UpdateSelectedItem();
    }

    /// <summary>
    ///     When the items change we remove any generated panel children and add any new ones as necessary
    /// </summary>
    /// <param name="e"></param>
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(e);

        if (_itemsHolderPanel == null)
            return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                _itemsHolderPanel.Children.Clear();
                break;

            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems != null)
                    foreach (var item in e.OldItems)
                    {
                        var cp = FindChildContentPresenter(item);
                        if (cp != null)
                            _itemsHolderPanel.Children.Remove(cp);
                    }

                // Don't do anything with new items because we don't want to
                // create visuals that aren't being shown

                UpdateSelectedItem();
                break;

            case NotifyCollectionChangedAction.Replace:
                throw new NotImplementedException("Replace not implemented yet");
        }
    }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);
        UpdateSelectedItem();
    }

    private void UpdateSelectedItem()
    {
        if (_itemsHolderPanel == null)
            return;

        // Generate a ContentPresenter if necessary
        var item = GetSelectedTabItem();
        if (item != null)
            CreateChildContentPresenter(item);

        // show the right child
        foreach (ContentPresenter child in _itemsHolderPanel.Children)
            child.Visibility = (child.Tag as TabItem).IsSelected ? Visibility.Visible : Visibility.Collapsed;
    }

    private ContentPresenter CreateChildContentPresenter(object item)
    {
        if (item == null)
            return null;

        var cp = FindChildContentPresenter(item);

        if (cp != null)
            return cp;

        // the actual child to be added.  cp.Tag is a reference to the TabItem
        cp = new ContentPresenter();
        cp.Content = item is TabItem ? (item as TabItem).Content : item;
        cp.ContentTemplate = SelectedContentTemplate;
        cp.ContentTemplateSelector = SelectedContentTemplateSelector;
        cp.ContentStringFormat = SelectedContentStringFormat;
        cp.Visibility = Visibility.Collapsed;
        cp.Tag = item is TabItem ? item : ItemContainerGenerator.ContainerFromItem(item);
        _itemsHolderPanel.Children.Add(cp);
        return cp;
    }

    private ContentPresenter FindChildContentPresenter(object data)
    {
        if (data is TabItem item)
            data = item.Content;

        if (data == null)
            return null;

        if (_itemsHolderPanel == null)
            return null;

        foreach (ContentPresenter cp in _itemsHolderPanel.Children)
            if (cp.Content == data)
                return cp;

        return null;
    }

    protected TabItem GetSelectedTabItem()
    {
        var selectedItem = SelectedItem;
        if (selectedItem == null)
            return null;

        return selectedItem as TabItem ?? ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as TabItem;
    }
}