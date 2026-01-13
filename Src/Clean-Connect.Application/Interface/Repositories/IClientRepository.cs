using Clean_Connect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Interface.Repositories
{
    public interface IClientRepository
    {
        Task CreateClient(Client client, CancellationToken cancellationToken);

        Task<Client> GetClientById(Guid clientId, CancellationToken cancellationToken);

        Task<IEnumerable<Client>> GetAllClients(CancellationToken cancellationToken);

        Task UpdateClient(Client client, CancellationToken cancellationToken);

        Task DeleteClient(Client client, CancellationToken cancellationToken);

        Task<Client> GetByEmail(string email, CancellationToken cancellationToken);

        Task<Client> GetClientByName(string name, CancellationToken cancellationToken);
    }
}
