using BinanceUi.Various;

namespace BinanceUi;

public class AppResourceRegistrator
{
    private readonly App _app;
    private readonly SchedulerRepository _schedulerRepository;

    public AppResourceRegistrator(App app, SchedulerRepository schedulerRepository)
    {
        _app = app;
        _schedulerRepository = schedulerRepository;
    }

    public void Register()
    {
        _app.Resources.Add(SchedulerRepository.ResourceName, _schedulerRepository);
    }
}