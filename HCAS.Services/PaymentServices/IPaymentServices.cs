using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCAS.DTO;

namespace HCAS.Services.PaymentServices
{
    public interface IPaymentServices
    {
        Task<List<PaymentDto>> GetAll();
        Task<PaymentDto> GetById(int id);
        Task<bool> AddPayment(PaymentDto paymentDto);
        Task<bool> UpdatePayment(PaymentDto paymentDto);
        Task<bool> DeletePayment(int id);
        Task<List<PaymentDto>> GetPaymentDetailsData();
    }
}
