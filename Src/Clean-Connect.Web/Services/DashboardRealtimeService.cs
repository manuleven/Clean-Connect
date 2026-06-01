using Clean_Connect.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Clean_Connect.Web.Services;

public sealed class DashboardRealtimeService(
    IHubContext<AdminDashboardHub> hubContext,
    IDashboardFeed dashboardFeed,
    ILogger<DashboardRealtimeService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
                await hubContext.Clients.Group(AdminDashboardHub.DashboardGroup)
                    .SendAsync("dashboard:update", dashboardFeed.NextSnapshot(), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Unable to publish the dashboard realtime update.");
            }
        }
    }
}
