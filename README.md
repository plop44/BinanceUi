# UnofficialBinanceDesktopAppForWindows

Unofficial Binance desktop app for Windows.

This app demonstrates a few WPF concepts, including:

- Automated UI testing using Microsoft.Automation
- MVVM with ViewModel first approach (DataTemplate)
- WPF Behaviors
- use of ReactiveExtensions (Rx) in WPF

![Binance UI render](https://raw.githubusercontent.com/plop44/UnofficialBinanceDesktopAppForWindows/main/Ticker.gif)

## Automated UI Testing

The app includes automated UI tests using Microsoft.Automation. These tests ensure that the app functions as expected and that changes don't introduce bugs.

## MVVM with ViewModel First Approach

We use the MVVM design pattern with a ViewModel first approach, using DataTemplates to bind the View to the ViewModel.

## WPF Behaviors

We leverage WPF Behaviors to add interactivity to the UI without writing code-behind.

## Use of ReactiveExtensions in WPF

We use Rx to wire up the change mechanism in WPF, allowing us to respond to changes in the UI in a reactive way.

**Note:** The appearance of the UI is not the focus of this app, and we have not spent much time restyling the native WPF controls. In a production app, we would likely use a third-party control library with styling themes.
