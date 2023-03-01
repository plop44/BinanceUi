using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;

#nullable disable

namespace BinanceUi.CustomControls;

// source https://stackoverflow.com/questions/9794151/stop-tabcontrol-from-recreating-its-children/9802346#9802346
// we are using this control to have our tab persisted, and not recreated at every SelectedItem change.
// we need that for the search to work as expected, and probably more stuff in the future.
[TemplatePart(Name = "PART_ItemsHolder", Type = typeof(Panel))]
public class TabControlEx : TabControl
{
    public Panel ItemsHolderPanel { get; private set; }

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
        ItemsHolderPanel = GetTemplateChild("PART_ItemsHolder") as Panel;
        UpdateSelectedItem();
    }

    /// <summary>
    ///     When the items change we remove any generated panel children and add any new ones as necessary
    /// </summary>
    /// <param name="e"></param>
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(e);

        if (ItemsHolderPanel == null)
            return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                ItemsHolderPanel.Children.Clear();
                break;

            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems != null)
                    foreach (var item in e.OldItems)
                    {
                        var cp = FindChildContentPresenter(item);
                        if (cp != null)
                            ItemsHolderPanel.Children.Remove(cp);
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
        if (ItemsHolderPanel == null)
            return;

        // Generate a ContentPresenter if necessary
        var item = GetSelectedTabItem();
        if (item != null)
            CreateChildContentPresenter(item);

        // show the right child
        foreach (ContentPresenter child in ItemsHolderPanel.Children)
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
        cp = new ContentPresenter
        {
            Content = item is TabItem ? (item as TabItem).Content : item,
            ContentTemplate = SelectedContentTemplate,
            ContentTemplateSelector = SelectedContentTemplateSelector,
            ContentStringFormat = SelectedContentStringFormat,
            Visibility = Visibility.Collapsed,
            Tag = item is TabItem ? item : ItemContainerGenerator.ContainerFromItem(item),
        };
        ItemsHolderPanel.Children.Add(cp);
        return cp;
    }

    private ContentPresenter FindChildContentPresenter(object data)
    {
        if (data is TabItem item)
            data = item.Content;

        if (data == null)
            return null;

        if (ItemsHolderPanel == null)
            return null;

        foreach (ContentPresenter cp in ItemsHolderPanel.Children)
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
    
    // following code has been added by me
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new CustomTabControlAutomationPeer(this);
    }
}

public class CustomTabItemAutomationPeer : TabItemAutomationPeer
{
    public CustomTabItemAutomationPeer(object owner, CustomTabControlAutomationPeer tabControlAutomationPeer) 
        : base(owner, tabControlAutomationPeer)
    {
    }
    
    protected override List<AutomationPeer> GetChildrenCore()
    {
        var headerChildren = base.GetChildrenCore();

        if (ItemsControlAutomationPeer.Owner is TabControlEx parentTabControl)
        {
            var contentHost = parentTabControl.ItemsHolderPanel;
            if (contentHost != null)
            {
                AutomationPeer contentHostPeer = new FrameworkElementAutomationPeer(contentHost);
                var contentChildren = contentHostPeer.GetChildren();
                if (contentChildren != null)
                {
                    if (headerChildren == null)
                        headerChildren = contentChildren;
                    else
                        headerChildren.AddRange(contentChildren);
                }
            }
        }


        return headerChildren;
    }
}
public class CustomTabControlAutomationPeer : TabControlAutomationPeer
{
    public CustomTabControlAutomationPeer(TabControlEx owner) : base(owner)
    {
    }
    protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
    {
        return new CustomTabItemAutomationPeer(item, this);
    }
}