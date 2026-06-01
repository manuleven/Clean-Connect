using Clean_Connect.Web.Services;
using Microsoft.AspNetCore.SignalR;

namespace Clean_Connect.Web.Hubs;

public sealed class AdminDashboardHub(IDashboardFeed dashboardFeed) : Hub
{
    public const string DashboardGroup = "admin-dashboard";

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, DashboardGroup);
        await Clients.Caller.SendAsync("dashboard:update", dashboardFeed.NextSnapshot());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, DashboardGroup);
        await base.OnDisconnectedAsync(exception);
    }
}
