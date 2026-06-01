using Clean_Connect.Web.Models;

namespace Clean_Connect.Web.Services;

public sealed class ExperienceDataService : IExperienceDataService
{
    public HomePageViewModel GetHomePage() => new(
        Services:
        [
            new("Home Reset", "Recurring upkeep for apartments and family homes with checklist-grade quality.", "from ₦18k", "2-4 hrs", "mint", "sparkle",
            [
                "Kitchen and bath detailing",
                "Dusting, floors, and surfaces",
                "Quality photo report"
            ]),
            new("Deep Clean", "Premium top-to-bottom refresh before hosting, moving in, or seasonal resets.", "from ₦42k", "4-8 hrs", "sky", "brush",
            [
                "Appliances and hard-to-reach zones",
                "Sanitised touch points",
                "Two-person crew option"
            ]),
            new("Office Shine", "Quiet, scheduled commercial cleaning with attendance tracking and supervisor notes.", "custom", "daily", "amber", "office",
            [
                "Workstations and shared areas",
                "Restroom consumables check",
                "After-hours availability"
            ])
        ],
        Metrics:
        [
            new("Bookings completed", "12,480", "+18%", "mint", "check"),
            new("Avg. response", "8 min", "-32%", "sky", "route"),
            new("Cleaner rating", "4.9/5", "+0.3", "amber", "sparkle")
        ],
        RecentActivity:
        [
            new("Cleaner matched", "A verified specialist accepted a Lekki Phase 1 booking.", "2 min ago", "mint", "user"),
            new("Escrow funded", "Payment secured for a deep-clean appointment.", "7 min ago", "sky", "card"),
            new("Quality check", "Supervisor review completed with photo proof.", "12 min ago", "amber", "shield")
        ]);

    public BookingPageViewModel GetBookingPage() => new(
        Options:
        [
            new("Standard Home Clean", "Best for weekly or bi-weekly upkeep", 18000, "2-4 hrs", "home"),
            new("Premium Deep Clean", "Detailed reset with appliances and corners", 42000, "4-8 hrs", "sparkle", Featured: true),
            new("Move-in Reset", "Empty-space clean before handover", 56000, "6-9 hrs", "brush"),
            new("Office Cleaning", "Teams, rosters, and recurring schedules", 75000, "custom", "office")
        ],
        Steps:
        [
            new("Service", "Choose a cleaning package", "complete"),
            new("Schedule", "Pick date, time, and address", "active"),
            new("Match", "Assign verified cleaner", "pending"),
            new("Pay", "Fund escrow securely", "pending")
        ],
        RecommendedCleaner: new("Amara Okafor", "Deep-clean specialist", "4.98", "428", "Today, 2:30 PM", "AO"));

    public PaymentPageViewModel GetPaymentPage() => new(
        PaymentMethods:
        [
            new("Card", "Visa, Mastercard, Verve", "card", Recommended: true),
            new("Bank Transfer", "Instant Paystack transfer", "shield"),
            new("Wallet", "Use Clean Connect balance", "sparkle")
        ],
        EscrowSteps:
        [
            new("Authorise", "Customer funds the booking", "complete"),
            new("Hold", "Clean Connect protects both sides", "active"),
            new("Release", "Cleaner is paid after completion", "pending")
        ],
        Subtotal: 42000,
        Discount: 4500,
        ServiceFee: 2500);

    public TrackingPageViewModel GetTrackingPage() => new(
        Cleaner: new("Amara Okafor", "Deep-clean specialist", "4.98", "428", "12-18 min", "AO"),
        Steps:
        [
            new("Booked", "Your appointment is confirmed.", "9:10 AM", "complete"),
            new("Cleaner assigned", "Amara accepted and reviewed your checklist.", "9:18 AM", "complete"),
            new("On the way", "Cleaner is headed to your address.", "Now", "active"),
            new("Job in progress", "Live quality checklist starts on arrival.", "Next", "pending"),
            new("Review and release", "Confirm completion to release escrow.", "Final", "pending")
        ],
        Updates:
        [
            new("ETA adjusted", "Traffic-aware arrival window updated to 12-18 minutes.", "Now", "sky", "route"),
            new("Checklist synced", "Kitchen, bathrooms, windows, and floors added to the job board.", "5 min ago", "mint", "check"),
            new("Supplies verified", "Cleaner confirmed eco-safe supplies and protective covers.", "11 min ago", "amber", "shield")
        ]);

    public AdminDashboardViewModel GetAdminDashboard() => new(
        Metrics:
        [
            new("Revenue today", "₦2.84m", "+24%", "mint", "chart"),
            new("Active bookings", "186", "+16", "sky", "calendar"),
            new("Pending payouts", "₦640k", "-8%", "amber", "card"),
            new("Support SLA", "96%", "+4%", "rose", "bell")
        ],
        Activities:
        [
            new("Priority booking", "Corporate deep-clean request needs a supervisor assignment.", "1 min ago", "amber", "office"),
            new("Cleaner verified", "Two new workers passed document and rating checks.", "4 min ago", "mint", "shield"),
            new("Payment captured", "Escrow funded for booking CC-48291.", "9 min ago", "sky", "card")
        ],
        Notifications:
        [
            new("Supply alert", "Eco disinfectant stock is below reorder threshold.", "Now", "amber"),
            new("Rating trend", "Post-clean ratings improved 7% this week.", "12 min ago", "mint"),
            new("Route cluster", "17 bookings can be grouped around Victoria Island.", "21 min ago", "sky")
        ]);

