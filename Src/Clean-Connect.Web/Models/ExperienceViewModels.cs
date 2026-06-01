namespace Clean_Connect.Web.Models;

public sealed record ServiceCardViewModel(
    string Name,
    string Description,
    string Price,
    string Duration,
    string Tone,
    string Icon,
    IReadOnlyList<string> Features);

public sealed record MetricCardViewModel(
    string Label,
    string Value,
    string Change,
    string Tone,
    string Icon);

public sealed record ActivityItemViewModel(
    string Title,
    string Description,
    string Time,
    string Tone,
    string Icon);

public sealed record NotificationViewModel(
    string Title,
    string Message,
    string Time,
    string Tone);

public sealed record BookingOptionViewModel(
    string Name,
    string Detail,
    decimal Price,
    string Duration,
    string Icon,
    bool Featured = false);

public sealed record BookingStepViewModel(
    string Label,
    string Detail,
    string State);

public sealed record PaymentMethodViewModel(
    string Name,
    string Detail,
    string Icon,
    bool Recommended = false);

public sealed record TrackStepViewModel(
    string Label,
    string Detail,
    string Time,
    string State);

public sealed record CleanerViewModel(
    string Name,
    string Specialty,
    string Rating,
    string CompletedJobs,
    string ArrivalWindow,
    string Initials);

public sealed record WorkerRowViewModel(
    string Name,
    string ServiceType,
    string Location,
    string Rating,
    string Status,
    string Initials);

public sealed record ClientRowViewModel(
    string Name,
    string Email,
    string Location,
    string ReferralCode,
    string Status,
    string Initials);

public sealed record ManagedServiceViewModel(
    string Name,
    string Description,
    decimal Amount,
    string ModifiedBy,
    string Status,
    string Icon);

public sealed record CouponCampaignViewModel(
    string Code,
    decimal DiscountPercentage,
    string Expiry,
    int UsageLimit,
    int UsedCount,
    bool IsActive);

public sealed record WalletLedgerItemViewModel(
    string Title,
    string Reference,
    string Amount,
    string Status,
    string Time,
    string Tone);

public sealed record HomePageViewModel(
    IReadOnlyList<ServiceCardViewModel> Services,
    IReadOnlyList<MetricCardViewModel> Metrics,
    IReadOnlyList<ActivityItemViewModel> RecentActivity);

public sealed record BookingPageViewModel(
    IReadOnlyList<BookingOptionViewModel> Options,
    IReadOnlyList<BookingStepViewModel> Steps,
    CleanerViewModel RecommendedCleaner);

public sealed record PaymentPageViewModel(
    IReadOnlyList<PaymentMethodViewModel> PaymentMethods,
    IReadOnlyList<BookingStepViewModel> EscrowSteps,
    decimal Subtotal,
    decimal Discount,
    decimal ServiceFee);

public sealed record TrackingPageViewModel(
    CleanerViewModel Cleaner,
    IReadOnlyList<TrackStepViewModel> Steps,
    IReadOnlyList<ActivityItemViewModel> Updates);

public sealed record AdminDashboardViewModel(
    IReadOnlyList<MetricCardViewModel> Metrics,
    IReadOnlyList<ActivityItemViewModel> Activities,
    IReadOnlyList<NotificationViewModel> Notifications);

public sealed record WorkerPortalViewModel(
    IReadOnlyList<WorkerRowViewModel> Workers,
    IReadOnlyList<BookingStepViewModel> VerificationSteps,
    IReadOnlyList<ActivityItemViewModel> BookingQueue);

public sealed record ClientPortalViewModel(
    IReadOnlyList<ClientRowViewModel> Clients,
    IReadOnlyList<ActivityItemViewModel> RecentRequests,
    IReadOnlyList<MetricCardViewModel> Metrics);

public sealed record ServicesPortalViewModel(
    IReadOnlyList<ManagedServiceViewModel> Services,
    IReadOnlyList<MetricCardViewModel> Metrics);

public sealed record CouponsPortalViewModel(
    IReadOnlyList<CouponCampaignViewModel> Coupons,
    IReadOnlyList<MetricCardViewModel> Metrics);

public sealed record WalletPortalViewModel(
    IReadOnlyList<MetricCardViewModel> Metrics,
    IReadOnlyList<WalletLedgerItemViewModel> Ledger,
    IReadOnlyList<BookingStepViewModel> PayoutSteps);
