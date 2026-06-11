namespace Clean_Connect.Web.Models;

public enum PortalRole
{
    Guest,
    Client,
    Worker
}

public sealed record PortalUserViewModel(
    string Name,
    string Email,
    PortalRole Role,
    string Initials,
    string Location);

public sealed record PortalMetricViewModel(
    string Label,
    string Value,
    string Detail,
    string Icon,
    string Tone);

public sealed record PortalServiceViewModel(
    Guid Id,
    string Name,
    string Summary,
    decimal Amount,
    string Duration,
    string Image,
    IReadOnlyList<string> Includes);

public sealed record PortalWorkerViewModel(
    Guid Id,
    string Name,
    string Service,
    string Location,
    double Rating,
    int CompletedJobs,
    string Availability,
    string Image);

public sealed record PortalBookingViewModel(
    Guid Id,
    string Reference,
    string Service,
    string ClientName,
    string WorkerName,
    string Address,
    DateTime DateOfService,
    string TimeRange,
    decimal Amount,
    string BookingStatus,
    string PaymentStatus,
    string NextAction);

public sealed record PortalNotificationViewModel(
    string Title,
    string Message,
    string Time,
    string Tone,
    bool IsUnread);

public sealed record PortalMessageViewModel(
    string Sender,
    string Preview,
    string Time,
    string ThreadStatus,
    string Image);

public sealed record PortalPaymentViewModel(
    string Reference,
    string BookingReference,
    decimal Amount,
    string Status,
    string Method,
    DateTime CreatedAt);

public sealed record PortalWalletItemViewModel(
    string Title,
    string Reference,
    decimal Amount,
    string Status,
    DateTime CreatedAt);

public sealed record PortalReviewViewModel(
    string Author,
    string Subject,
    int Rating,
    string Comment,
    string Date);

public sealed record PublicHomeViewModel(
    IReadOnlyList<PortalServiceViewModel> Services,
    IReadOnlyList<PortalWorkerViewModel> FeaturedWorkers,
    IReadOnlyList<PortalReviewViewModel> Reviews);

public sealed record ClientDashboardViewModel(
    PortalUserViewModel User,
    IReadOnlyList<PortalMetricViewModel> Metrics,
    IReadOnlyList<PortalBookingViewModel> UpcomingBookings,
    IReadOnlyList<PortalServiceViewModel> Services,
    IReadOnlyList<PortalNotificationViewModel> Notifications);

public sealed record ClientBookingFlowViewModel(
    PortalUserViewModel User,
    IReadOnlyList<PortalServiceViewModel> Services,
    IReadOnlyList<PortalWorkerViewModel> RecommendedWorkers,
    IReadOnlyList<PortalBookingViewModel> RecentBookings);

public sealed record ClientPaymentsViewModel(
    PortalUserViewModel User,
    IReadOnlyList<PortalPaymentViewModel> Payments,
    IReadOnlyList<PortalBookingViewModel> Bookings);

public sealed record ClientSettingsViewModel(
    PortalUserViewModel User,
    IReadOnlyList<string> Addresses,
    IReadOnlyList<string> NotificationChannels);

public sealed record WorkerDashboardViewModel(
    PortalUserViewModel User,
    IReadOnlyList<PortalMetricViewModel> Metrics,
    IReadOnlyList<PortalBookingViewModel> JobRequests,
    IReadOnlyList<PortalNotificationViewModel> Notifications);

public sealed record WorkerJobsViewModel(
    PortalUserViewModel User,
    IReadOnlyList<PortalBookingViewModel> ActiveJobs,
    IReadOnlyList<PortalBookingViewModel> CompletedJobs);

public sealed record WorkerWalletViewModel(
    PortalUserViewModel User,
    IReadOnlyList<PortalMetricViewModel> Metrics,
    IReadOnlyList<PortalWalletItemViewModel> Ledger);

public sealed record MessagesViewModel(
    PortalUserViewModel User,
    IReadOnlyList<PortalMessageViewModel> Threads);
