﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCAS.DL;
using HCAS.DTO;
using HCAS.Services.Mapperly;
using Microsoft.EntityFrameworkCore;

namespace HCAS.Services.PaymentServices
{
    public class PaymentServices : IPaymentServices
    {
        private readonly HCASDbContext _dbContext;

        public PaymentServices(HCASDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PaymentDto>> GetAll()
        {
            try
            {
                List<Payment> payments = await _dbContext.Payments.ToListAsync();
                if (payments == null || payments.Count == 0)
                    return [];

                return Mapper.Map(payments);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching all payments.", ex);
            }
        }

        public async Task<PaymentDto> GetById(int id)
        {
            try
            {
                Payment payment = await _dbContext.Payments.FindAsync(id);
                return payment == null ? throw new KeyNotFoundException($"Payment with ID {id} not found.") : Mapper.Map(payment);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching payment by ID {id}.", ex);
            }
        }

        public async Task<bool> AddPayment(PaymentDto paymentDto)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(paymentDto);

                Payment payment = Mapper.Map(paymentDto);
                await _dbContext.Payments.AddAsync(payment);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error adding payment.", ex);
            }
        }

        public async Task<bool> UpdatePayment(PaymentDto paymentDto)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(paymentDto);

                Payment payment = await _dbContext.Payments.FindAsync(paymentDto.ID)
                    ?? throw new KeyNotFoundException($"Payment with ID {paymentDto.ID} not found.");

                payment.AppointmentID = paymentDto.AppointmentID;
                payment.Amount = paymentDto.Amount;
                payment.PaymentDate = (DateTime)paymentDto.PaymentDate;
                payment.PaymentMethod = paymentDto.PaymentMethod;

                _dbContext.Payments.Update(payment);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating payment with ID {paymentDto.ID}.", ex);
            }
        }

        public async Task<bool> DeletePayment(int id)
        {
            try
            {
                Payment payment = await _dbContext.Payments.FindAsync(id)
                    ?? throw new KeyNotFoundException($"Payment with ID {id} not found.");

                _dbContext.Payments.Remove(payment);
                int affectedRows = await _dbContext.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting payment with ID {id}.", ex);
            }
        }


        public async Task<List<PaymentDto>> GetPaymentDetailsData()
        {
            try
            {
                var payments = await _dbContext.Payments
                    .Include(p => p.Appointment)
                    .Select(p => new PaymentDto
                    {
                        ID = p.ID,
                        Amount = p.Amount,
                        PaymentDate = p.PaymentDate,
                        PaymentMethod = p.PaymentMethod,
                        Patient = new PatientDto
                        {
                            ID = p.Appointment.Patient.ID,
                            FullName = p.Appointment.Patient.FullName,
                            PhoneNumber = p.Appointment.Patient.PhoneNumber
                        },
                        Appointment = new AppointmentDto
                        {
                            StartDateTime = p.Appointment.StartDateTime,
                            EndDateTime = p.Appointment.EndDateTime,
                        }
                    })
                    .ToListAsync();

                return payments;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching payments with patient and appointment details.", ex);
            }
        }
    }
}
