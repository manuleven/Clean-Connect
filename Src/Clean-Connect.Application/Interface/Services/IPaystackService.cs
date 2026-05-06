using Clean_Connect.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Interface.Services
{
    public interface IPaystackService
    {
        Task <PaystackInitResponse> InitializePayment(decimal amount, string email, string reference);
    }
}
