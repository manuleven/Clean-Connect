using Clean_Connect.Web.Models;

namespace Clean_Connect.Web.Services;

public sealed class DashboardFeed : IDashboardFeed
{
    private readonly string[] _serviceLabels = ["Home", "Deep", "Office", "Move-in"];
    private int _tick;

    public DashboardSnapshot NextSnapshot()
    {
        var tick = Interlocked.Increment(ref _tick);
        var revenue = 2_400_000 + (tick * 57_500) + Random.Shared.Next(18_000, 82_000);
        var activeBookings = 146 + (tick % 14) + Random.Shared.Next(0, 9);
        var supportSla = 91 + (tick % 8);
        var payout = 740_000 - (tick * 9_500) + Random.Shared.Next(5_000, 28_000);

        return new DashboardSnapshot(
            Metrics:
            [
                new("Revenue today", $"₦{revenue / 1_000_000m:0.00}m", "+24%", "mint", "chart"),
                new("Active bookings", activeBookings.ToString(), $"+{tick % 6 + 4}", "sky", "calendar"),
                new("Pending payouts", $"₦{Math.Max(payout, 380_000) / 1_000m:0}k", "-8%", "amber", "card"),
                new("Support SLA", $"{supportSla}%", "+4%", "rose", "bell")
            ],
            Revenue: BuildSeries(6, 250 + tick * 7, 38),
            Bookings: BuildSeries(6, 62 + tick * 2, 10),
            ServiceMix: BuildMix(tick),
            Activity: BuildActivity(tick),
            Notification: BuildNotification(tick));
    }

    private static IReadOnlyList<DashboardChartPoint> BuildSeries(int count, int baseline, int spread)
    {
        var labels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        return Enumerable.Range(0, count)
            .Select(index => new DashboardChartPoint(labels[index], baseline + Random.Shared.Next(-spread, spread + 1) + index * 11))
            .ToArray();
    }

    private IReadOnlyList<DashboardChartPoint> BuildMix(int tick) =>
        _serviceLabels
            .Select((label, index) => new DashboardChartPoint(label, 18 + ((tick + index * 9) % 34) + Random.Shared.Next(0, 8)))
            .ToArray();

    private static DashboardActivityUpdate BuildActivity(int tick)
    {
        var activities = new[]
        {
            new DashboardActivityUpdate("Cleaner dispatched", "Nearest verified worker accepted a same-day deep clean.", "just now", "mint", "route"),
            new DashboardActivityUpdate("Escrow released", "Completion confirmed and payout queued for review.", "just now", "sky", "card"),
            new DashboardActivityUpdate("Supervisor note", "Quality check added for a high-value office booking.", "just now", "amber", "office"),
            new DashboardActivityUpdate("Customer message", "Client requested hypoallergenic supplies before arrival.", "just now", "rose", "bell")
        };

        return activities[tick % activities.Length];
    }

    private static DashboardNotificationUpdate BuildNotification(int tick)
    {
        var notifications = new[]
        {
            new DashboardNotificationUpdate("New surge window", "Demand is rising around Ikoyi and Lekki.", "live", "sky"),
            new DashboardNotificationUpdate("Payout review", "Three payouts need admin approval.", "live", "amber"),
            new DashboardNotificationUpdate("Rating lift", "Cleaner quality score is trending upward.", "live", "mint"),
            new DashboardNotificationUpdate("Route optimization", "Six bookings can share a cleaner cluster.", "live", "rose")
        };

        return notifications[tick % notifications.Length];
    }
}
