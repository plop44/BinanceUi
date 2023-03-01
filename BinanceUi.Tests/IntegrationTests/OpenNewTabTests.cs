using System.Diagnostics;
using System.Windows.Automation;

namespace BinanceUi.Tests.IntegrationTests;

internal class OpenNewTabTests
{
    private AutomationElement _binanceUiRoot;
    private Process _binanceUiProcess;

    [SetUp]
    public void BeforeEveryTest()
    {
        _binanceUiProcess = Process.Start("BinanceUi.exe");
        _binanceUiRoot = _binanceUiProcess.FindWindow();
    }

    [TearDown]
    public void AfterEveryTest()
    {
        _binanceUiProcess.CloseMainWindow();
        _binanceUiProcess.Dispose();
    }

    [Test]
    public void WhenWeOpenTradingPairInfo()
    {
        var tradingInfoButton = _binanceUiRoot.GetDescendantWithAutomationProperty("OpenTradingInfoButton");
        tradingInfoButton.Invoke();

        var newTab = _binanceUiRoot.GetDescendantWithAutomationProperty("TradingPairInfo.DataGrid");

        Assert.That(newTab, Is.Not.Null);
    }
}

public static class AutomationElementExtensions
{
    public static AutomationElement GetDescendantWithAutomationProperty(this AutomationElement element, string automationPropertyValue)
    {

        return element.Find(new PropertyCondition(AutomationElement.AutomationIdProperty, automationPropertyValue));
    }

    public static AutomationElement GetDescendantWithNameProperty(this AutomationElement element, string namePropertyValue)
    {
        return element.Find(new PropertyCondition(AutomationElement.NameProperty, namePropertyValue));
    }

    public static AutomationElement FindWindow(this Process process)
    {
        return AutomationElement.RootElement.Find(new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id), TreeScope.Children);
    }

    private static AutomationElement Find(this AutomationElement element, PropertyCondition propertyCondition, TreeScope treeScope = TreeScope.Descendants)
    {
        var stopwatch = Stopwatch.StartNew();
        do
        {
            var automationElement = element.FindFirst(treeScope, propertyCondition);

            if (automationElement != null)
                return automationElement;

        } while (stopwatch.ElapsedMilliseconds < 1500);

        throw new Exception("Could not find child");
    }

    public static void Invoke(this AutomationElement element)
    {
        InvokePattern invokePattern = element.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
        invokePattern?.Invoke();
    }
}