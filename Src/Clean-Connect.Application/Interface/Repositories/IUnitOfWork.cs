using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Interface.Repositories
{
    public interface IUnitOfWork
    {
        IWorkerRepository Workers { get; } 

        IClientRepository Clients { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellation);
    }
}
