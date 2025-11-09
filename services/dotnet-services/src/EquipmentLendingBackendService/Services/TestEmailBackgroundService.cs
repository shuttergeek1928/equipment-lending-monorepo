public class TestEmailBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TestEmailBackgroundService> _logger;

    public TestEmailBackgroundService(IServiceScopeFactory scopeFactory, ILogger<TestEmailBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Example loop — adapt to your timer/schedule
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<INotificationSender>();

                await sender.SendEmailAsync(
                    "testuser@example.com",
                    "MailHog Test",
                    $"Email sent at {DateTime.Now:HH:mm:ss}"
                );
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { /* shutting down */ }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending emails.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // or whatever schedule
        }
    }
}


