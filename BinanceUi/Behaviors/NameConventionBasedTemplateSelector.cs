using System;
using System.Windows;
using System.Windows.Controls;

namespace BinanceUi.Behaviors;

public class NameConventionBasedTemplateSelector : DataTemplateSelector
{
    public string? Suffix { get; set; }
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (string.IsNullOrEmpty(Suffix))
            throw new Exception("Property 'Suffix' should be set.");

        var tabHeader = item.GetType().Name + Suffix;
        return (DataTemplate)Application.Current.Resources[tabHeader] 
               ?? throw new Exception($"Cannot find a DataTemplate resource with key '{tabHeader}', please add it.");
    }
}