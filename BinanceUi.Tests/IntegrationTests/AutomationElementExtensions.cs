using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Automation;

namespace BinanceUi.Tests.IntegrationTests;

public static class AutomationElementExtensions
{
    public static AutomationElement GetDescendantWithAutomationProperty(this AutomationElement element, string automationPropertyValue, [CallerArgumentExpression(nameof(automationPropertyValue))] string expression = "")
    {
        return element.Find(new PropertyCondition(AutomationElement.AutomationIdProperty, automationPropertyValue), expression);
    }

    public static AutomationElement FindWindow(this Process process)
    {
        return AutomationElement.RootElement.Find(new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id), "Window", TreeScope.Children);
    }

    public static AutomationElement Find(this AutomationElement element, PropertyCondition propertyCondition, string childName, TreeScope treeScope = TreeScope.Descendants)
    {
        var stopwatch = Stopwatch.StartNew();
        do
        {
            var automationElement = element.FindFirst(treeScope, propertyCondition);

            if (automationElement != null)
                return automationElement;

            Thread.Sleep(100);
        } while (stopwatch.ElapsedMilliseconds < 3000);

        throw new Exception($"Could not find child {childName}");
    }

    public static void Invoke(this AutomationElement element)
    {
        var invokePattern = element.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
        invokePattern?.Invoke();
    }

    public static AutomationElement IntroduceDelayForLoading(this AutomationElement element)
    {
        Thread.Sleep(5000);
        return element;
    }

    public static AutomationElement GetFirstItem(this AutomationElement element)
    {
        var itemContainerPattern = element.GetCurrentPattern(ItemContainerPatternIdentifiers.Pattern) as ItemContainerPattern;

        if (itemContainerPattern == null) throw new Exception($"Expecting ItemContainerPattern at this point. Check {nameof(element.GetSupportedPatterns)}");

        var findItemByProperty = itemContainerPattern.FindItemByProperty(null, AutomationProperty.LookupById(0), null);

        if (findItemByProperty == null) throw new Exception("No items found");
        return findItemByProperty;
    }

    public static void OpenContextMenu(this AutomationElement automationElement)
    {
        var clickablePoint = automationElement.GetClickablePoint();
        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown, clickablePoint);
        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp, clickablePoint);
    }
}