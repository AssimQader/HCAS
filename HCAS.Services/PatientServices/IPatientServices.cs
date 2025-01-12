﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCAS.DTO;

namespace HCAS.Services.PatientServices
{
    public interface IPatientServices
    {
        Task<List<PatientDto>> GetAll();
        Task<PatientDto> GetById(int id);
        Task<bool> AddPatient(PatientDto patientDto);
        Task<bool> UpdatePatient(PatientDto patientDto);
        Task<bool> DeletePatient(int id);
        Task<List<AppointmentDto>> GetAppointmentsByPatientId(int patientId);
        Task<List<PaymentDto>> GetPaymentsByPatientId(int patientId);
    }
}
