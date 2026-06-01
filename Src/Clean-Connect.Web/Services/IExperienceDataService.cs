using Clean_Connect.Web.Models;

namespace Clean_Connect.Web.Services;

public interface IExperienceDataService
{
    HomePageViewModel GetHomePage();
    BookingPageViewModel GetBookingPage();
    PaymentPageViewModel GetPaymentPage();
    TrackingPageViewModel GetTrackingPage();
    AdminDashboardViewModel GetAdminDashboard();
    WorkerPortalViewModel GetWorkerPortal();
    ClientPortalViewModel GetClientPortal();
    ServicesPortalViewModel GetServicesPortal();
    CouponsPortalViewModel GetCouponsPortal();
    WalletPortalViewModel GetWalletPortal();
}
