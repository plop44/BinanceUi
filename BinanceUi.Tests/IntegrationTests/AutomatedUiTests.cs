using System.Diagnostics;
using System.Windows.Automation;

namespace BinanceUi.Tests.IntegrationTests;

/// <summary>
/// Automated UI test using Microsoft Automation.
/// A (not so) good tool to see the Automation Element tree is https://accessibilityinsights.io/downloads/
/// </summary>
internal class AutomatedUiTests
{
    private Process _binanceUiProcess;
    private AutomationElement _binanceUiRoot;

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
        var tradingInfoDataGrid = OpenTradingInfoTab()
            .GetTradingInfoDataGrid();

        Assert.That(tradingInfoDataGrid, Is.Not.Null);
    }

    [Test(Description = "We are testing scenario where we open a Symbol ticker from the SymbolInfo screen. It involves, opening Symbol Info, then right clicking on the first row to open menu item, then clicking it.")]
    public void WhenWeOpenTicker()
    {
        var tradingInfoDataGridFirstItem = OpenTradingInfoTab()
            .GetTradingInfoDataGrid()
            .IntroduceDelayForLoading()
            .GetFirstItem();

        tradingInfoDataGridFirstItem.OpenContextMenu();

        // get menu item
        var priceTickerMenuItem = _binanceUiRoot.GetDescendantWithAutomationProperty("TradingPairInfo.GoToPriceTicker");
        priceTickerMenuItem.Invoke();

        //  ASSERT
        var newTab = _binanceUiRoot.GetDescendantWithAutomationProperty("Ticker.Symbol");
        Assert.That(newTab, Is.Not.Null);
    }

    private AutomationElement GetTradingInfoDataGrid()
    {
        return _binanceUiRoot.GetDescendantWithAutomationProperty("TradingPairInfo.DataGrid");
    }

    private AutomatedUiTests OpenTradingInfoTab()
    {
        var tradingInfoButton = _binanceUiRoot.GetDescendantWithAutomationProperty("OpenTradingInfoButton");
        tradingInfoButton.Invoke();

        return this;
    }
}