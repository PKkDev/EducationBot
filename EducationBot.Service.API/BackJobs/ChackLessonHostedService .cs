namespace EducationBot.Service.API.BackJobs;

public class CheckLessonHostedService : IHostedService, IDisposable
{
    private readonly ILogger<CheckLessonHostedService> _logger;
    private Timer? _timer = null;

    private readonly IServiceProvider _services;

    public CheckLessonHostedService(
        ILogger<CheckLessonHostedService> logger,
        IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        _logger.LogInformation($"{nameof(CheckLessonHostedService)} start");

        _timer = new Timer(async (a) => await DoWork(a), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
    }

    public async Task StopAsync(CancellationToken ct)
    {
        _logger.LogInformation($"{nameof(CheckLessonHostedService)} stop");

        _timer?.Change(Timeout.Infinite, 0);
    }

    private async Task DoWork(object? state)
    {
        try
        {
            _logger.LogInformation($"{nameof(CheckLessonHostedService)} processing");

            using var scope = _services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<CheckLessonWorker>();

            await service.DoWork(default);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(CheckLessonHostedService)} error");
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

}
