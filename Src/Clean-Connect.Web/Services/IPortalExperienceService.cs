using Clean_Connect.Web.Models;

namespace Clean_Connect.Web.Services;

public interface IPortalExperienceService
{
    PublicHomeViewModel GetPublicHome();
    ClientDashboardViewModel GetClientDashboard();
    ClientBookingFlowViewModel GetClientBookingFlow();
    ClientPaymentsViewModel GetClientPayments();
    ClientSettingsViewModel GetClientSettings();
    WorkerDashboardViewModel GetWorkerDashboard();
    WorkerJobsViewModel GetWorkerJobs();
    WorkerWalletViewModel GetWorkerWallet();
    MessagesViewModel GetClientMessages();
    MessagesViewModel GetWorkerMessages();
}
