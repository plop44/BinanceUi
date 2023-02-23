using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BinanceUi.Various;
using Microsoft.Xaml.Behaviors;

namespace BinanceUi.Behaviors;

public class CloseItemOnMiddleClickBehavior : Behavior<ItemsControl>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseDown += OnPreviewMouseDown;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewMouseDown -= OnPreviewMouseDown;
    }

    private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed)
        {
            var itemToRemove = ((DependencyObject)e.OriginalSource).FindVisualAncestor<FrameworkElement>();

            if (itemToRemove != null) AssociatedObject.RemoveFromItemsSource(itemToRemove.DataContext);
        }
    }
}