    public WorkerPortalViewModel GetWorkerPortal() => new(
        Workers:
        [
            new("Amara Okafor", "Deep Clean", "Lekki Phase 1", "4.98", "Available", "AO"),
            new("Tunde Balogun", "Office Shine", "Victoria Island", "4.91", "On job", "TB"),
            new("Ifeoma Eze", "Home Reset", "Ikoyi", "4.87", "Available", "IE"),
            new("Musa Danjuma", "Move-in Reset", "Yaba", "4.82", "Review", "MD")
        ],
        VerificationSteps:
        [
            new("Identity", "Government ID and selfie match", "complete"),
            new("Skill check", "Service category and tools reviewed", "complete"),
            new("Bank details", "Payout account awaiting confirmation", "active"),
            new("Go live", "Cleaner becomes matchable", "pending")
        ],
        BookingQueue:
        [
            new("Accept request", "Deep clean booking CC-48291 is within 1.8km.", "Now", "mint", "route"),
            new("Job in progress", "Office cleaning checklist started at Civic Towers.", "8 min ago", "sky", "office"),
            new("Completion pending", "Client needs to confirm and release escrow.", "14 min ago", "amber", "shield")
        ]);

    public ClientPortalViewModel GetClientPortal() => new(
        Clients:
        [
            new("Ada Williams", "ada@example.com", "Lekki", "ADA-9K2", "Active", "AW"),
            new("Chinedu Nwosu", "chinedu@example.com", "Ikoyi", "CHI-14Q", "VIP", "CN"),
            new("Mariam Bello", "mariam@example.com", "Yaba", "MAR-77X", "New", "MB"),
            new("Kelechi Obi", "kelechi@example.com", "Ajah", "KEL-3PA", "Active", "KO")
        ],
        RecentRequests:
        [
            new("Booking completed", "Ada confirmed completion and rated Amara 5 stars.", "3 min ago", "mint", "check"),
            new("Referral redeemed", "Mariam used a referral coupon on first booking.", "17 min ago", "sky", "sparkle"),
            new("Support note", "Chinedu requested recurring office billing.", "31 min ago", "amber", "bell")
        ],
        Metrics:
        [
            new("Active clients", "3,420", "+12%", "mint", "user"),
            new("Referral revenue", "₦480k", "+28%", "sky", "sparkle"),
            new("Repeat rate", "72%", "+9%", "amber", "chart")
        ]);

    public ServicesPortalViewModel GetServicesPortal() => new(
        Services:
        [
            new("Home Reset", "Recurring home upkeep with structured room checklist.", 18000, "ops@cleanconnect", "Published", "home"),
            new("Premium Deep Clean", "Detailed top-to-bottom reset for high-effort spaces.", 42000, "admin@cleanconnect", "Published", "sparkle"),
            new("Move-in Reset", "Empty-apartment and handover-ready cleaning.", 56000, "ops@cleanconnect", "Draft", "brush"),
            new("Office Shine", "Commercial cleaning with after-hours scheduling.", 75000, "admin@cleanconnect", "Published", "office")
        ],
        Metrics:
        [
            new("Published services", "12", "+3", "mint", "check"),
            new("Avg. order value", "₦41k", "+16%", "sky", "chart"),
            new("Draft updates", "4", "today", "amber", "brush")
        ]);

    public CouponsPortalViewModel GetCouponsPortal() => new(
        Coupons:
        [
            new("CLEAN10", 10, "Jun 30, 2026", 500, 184, true),
            new("FIRST25", 25, "Jul 12, 2026", 250, 92, true),
            new("OFFICE15", 15, "Aug 02, 2026", 120, 41, true),
            new("SPRING5", 5, "May 31, 2026", 400, 400, false)
        ],
        Metrics:
        [
            new("Active campaigns", "3", "+1", "mint", "sparkle"),
            new("Discount used", "₦310k", "+18%", "sky", "card"),
            new("Validation rate", "94%", "+6%", "amber", "check")
        ]);

    public WalletPortalViewModel GetWalletPortal() => new(
        Metrics:
        [
            new("Worker balance", "₦1.24m", "+14%", "mint", "card"),
            new("Total earned", "₦9.82m", "+31%", "sky", "chart"),
            new("Escrow held", "₦3.10m", "+22%", "amber", "shield")
        ],
        Ledger:
        [
            new("Escrow funded", "CC-48291", "+₦42,000", "Held", "2 min ago", "amber"),
            new("Payout requested", "CC-48180", "-₦36,500", "Review", "18 min ago", "sky"),
            new("Payout released", "CC-48021", "-₦24,000", "Paid", "1 hr ago", "mint"),
            new("Escrow refunded", "CC-47992", "-₦18,000", "Resolved", "3 hrs ago", "rose")
        ],
        PayoutSteps:
        [
            new("Complete job", "Worker marks service as completed", "complete"),
            new("Client confirms", "Customer verifies quality", "complete"),
            new("Request payout", "Worker submits bank details", "active"),
            new("Transfer", "Admin/Paystack releases funds", "pending")
        ]);
}
