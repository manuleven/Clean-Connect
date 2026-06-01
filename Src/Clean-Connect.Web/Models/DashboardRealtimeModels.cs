namespace Clean_Connect.Web.Models;

public sealed record DashboardMetricUpdate(
    string Label,
    string Value,
    string Change,
    string Tone,
    string Icon);

public sealed record DashboardChartPoint(
    string Label,
    decimal Value);

public sealed record DashboardActivityUpdate(
    string Title,
    string Description,
    string Time,
    string Tone,
    string Icon);

public sealed record DashboardNotificationUpdate(
    string Title,
    string Message,
    string Time,
    string Tone);

public sealed record DashboardSnapshot(
    IReadOnlyList<DashboardMetricUpdate> Metrics,
    IReadOnlyList<DashboardChartPoint> Revenue,
    IReadOnlyList<DashboardChartPoint> Bookings,
    IReadOnlyList<DashboardChartPoint> ServiceMix,
    DashboardActivityUpdate Activity,
    DashboardNotificationUpdate Notification);
