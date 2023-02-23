using System.Reactive.Concurrency;
using System.Threading;
using System.Windows;
using BinanceUi.Various;

namespace BinanceUi;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // this call should absolutely be in main thread to get the right synchronization context.
        var synchronizationContextScheduler = new SynchronizationContextScheduler(SynchronizationContext.Current!);

        var dependencyInjectionContainer = new DependencyInjectionContainer(this, synchronizationContextScheduler);

        // should be called first
        dependencyInjectionContainer.GetResourceRegistrator().Register();
        var mainWindow = dependencyInjectionContainer.GetMainWindow();
        var mainWindowViewModel = dependencyInjectionContainer.GetViewModel();

        mainWindow.DataContext = mainWindowViewModel;
        MainWindow = mainWindow;
        mainWindow.Show();
    }
}