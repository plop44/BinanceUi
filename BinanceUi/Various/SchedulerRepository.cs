using System.Reactive.Concurrency;

namespace BinanceUi.Various;

public class SchedulerRepository
{
    public const string ResourceName = nameof(SchedulerRepository);

    public SchedulerRepository(ImmediateOrDispatcherScheduler immediateOrDispatcherScheduler, SynchronizationContextScheduler synchronizationContextScheduler)
    {
        ImmediateOrDispatcherScheduler = immediateOrDispatcherScheduler;
        SynchronizationContextScheduler = synchronizationContextScheduler;
    }

    public ImmediateOrDispatcherScheduler ImmediateOrDispatcherScheduler { get; }
    public SynchronizationContextScheduler SynchronizationContextScheduler { get; }
}