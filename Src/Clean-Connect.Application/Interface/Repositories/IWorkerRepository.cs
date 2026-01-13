using Clean_Connect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Interface.Repositories
{
    public interface IWorkerRepository
    {
        Task CreateWorker(Worker worker, CancellationToken cancellationToken);

        Task<Worker> GetWorkerById(Guid workerId, CancellationToken cancellationToken);

        Task<IEnumerable<Worker>> GetAllWorkers(CancellationToken cancellationToken);

        Task UpdateWorker(Worker worker, CancellationToken cancellationToken);

        Task DeleteWorker(Worker worker, CancellationToken cancellationToken);

        Task<Worker> GetByEmail(string email, CancellationToken cancellationToken);

        Task<Worker> GetWorkerByName(string name, CancellationToken cancellationToken);
    }
}
