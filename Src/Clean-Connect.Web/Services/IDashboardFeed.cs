using Clean_Connect.Web.Models;

namespace Clean_Connect.Web.Services;

public interface IDashboardFeed
{
    DashboardSnapshot NextSnapshot();
}
