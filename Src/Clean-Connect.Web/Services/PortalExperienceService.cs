using Clean_Connect.Web.Models;

namespace Clean_Connect.Web.Services;

public sealed class PortalExperienceService : IPortalExperienceService
{
    private static readonly PortalUserViewModel ClientUser = new(
        "Ada Williams",
        "ada.williams@example.com",
        PortalRole.Client,
        "AW",
        "Lekki Phase 1, Lagos");

    private static readonly PortalUserViewModel WorkerUser = new(
        "Amara Okafor",
        "amara.okafor@example.com",
        PortalRole.Worker,
        "AO",
        "Victoria Island, Lagos");

    private static readonly IReadOnlyList<PortalServiceViewModel> Services =
    [
        new(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Standard Home Clean",
            "Reliable weekly upkeep for apartments and family homes.",
            18000,
            "2-4 hrs",
            "/monoline/assets/img/bg/1.jpg",
            ["Kitchen and bathroom refresh", "Dusting and floor care", "Photo completion report"]),
        new(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            "Premium Deep Clean",
            "Detailed reset for move-ins, hosting, or seasonal cleaning.",
            42000,
            "4-8 hrs",
            "/monoline/assets/img/bg/2.jpg",
            ["Appliance detailing", "Hard-to-reach surfaces", "Sanitised touch points"]),
        new(
            Guid.Parse("33333333-3333-3333-3333-333333333333"),
            "Office Shine",
            "Scheduled commercial cleaning with clear attendance tracking.",
            75000,
            "Recurring",
            "/monoline/assets/img/bg/3.jpg",
            ["Workstations and shared areas", "Restroom supply checks", "After-hours options"])
    ];

    private static readonly IReadOnlyList<PortalWorkerViewModel> Workers =
    [
        new(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Amara Okafor", "Premium Deep Clean", "Victoria Island", 4.98, 428, "Available today", "/monoline/assets/img/team/1.jpg"),
        new(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Tunde Balogun", "Office Shine", "Ikoyi", 4.91, 312, "Next slot 3:00 PM", "/monoline/assets/img/team/2.jpg"),
        new(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Ifeoma Eze", "Standard Home Clean", "Yaba", 4.87, 276, "Available tomorrow", "/monoline/assets/img/team/3.jpg")
    ];

    private static readonly IReadOnlyList<PortalBookingViewModel> Bookings =
    [
        new(Guid.Parse("90000000-0000-0000-0000-000000000001"), "CC-48291", "Premium Deep Clean", "Ada Williams", "Amara Okafor", "Admiralty Way, Lekki Phase 1", DateTime.Today.AddDays(1).AddHours(10), "Morning", 42000, "AcceptedAwaitingPayment", "Pending", "Fund escrow"),
        new(Guid.Parse("90000000-0000-0000-0000-000000000002"), "CC-48212", "Office Shine", "Ridgeway Studios", "Tunde Balogun", "Akin Adesola Street, Victoria Island", DateTime.Today.AddDays(2).AddHours(14), "Afternoon", 75000, "InProgress", "MarkAsPaid", "Track progress"),
        new(Guid.Parse("90000000-0000-0000-0000-000000000003"), "CC-48044", "Standard Home Clean", "Ada Williams", "Ifeoma Eze", "Bourdillon Road, Ikoyi", DateTime.Today.AddDays(-2).AddHours(9), "Morning", 18000, "Completed", "Paid", "Leave review")
    ];

    private static readonly IReadOnlyList<PortalNotificationViewModel> Notifications =
    [
        new("Cleaner matched", "Amara accepted your deep clean request.", "2 min ago", "success", true),
        new("Escrow ready", "Payment can now be authorised for CC-48291.", "8 min ago", "warning", true),
        new("Checklist updated", "Kitchen cabinets and balcony glass were added.", "1 hr ago", "info", false)
    ];

    public PublicHomeViewModel GetPublicHome() => new(Services, Workers, Reviews());

    public ClientDashboardViewModel GetClientDashboard() => new(
        ClientUser,
        [
            new("Upcoming bookings", "2", "Next clean tomorrow", "fa-calendar-check-o", "green"),
            new("Escrow protected", "NGN 42k", "Awaiting payment", "fa-shield", "blue"),
            new("Referral rewards", "NGN 8k", "2 friends joined", "fa-gift", "orange")
        ],
        Bookings.Where(booking => booking.BookingStatus != "Completed").ToArray(),
        Services,
        Notifications);

    public ClientBookingFlowViewModel GetClientBookingFlow() => new(ClientUser, Services, Workers, Bookings);

    public ClientPaymentsViewModel GetClientPayments() => new(
        ClientUser,
        [
            new("PAY-48291", "CC-48291", 42000, "Pending", "Card", DateTime.Now.AddMinutes(-10)),
            new("PAY-48044", "CC-48044", 18000, "Paid", "Wallet", DateTime.Now.AddDays(-2))
        ],
        Bookings);

    public ClientSettingsViewModel GetClientSettings() => new(
        ClientUser,
        ["Admiralty Way, Lekki Phase 1", "Bourdillon Road, Ikoyi"],
        ["Email", "SMS", "In-app"]);

    public WorkerDashboardViewModel GetWorkerDashboard() => new(
        WorkerUser,
        [
            new("Today earnings", "NGN 42k", "1 job awaiting confirmation", "fa-money", "green"),
            new("Active jobs", "2", "One in progress", "fa-briefcase", "blue"),
            new("Rating", "4.98", "428 completed jobs", "fa-star", "orange")
        ],
        Bookings.Where(booking => booking.WorkerName == "Amara Okafor" || booking.BookingStatus is "Pending" or "InProgress").ToArray(),
        Notifications);

    public WorkerJobsViewModel GetWorkerJobs() => new(
        WorkerUser,
        Bookings.Where(booking => booking.BookingStatus != "Completed").ToArray(),
        Bookings.Where(booking => booking.BookingStatus == "Completed").ToArray());

    public WorkerWalletViewModel GetWorkerWallet() => new(
        WorkerUser,
        [
            new("Available balance", "NGN 86k", "Ready for payout", "fa-credit-card", "green"),
            new("Escrow held", "NGN 42k", "Awaiting client confirmation", "fa-lock", "blue"),
            new("Total earned", "NGN 1.8m", "This quarter", "fa-line-chart", "orange")
        ],
        [
            new("Escrow funded", "CC-48291", 42000, "Held", DateTime.Now.AddMinutes(-20)),
            new("Payout requested", "CC-48044", -18000, "Processing", DateTime.Now.AddDays(-1)),
            new("Payout released", "CC-47912", -35000, "Paid", DateTime.Now.AddDays(-3))
        ]);

    public MessagesViewModel GetClientMessages() => new(ClientUser, MessageThreads());

    public MessagesViewModel GetWorkerMessages() => new(WorkerUser, MessageThreads());

    private static IReadOnlyList<PortalMessageViewModel> MessageThreads() =>
    [
        new("Amara Okafor", "I reviewed the balcony glass note and will bring the right supplies.", "Now", "Open", "/monoline/assets/img/testimonial/1.jpg"),
        new("Clean Connect Support", "Your escrow is protected until completion is confirmed.", "12 min ago", "Unread", "/monoline/assets/img/testimonial/2.jpg"),
        new("Tunde Balogun", "Office access instructions received. I will arrive after closing.", "Yesterday", "Closed", "/monoline/assets/img/testimonial/3.jpg")
    ];

    private static IReadOnlyList<PortalReviewViewModel> Reviews() =>
    [
        new("Ada Williams", "Premium Deep Clean", 5, "The cleaner arrived on time, sent checklist photos, and left the apartment spotless.", "May 2026"),
        new("Ridgeway Studios", "Office Shine", 5, "Recurring cleaning finally feels organised. The after-hours flow is exactly what we needed.", "May 2026"),
        new("Mariam Bello", "Standard Home Clean", 4, "Fast booking, clear pricing, and a professional worker. I booked again for next week.", "April 2026")
    ];
}
